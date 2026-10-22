using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelUI : MonoBehaviour
{
    public GameObject panelRoot;
    public Transform content;
    public Action<string> onAccuse;

    CaseFile cf;
    TaleDef tale;
    bool isOpen;
    bool revealArmed;
    readonly Dictionary<string, Text> cardLabels = new Dictionary<string, Text>();
    readonly Dictionary<string, Image> cardBg = new Dictionary<string, Image>();

    void Start() { if (panelRoot != null) panelRoot.SetActive(false); }
    void Update() { if (Input.GetKeyDown(KeyCode.P)) Toggle(); }

    public bool IsOpen => isOpen;

    public void Configure(CaseFile caseFile, TaleDef taleDef, Action<string> accuseCallback)
    {
        cf = caseFile; tale = taleDef; onAccuse = accuseCallback;
        revealArmed = false;
        Rebuild();
    }

    public void Toggle() => SetOpen(!isOpen);

    public void SetOpen(bool open)
    {
        isOpen = open;
        if (panelRoot != null) panelRoot.SetActive(open);
    }

    public void OpenAccuse()
    {
        SetOpen(true);
    }

    void Rebuild()
    {
        if (content == null) return;
        for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
        cardLabels.Clear(); cardBg.Clear();
        if (cf == null) return;

        UIKit.Label(content, $"PERSONS OF INTEREST - {cf.city}", 26, UIKit.Amber, TextAnchor.MiddleCenter,
                    Vector2.zero, Vector2.one).gameObject.AddComponent<LayoutElement>().preferredHeight = 40;

        foreach (var s in cf.suspects)
        {
            var card = UIKit.Panel(content, "Card_" + s.id, new Color(1, 1, 1, 0.06f), Vector2.zero, Vector2.one);
            card.gameObject.AddComponent<LayoutElement>().preferredHeight = 170;
            cardBg[s.id] = card.GetComponent<Image>();

            UIKit.Portrait(card, s, new Vector2(0.01f, 0.06f), new Vector2(0.14f, 0.94f));

            string statements = "";
            int shown = 0;
            foreach (var line in s.statements)
            {
                if (shown >= 2) break;
                statements += (shown == 0 ? "" : " ") + "\"" + line + "\"";
                shown++;
            }
            string body = $"<b>{s.name}</b> <color=#B9C2CC>({s.age})</color> - {s.role}\n"
                        + $"<color=#9FB0C0>Alibi:</color> {s.alibi}";
            if (!string.IsNullOrEmpty(statements)) body += $"\n<color=#8FA0B0>{statements}</color>";
            var lbl = UIKit.Label(card, body, 22, UIKit.Body, TextAnchor.MiddleLeft, new Vector2(0.15f, 0.3f), new Vector2(0.99f, 0.97f));
            cardLabels[s.id] = lbl;

            if (onAccuse != null)
            {
                string id = s.id;
                UIKit.SmallButton(card, "Accuse " + s.LastName, new Vector2(0.15f, 0.05f), new Vector2(0.4f, 0.27f),
                                  () => onAccuse(id), true, new Color(0.4f, 0.16f, 0.16f, 0.95f));
            }
        }

        if (cf.victim != null)
        {
            var v = cf.victim;
            var vp = UIKit.Panel(content, "VictimCard", new Color(0.35f, 0.12f, 0.12f, 0.35f), Vector2.zero, Vector2.one);
            vp.gameObject.AddComponent<LayoutElement>().preferredHeight = 130;
            UIKit.Portrait(vp, v, new Vector2(0.01f, 0.08f), new Vector2(0.12f, 0.92f));
            UIKit.Label(vp, $"<b>VICTIM</b>  {v.name}, {v.age} - {v.role}\n<color=#C9A0A0>{v.bio}</color>",
                        22, UIKit.Body, TextAnchor.MiddleLeft, new Vector2(0.13f, 0.08f), new Vector2(0.99f, 0.92f));
        }
    }

    public void RevealCulprit(string culpritId)
    {
        if (revealArmed || cf == null || cf.culprit == null) return;
        revealArmed = true;
        Image bg;
        if (cardBg.TryGetValue(culpritId, out bg)) bg.color = new Color(0.2f, 0.5f, 0.2f, 0.55f);
        Text lbl;
        if (cardLabels.TryGetValue(culpritId, out lbl))
            lbl.text += "\n<color=#7CFC7C>The chief's warrant tip - press this one.</color>";
    }

    public void ClearReveal(string culpritId)
    {
        revealArmed = false;
        Image bg;
        if (cardBg.TryGetValue(culpritId, out bg)) bg.color = new Color(1, 1, 1, 0.06f);
    }
}
