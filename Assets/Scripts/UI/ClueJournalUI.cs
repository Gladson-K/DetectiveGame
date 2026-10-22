using UnityEngine;
using UnityEngine.UI;

/// NOTE: attach to an ALWAYS-ACTIVE object (the HUD). The original spec attached it to the
/// journal panel, which is deactivated, so the J key and event subscription would stop working.
public class ClueJournalUI : MonoBehaviour
{
    public GameObject journalRoot;
    public Transform entryContainer;
    private bool isOpen;

    private void OnEnable() { if (ClueManager.Instance != null) ClueManager.Instance.OnClueCollected += HandleClueCollected; }
    private void OnDisable() { if (ClueManager.Instance != null) ClueManager.Instance.OnClueCollected -= HandleClueCollected; }
    private void Start() { if (journalRoot != null) journalRoot.SetActive(false); }
    private void Update() { if (Input.GetKeyDown(KeyCode.J)) Toggle(); }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (journalRoot != null) journalRoot.SetActive(isOpen);
    }

    private void HandleClueCollected(ClueDefinition clue)
    {
        if (entryContainer == null || clue == null) return;
        var entry = UIKit.Panel(entryContainer, "Entry_" + clue.clueID, new Color(1, 1, 1, 0.08f), Vector2.zero, Vector2.one);
        entry.gameObject.AddComponent<LayoutElement>().preferredHeight = 108;
        string body = $"<b><color=#E6C27A>{clue.displayName}</color></b>\n{clue.description}";
        if (!string.IsNullOrEmpty(clue.pointsToName))
            body += $"\n<color=#E6725A>Evidence points to: {clue.pointsToName}</color>";
        else if (clue.kind == "Flavor")
            body += "\n<color=#8FA0B0>Context only - no direct lead.</color>";
        UIKit.Label(entry, body, 24, UIKit.Body, TextAnchor.MiddleLeft, Vector2.zero, Vector2.one);
    }
}
