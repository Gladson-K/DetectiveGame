using System;
using UnityEngine;
using UnityEngine.UI;

public static class MainMenuUI
{
    public static void Build(Transform root, Action<TaleDef> openTale, Action newIdentity)
    {
        UIKit.Label(root, "MARKED", 84, UIKit.Amber, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.9f), new Vector2(0.95f, 1f));
        UIKit.Label(root, "<color=#E62626>X</color>  <color=#40D9F2>\u221E</color>  Tales of Signs and Shadows",
                    30, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.845f), new Vector2(0.95f, 0.9f));

        UIKit.Label(root, $"Detective Identity <b>#{SeedService.IdentityCode}</b> - your case files are generated uniquely for you",
                    22, UIKit.Body, TextAnchor.MiddleLeft, new Vector2(0.05f, 0.78f), new Vector2(0.6f, 0.84f));
        UIKit.SmallButton(root, SfxBank.Enabled ? "Sound: ON" : "Sound: OFF", new Vector2(0.62f, 0.785f), new Vector2(0.78f, 0.835f),
                          () => { SfxBank.Toggle(); GameController.Instance.RefreshMenu(); }, true, new Color(0.16f, 0.2f, 0.26f, 0.95f));
        UIKit.SmallButton(root, "New Identity", new Vector2(0.79f, 0.785f), new Vector2(0.95f, 0.835f),
                          () => newIdentity(), true, new Color(0.4f, 0.16f, 0.16f, 0.95f));

        Transform content;
        UIKit.Scroll(root, "TaleList", new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.78f), out content);

        var tales = TaleLibrary.All;
        if (tales.Count == 0)
        {
            UIKit.Label(content, "No tales found. Add JSON files to Assets/Resources/Tales/ (see Docs/V2_AUTHORING_GUIDE.md).",
                        26, new Color(1f, 0.5f, 0.5f), TextAnchor.MiddleCenter, Vector2.zero, Vector2.one)
                        .gameObject.AddComponent<LayoutElement>().preferredHeight = 200;
        }
        foreach (var tale in tales) BuildTaleCard(content, tale, openTale);

        UIKit.Label(root, "Find every clue in the room, read the journal, then name the killer. Each case is freshly generated - no two detectives see the same case file.",
                    22, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.11f));
    }

    static void BuildTaleCard(Transform parent, TaleDef tale, Action<TaleDef> openTale)
    {
        var card = UIKit.Panel(parent, "Tale_" + tale.id, new Color(1, 1, 1, 0.05f), Vector2.zero, Vector2.one);
        card.gameObject.AddComponent<LayoutElement>().preferredHeight = 260;

        int solved = 0, total = 0, arrested = 0;
        foreach (var arc in tale.arcs)
        {
            var cases = CaseGenerator.GetArcCases(tale, arc);
            total += cases.Count;
            solved += CaseGenerator.SolvedCount(tale, arc);
            if (SeedService.IsArrested(tale.id, arc.id)) arrested++;
        }
        bool complete = total > 0 && solved >= total;

        UIKit.Label(card, tale.title, 38, complete ? new Color(0.5f, 1f, 0.5f) : UIKit.Amber,
                    TextAnchor.UpperLeft, new Vector2(0.02f, 0.66f), new Vector2(0.6f, 1f));
        UIKit.Label(card, tale.subtitle, 24, UIKit.Body, TextAnchor.UpperLeft, new Vector2(0.02f, 0.54f), new Vector2(0.6f, 0.68f));
        UIKit.Label(card, tale.description, 22, new Color(0.75f, 0.78f, 0.82f), TextAnchor.UpperLeft,
                    new Vector2(0.02f, 0.24f), new Vector2(0.6f, 0.55f));

        string status = complete
            ? "<color=#7CFC7C>TALE COMPLETE - all menaces captured</color>"
            : $"Cases closed {solved}/{total}   -   menaces behind bars {arrested}/{tale.arcs.Count}   -   {tale.characters.Count} persons on file";
        UIKit.Label(card, status, 22, UIKit.Body, TextAnchor.MiddleLeft, new Vector2(0.02f, 0.13f), new Vector2(0.6f, 0.23f));

        int shown = 0;
        float[] xs = { 0.63f, 0.8f };
        float[] ys = { 0.5f, 0.08f };
        for (int i = 0; i < tale.characters.Count && shown < 4; i++)
        {
            var c = tale.characters[i];
            var cell = UIKit.Panel(card, "P_" + c.id, new Color(0, 0, 0, 0.4f), new Vector2(xs[shown % 2], ys[shown / 2]), new Vector2(xs[shown % 2] + 0.16f, ys[shown / 2] + 0.42f));
            UIKit.Portrait(cell, c, new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.98f));
            shown++;
        }
        if (tale.characters.Count > shown)
            UIKit.Label(card, $"+{tale.characters.Count - shown} more", 20, UIKit.Body, TextAnchor.LowerRight, new Vector2(0.6f, 0.02f), new Vector2(0.98f, 0.12f));

        UIKit.MakeButton(card, "Open File", new Vector2(0.63f, 0.02f), new Vector2(0.98f, 0.24f), () => openTale(tale));
    }
}
