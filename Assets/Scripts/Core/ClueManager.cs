using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance { get; private set; }

    public event Action<ClueDefinition> OnClueCollected;
    public event Action<int, int> OnProgressChanged; // (collected, total)
    public event Action OnAllCluesCollected;

    private readonly HashSet<string> collectedIDs = new HashSet<string>();
    private readonly HashSet<CluePickup> registeredPickups = new HashSet<CluePickup>();

    public int TotalClues => registeredPickups.Count;
    public int CollectedCount => collectedIDs.Count;
    public bool AllCollected => TotalClues > 0 && CollectedCount >= TotalClues;

    // A new case always replaces the previous manager (the old one dies with its PlayRoot).
    private void Awake() { Instance = this; }
    private void OnDestroy() { if (Instance == this) Instance = null; }

    public void RegisterPickup(CluePickup pickup)
    {
        if (pickup == null || pickup.ClueData == null) return;
        if (registeredPickups.Add(pickup)) OnProgressChanged?.Invoke(CollectedCount, TotalClues);
    }

    public void CollectClue(ClueDefinition clue)
    {
        if (clue == null || string.IsNullOrEmpty(clue.clueID)) return;
        if (!collectedIDs.Add(clue.clueID)) return;

        OnClueCollected?.Invoke(clue);
        OnProgressChanged?.Invoke(CollectedCount, TotalClues);
        if (AllCollected) OnAllCluesCollected?.Invoke();
    }

    public bool HasClue(string clueID) => collectedIDs.Contains(clueID);
}
