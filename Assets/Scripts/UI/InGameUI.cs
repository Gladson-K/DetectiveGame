using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayScreenRefs
{
    public GameObject hud;
    public Text toastText;
    public CanvasGroup toastGroup;
    public CaseTimer timer;
    public ClueJournalUI journal;
    public CharacterPanelUI persons;
    public Button accuseButton;
    public Text accuseLabel;
}

public static class InGameUI
{
    public static PlayScreenRefs BuildHud(Transform root, TaleDef tale, ArcDef arc, CaseFile cf,
                                          Action menuAction, Action accuseAction, Action<string> onAccuseSuspect)
    {
        var refs = new PlayScreenRefs();
        var hud = UIKit.Panel(root, "HUD", new Color(0, 0, 0, 0.62f), new Vector2(0, 0.93f), new Vector2(1, 1));
        refs.hud = hud.gameObject;

        UIKit.Label(hud, $"<color=#{ColorUtility.ToHtmlStringRGB(arc.MarkColor)}>{arc.markName}</color>  {cf.displayCode}  {cf.title}",
                    28, UIKit.Amber, TextAnchor.MiddleLeft, new Vector2(0.01f, 0), new Vector2(0.34f, 1));
        var counter = UIKit.Label(hud, "0 / 0 Clues Found", 26, Color.white, TextAnchor.MiddleCenter, new Vector2(0.34f, 0), new Vector2(0.47f, 1));
        hud.gameObject.AddComponent<ClueCounterUI>().counterText = counter;

        var timeText = UIKit.Label(hud, "00:00", 26, Color.white, TextAnchor.MiddleCenter, new Vector2(0.47f, 0), new Vector2(0.55f, 1));
        refs.timer = hud.gameObject.AddComponent<CaseTimer>(); refs.timer.label = timeText;

        var journal = hud.gameObject.AddComponent<ClueJournalUI>();
        var persons = hud.gameObject.AddComponent<CharacterPanelUI>();

        UIKit.SmallButton(hud, "Journal (J)", new Vector2(0.55f, 0.12f), new Vector2(0.66f, 0.88f), () => journal.Toggle());
        UIKit.SmallButton(hud, "Persons (P)", new Vector2(0.67f, 0.12f), new Vector2(0.78f, 0.88f), () => persons.Toggle());
        refs.accuseButton = UIKit.SmallButton(hud, "Accuse", new Vector2(0.79f, 0.12f), new Vector2(0.89f, 0.88f),
                                              () => accuseAction(), true, new Color(0.42f, 0.16f, 0.16f, 0.95f));
        refs.accuseLabel = refs.accuseButton.GetComponentInChildren<Text>();
        UIKit.SmallButton(hud, "Menu", new Vector2(0.9f, 0.12f), new Vector2(0.99f, 0.88f), () => menuAction());

        var journalPanel = UIKit.Panel(root, "JournalPanel", new Color(0, 0, 0, 0.94f), new Vector2(0.12f, 0.08f), new Vector2(0.88f, 0.9f));
        UIKit.Label(journalPanel, "CLUE JOURNAL", 44, UIKit.Amber, TextAnchor.MiddleCenter, new Vector2(0, 0.9f), new Vector2(0.8f, 1f));
        Transform journalContent;
        UIKit.Scroll(journalPanel, "JournalScroll", new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.89f), out journalContent);
        UIKit.SmallButton(journalPanel, "Close", new Vector2(0.82f, 0.9f), new Vector2(0.98f, 0.99f), journal.Toggle);
        journal.journalRoot = journalPanel.gameObject;
        journal.entryContainer = journalContent;
        refs.journal = journal;

        var personsPanel = UIKit.Panel(root, "PersonsPanel", new Color(0.03f, 0.03f, 0.05f, 0.96f), new Vector2(0.08f, 0.05f), new Vector2(0.92f, 0.92f));
        UIKit.Label(personsPanel, "PERSONS OF INTEREST", 40, UIKit.Amber, TextAnchor.MiddleCenter, new Vector2(0, 0.91f), new Vector2(0.8f, 1f));
        UIKit.Label(personsPanel, "Read every alibi. The evidence in your journal points to someone. Wrong accusation = +30s penalty.",
                    20, UIKit.Body, TextAnchor.MiddleCenter, new Vector2(0.02f, 0.865f), new Vector2(0.98f, 0.905f));
        Transform personsContent;
        UIKit.Scroll(personsPanel, "PersonsScroll", new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.86f), out personsContent);
        UIKit.SmallButton(personsPanel, "Close", new Vector2(0.82f, 0.91f), new Vector2(0.98f, 0.99f), persons.Toggle);
        persons.panelRoot = personsPanel.gameObject;
        persons.content = personsContent;
        persons.Configure(cf, tale, onAccuseSuspect);
        refs.persons = persons;

        var popup = UIKit.Panel(root, "CluePopup", new Color(0.1f, 0.1f, 0.12f, 0.92f), new Vector2(0.28f, 0.13f), new Vector2(0.72f, 0.24f));
        var cg = popup.gameObject.AddComponent<CanvasGroup>(); cg.alpha = 0; cg.blocksRaycasts = false; cg.interactable = false;
        var pt = UIKit.Label(popup, "", 30, UIKit.Amber, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        var popupUI = popup.gameObject.AddComponent<CluePopupUI>(); popupUI.canvasGroup = cg; popupUI.titleText = pt;

        var toast = UIKit.Panel(root, "Toast", new Color(0.08f, 0.08f, 0.1f, 0.9f), new Vector2(0.25f, 0.015f), new Vector2(0.75f, 0.1f));
        var tcg = toast.gameObject.AddComponent<CanvasGroup>(); tcg.alpha = 0; tcg.blocksRaycasts = false; tcg.interactable = false;
        var tt = UIKit.Label(toast, "", 26, UIKit.Body, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        refs.toastText = tt; refs.toastGroup = tcg;

        return refs;
    }
}
