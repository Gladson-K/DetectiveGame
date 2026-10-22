using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    Camera cam;
    ClickDetector clicker;
    GameObject menuRoot, playRoot;
    Transform playCanvasRoot;
    CaseTimer timer;
    PlayScreenRefs refs;
    TaleDef curTale;
    ArcDef curArc;
    CaseFile curCase;
    Coroutine toastRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (Instance != null) return;
        var go = new GameObject("GameController");
        go.AddComponent<GameController>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        Application.targetFrameRate = 60;
        Screen.autorotateToPortrait = false; Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true; Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        if (EventSystem.current == null) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        cam = Camera.main;
        if (cam == null)
        {
            var cg = new GameObject("Main Camera") { tag = "MainCamera" };
            cam = cg.AddComponent<Camera>(); cg.AddComponent<AudioListener>();
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.07f);
        cam.fieldOfView = 55f;
        cam.transform.position = new Vector3(0, 1.9f, -5.5f);
        cam.transform.LookAt(new Vector3(0, 1.1f, 1.5f));
        clicker = cam.gameObject.GetComponent<ClickDetector>() ?? cam.gameObject.AddComponent<ClickDetector>();
        clicker.enabled = false;

        SfxBank.Init(gameObject);
        ShowMenu();
    }

    void ClearScreens()
    {
        if (menuRoot != null) Destroy(menuRoot);
        if (playRoot != null) Destroy(playRoot);
        menuRoot = playRoot = null;
        clicker.enabled = false;
        if (toastRoutine != null) { StopCoroutine(toastRoutine); toastRoutine = null; }
        if (ClueManager.Instance != null) ClueManager.Instance.OnClueCollected -= HandleClueCollected;
    }

    Transform menuCanvasRoot;

    Transform NewMenuScreen()
    {
        menuRoot = new GameObject("MenuRoot");
        menuCanvasRoot = UIKit.CreateCanvas(menuRoot.transform, "MenuCanvas").transform;
        return menuCanvasRoot;
    }

    // ---------------------------------------------------------------- MENU
    public void ShowMenu()
    {
        ClearScreens();
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.12f);
        var root = NewMenuScreen();
        MainMenuUI.Build(root, ShowTale, ConfirmNewIdentity);
    }

    public void RefreshMenu() => ShowMenu();

    void ConfirmNewIdentity()
    {
        Transform host = menuCanvasRoot;
        if (host == null) return;
        var panel = UIKit.Panel(host, "ConfirmIdentity", new Color(0, 0, 0, 0.85f), new Vector2(0.25f, 0.35f), new Vector2(0.75f, 0.65f));
        UIKit.Label(panel, "Start a brand new detective identity?\nAll progress and the unique case files you were given will be replaced.",
                    28, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.35f), new Vector2(0.95f, 0.95f));
        UIKit.MakeButton(panel, "Yes, new identity", new Vector2(0.06f, 0.06f), new Vector2(0.48f, 0.3f), () =>
        {
            SeedService.ResetIdentity();
            CaseGenerator.ClearCache();
            PortraitLibrary.Clear();
            ShowMenu();
        });
        UIKit.MakeButton(panel, "Cancel", new Vector2(0.52f, 0.06f), new Vector2(0.94f, 0.3f), () => Destroy(panel.gameObject));
    }

    void ShowTale(TaleDef tale)
    {
        ClearScreens();
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.12f);
        var root = NewMenuScreen();

        UIKit.Label(root, tale.title, 60, UIKit.Amber, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.9f), new Vector2(0.95f, 0.99f));
        UIKit.Label(root, tale.subtitle, 26, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.845f), new Vector2(0.95f, 0.9f));
        UIKit.SmallButton(root, "Back to Tales", new Vector2(0.03f, 0.845f), new Vector2(0.2f, 0.9f), ShowMenu);

        Transform content;
        UIKit.Scroll(root, "ArcList", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.84f), out content);

        foreach (var arc in tale.arcs) BuildArcSection(content, tale, arc);

        UIKit.Label(root, "Tip: clues in your journal name the person they point to. Read the alibis, then accuse.",
                    20, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.005f), new Vector2(0.95f, 0.06f));
    }

    void BuildArcSection(Transform parent, TaleDef tale, ArcDef arc)
    {
        var cases = CaseGenerator.GetArcCases(tale, arc);
        int solved = CaseGenerator.SolvedCount(tale, arc);
        bool arrested = SeedService.IsArrested(tale.id, arc.id);

        var panel = UIKit.Panel(parent, "Arc_" + arc.id, new Color(1, 1, 1, 0.05f), Vector2.zero, Vector2.one);
        panel.gameObject.AddComponent<LayoutElement>().preferredHeight = 120 + cases.Count * 56 + 60;

        string markColor = "#" + ColorUtility.ToHtmlStringRGB(arc.MarkColor);
        UIKit.Label(panel, $"<color={markColor}>{arc.markName}</color>  {arc.title}", 36, UIKit.Amber,
                    TextAnchor.MiddleLeft, new Vector2(0.02f, 0.78f), new Vector2(0.75f, 0.98f));

        string status = arrested ? "<color=#7CFC7C>ARRESTED - behind bars</color>"
            : $"Cases closed {solved}/{cases.Count} - close them all to capture {arc.menaceName}";
        UIKit.Label(panel, status, 24, UIKit.Body, TextAnchor.MiddleLeft, new Vector2(0.02f, 0.66f), new Vector2(0.75f, 0.78f));

        bool canReshuffle = solved == 0 && !arrested;
        UIKit.SmallButton(panel, "Reshuffle Case Files", new Vector2(0.77f, 0.72f), new Vector2(0.98f, 0.86f), () =>
        {
            SeedService.Reshuffle(tale.id, arc.id);
            CaseGenerator.ClearCache();
            SfxBank.Play("open");
            ShowTale(tale);
        }, canReshuffle, new Color(0.18f, 0.24f, 0.32f, 0.95f));
        if (!canReshuffle)
            UIKit.Label(panel, "Files locked once a case is closed", 18, new Color(0.6f, 0.62f, 0.66f),
                        TextAnchor.MiddleRight, new Vector2(0.6f, 0.6f), new Vector2(0.98f, 0.72f));

        for (int i = 0; i < cases.Count; i++)
        {
            var cf = cases[i];
            bool solvedCase = SeedService.IsSolved(SeedService.CaseKey(tale.id, arc.id, cf.slotIndex, cf.hash));
            bool unlocked = i == 0 || SeedService.IsSolved(SeedService.CaseKey(tale.id, arc.id, cases[i - 1].slotIndex, cases[i - 1].hash));
            if (arrested) unlocked = false;

            string victimName = cf.victim != null ? cf.victim.name : "unknown";
            string tag = solvedCase ? "  [SOLVED]" : (unlocked ? "" : "  [LOCKED]");
            float top = 0.6f - i * 0.145f;
            int slot = i;
            UIKit.SmallButton(panel, $"{cf.displayCode}  Case {i + 1}: {cf.title} - {victimName}, {cf.city}{tag}",
                              new Vector2(0.03f, top - 0.12f), new Vector2(0.97f, top), () => StartCase(tale, arc, slot),
                              unlocked, solvedCase ? new Color(0.14f, 0.26f, 0.16f, 0.95f) : new Color(0.18f, 0.18f, 0.22f, 0.95f));
        }
    }

    // ---------------------------------------------------------------- PLAY
    void StartCase(TaleDef tale, ArcDef arc, int slot)
    {
        curTale = tale; curArc = arc;
        curCase = CaseGenerator.GetCase(tale, arc, slot);
        if (curCase == null) { ShowTale(tale); return; }
        curCase.mistakes = 0;
        BuildPlayScreen();
    }

    void BuildPlayScreen()
    {
        ClearScreens();
        playRoot = new GameObject("PlayRoot");

        var mgr = new GameObject("ClueManager");
        mgr.transform.SetParent(playRoot.transform, false);
        mgr.AddComponent<ClueManager>();
        CrimeSceneBuilder.Build(curCase, curTale, playRoot.transform);

        playCanvasRoot = UIKit.CreateCanvas(playRoot.transform, "PlayCanvas").transform;
        refs = InGameUI.BuildHud(playCanvasRoot, curTale, curArc, curCase, ShowMenu, RequestAccusation, ConfirmAccuse);
        timer = refs.timer;
        timer.Running = false;

        var done = playRoot.AddComponent<CaseCompletionHandler>();
        done.onSolved = OnAllCluesFound;

        if (ClueManager.Instance != null) ClueManager.Instance.OnClueCollected += HandleClueCollected;

        BuildBriefing();
    }

    void HandleClueCollected(ClueDefinition clue) => SfxBank.Play("found");

    void BuildBriefing()
    {
        var brief = UIKit.Panel(playCanvasRoot, "Briefing", new Color(0, 0, 0, 0.9f), Vector2.zero, Vector2.one);
        var victim = curCase.victim;

        UIKit.Label(brief, $"CASE FILE {curCase.displayCode}", 34, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.9f), new Vector2(0.95f, 0.97f));
        UIKit.Label(brief, curCase.title, 56, UIKit.Amber, TextAnchor.MiddleCenter, new Vector2(0.05f, 0.8f), new Vector2(0.95f, 0.9f));

        var portrait = UIKit.Panel(brief, "VictimBox", new Color(0, 0, 0, 0.35f), new Vector2(0.06f, 0.24f), new Vector2(0.28f, 0.78f));
        UIKit.Portrait(portrait, victim, new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.98f));
        UIKit.Label(portrait, victim != null ? $"<b>{victim.name}</b>\n{victim.age} - {victim.role}\n{victim.city}" : "UNIDENTIFIED VICTIM",
                    22, UIKit.Body, TextAnchor.UpperCenter, new Vector2(0.02f, 0f), new Vector2(0.98f, 0.16f));

        string body = $"{curCase.situation}\n\n{curCase.briefing}\n\n"
                    + $"<color=#{ColorUtility.ToHtmlStringRGB(curArc.MarkColor)}>{curArc.markName}</color> - the killer's signature is in this room too.\n"
                    + "Find every clue, read every alibi, then accuse the right person.";
        UIKit.Label(brief, body, 26, UIKit.Body, TextAnchor.UpperLeft, new Vector2(0.3f, 0.24f), new Vector2(0.94f, 0.78f));

        int shown = 0;
        float x = 0.3f;
        foreach (var s in curCase.suspects)
        {
            if (shown >= 5) break;
            var cell = UIKit.Panel(brief, "S_" + s.id, new Color(1, 1, 1, 0.06f), new Vector2(x, 0.1f), new Vector2(x + 0.12f, 0.23f));
            UIKit.Portrait(cell, s, new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.98f));
            UIKit.Label(cell, s.LastName, 18, UIKit.Body, TextAnchor.LowerCenter, Vector2.zero, Vector2.one);
            x += 0.13f; shown++;
        }

        UIKit.MakeButton(brief, "Begin Investigation", new Vector2(0.35f, 0.015f), new Vector2(0.65f, 0.09f), () =>
        {
            SfxBank.Play("open");
            Destroy(brief.gameObject);
            clicker.enabled = true;
            timer.Running = true;
        });
    }

    void OnAllCluesFound()
    {
        ShowToast("All clues found - time to name the killer. Press Accuse.", UIKit.Amber);
        if (refs != null && refs.accuseLabel != null)
        {
            refs.accuseLabel.text = "Accuse!";
            refs.accuseLabel.color = new Color(1f, 0.85f, 0.5f);
        }
    }

    void RequestAccusation()
    {
        if (refs == null || refs.persons == null) return;
        SfxBank.Play("open");
        refs.persons.OpenAccuse();
        ShowToast("Pick the suspect whose story the evidence destroys.", UIKit.Body);
    }

    void ConfirmAccuse(string suspectId)
    {
        var suspect = curCase.suspects.Find(s => s.id == suspectId);
        if (suspect == null || playCanvasRoot == null) return;
        SfxBank.Play("click");

        var panel = UIKit.Panel(playCanvasRoot, "ConfirmAccuse", new Color(0, 0, 0, 0.88f), new Vector2(0.28f, 0.32f), new Vector2(0.72f, 0.68f));
        UIKit.Portrait(panel, suspect, new Vector2(0.04f, 0.3f), new Vector2(0.28f, 0.95f));
        UIKit.Label(panel, $"Accuse <b>{suspect.name}</b>?", 34, UIKit.Amber, TextAnchor.MiddleLeft, new Vector2(0.3f, 0.72f), new Vector2(0.96f, 0.95f));
        UIKit.Label(panel, $"{suspect.role}, {suspect.city}\n\"{suspect.alibi}\"", 22, UIKit.Body, TextAnchor.UpperLeft, new Vector2(0.3f, 0.4f), new Vector2(0.96f, 0.72f));
        UIKit.Label(panel, "A wrong accusation costs 30 seconds.", 20, new Color(1f, 0.6f, 0.6f), TextAnchor.MiddleLeft, new Vector2(0.3f, 0.3f), new Vector2(0.96f, 0.4f));

        UIKit.MakeButton(panel, "Make the accusation", new Vector2(0.05f, 0.05f), new Vector2(0.5f, 0.24f), () =>
        {
            Destroy(panel.gameObject);
            DoAccuse(suspect);
        }, true);
        UIKit.MakeButton(panel, "Not yet", new Vector2(0.52f, 0.05f), new Vector2(0.95f, 0.24f), () => Destroy(panel.gameObject));
    }

    void DoAccuse(CharacterDef suspect)
    {
        if (curCase == null) return;
        if (suspect.id == (curCase.culprit != null ? curCase.culprit.id : ""))
        {
            SfxBank.Play("solved");
            OnCaseSolved();
        }
        else
        {
            curCase.mistakes++;
            if (timer != null) timer.AddSeconds(30f);
            SfxBank.Play("wrong");
            ShowToast($"{suspect.name} has an alibi for that. +30 seconds. ({curCase.mistakes} wrong)", new Color(1f, 0.55f, 0.55f));
            if (curCase.mistakes >= 2 && refs != null && refs.persons != null)
                refs.persons.RevealCulprit(curCase.culprit != null ? curCase.culprit.id : "");
        }
    }

    void OnCaseSolved()
    {
        clicker.enabled = false;
        if (timer != null) timer.Running = false;
        if (refs != null && refs.persons != null) refs.persons.SetOpen(false);

        SeedService.MarkSolved(SeedService.CaseKey(curTale.id, curArc.id, curCase.slotIndex, curCase.hash));

        bool arcComplete = CaseGenerator.AllSolved(curTale, curArc);
        bool newlyArrested = false;
        if (arcComplete && !SeedService.IsArrested(curTale.id, curArc.id))
        {
            SeedService.MarkArrested(curTale.id, curArc.id);
            newlyArrested = true;
            SfxBank.Play("arrest");
        }

        bool taleComplete = true;
        foreach (var a in curTale.arcs)
            if (!CaseGenerator.AllSolved(curTale, a)) { taleComplete = false; break; }

        var p = UIKit.Panel(playCanvasRoot, "CaseSolvedPanel", new Color(0, 0, 0, 0.9f), Vector2.zero, Vector2.one);
        UIKit.Label(p, newlyArrested ? "MENACE CAPTURED" : "CASE CLOSED", 64, UIKit.Amber, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.85f), new Vector2(0.9f, 0.98f));

        var culprit = curCase.culprit;
        var box = UIKit.Panel(p, "CulpritBox", new Color(0.25f, 0.1f, 0.1f, 0.5f), new Vector2(0.33f, 0.42f), new Vector2(0.67f, 0.84f));
        UIKit.Portrait(box, culprit, new Vector2(0.28f, 0.18f), new Vector2(0.72f, 0.98f));
        UIKit.Label(box, culprit != null ? $"<b>{culprit.name}</b>\n{culprit.role}" : "", 24, UIKit.Body,
                    TextAnchor.UpperCenter, new Vector2(0.02f, 0f), new Vector2(0.98f, 0.2f));

        string arcLine = newlyArrested
            ? $"<color=#7CFC7C>{curArc.menaceName} is behind bars. The {curArc.markName} case is closed for good.</color>"
            : $"{curArc.menaceName} is still out there. Cases closed {CaseGenerator.SolvedCount(curTale, curArc)}/{CaseGenerator.GetArcCases(curTale, curArc).Count}.";
        string taleLine = taleComplete ? "\n<color=#7CFC7C>Every tale in this file is solved. You beat the game.</color>"
                                       : "";
        int wrong = curCase.mistakes;
        UIKit.Label(p, $"Time: {CaseTimer.Format(timer.Elapsed)}   -   Wrong accusations: {wrong}\n{arcLine}{taleLine}",
                    28, UIKit.Body, TextAnchor.UpperCenter, new Vector2(0.12f, 0.24f), new Vector2(0.88f, 0.42f));

        var nextCase = NextUnsolvedCase(curTale, curArc);
        if (nextCase != null)
            UIKit.MakeButton(p, "Next Case: " + nextCase.title, new Vector2(0.28f, 0.12f), new Vector2(0.72f, 0.21f),
                             () => StartCase(curTale, curArc, nextCase.slotIndex));
        UIKit.MakeButton(p, "Case Files", new Vector2(0.28f, 0.02f), new Vector2(0.5f, 0.11f), () => ShowTale(curTale));
        UIKit.MakeButton(p, "Main Menu", new Vector2(0.52f, 0.02f), new Vector2(0.72f, 0.11f), ShowMenu);
    }

    CaseFile NextUnsolvedCase(TaleDef tale, ArcDef arc)
    {
        foreach (var c in CaseGenerator.GetArcCases(tale, arc))
            if (!SeedService.IsSolved(SeedService.CaseKey(tale.id, arc.id, c.slotIndex, c.hash))) return c;
        return null;
    }

    public void ShowToast(string message, Color color)
    {
        if (refs == null || refs.toastText == null) return;
        refs.toastText.text = message;
        refs.toastText.color = color;
        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = StartCoroutine(ToastRoutine());
    }

    IEnumerator ToastRoutine()
    {
        var cg = refs.toastGroup;
        float t = 0f;
        while (t < 0.2f) { t += Time.deltaTime; cg.alpha = Mathf.Lerp(0f, 1f, t / 0.2f); yield return null; }
        cg.alpha = 1f;
        yield return new WaitForSeconds(2.6f);
        t = 0f;
        while (t < 0.3f) { t += Time.deltaTime; cg.alpha = Mathf.Lerp(1f, 0f, t / 0.3f); yield return null; }
        cg.alpha = 0f;
    }
}
