using UnityEngine;

[CreateAssetMenu(fileName = "NewClue", menuName = "DetectiveGame/Clue Definition")]
public class ClueDefinition : ScriptableObject
{
    [Header("Identity")]
    public string clueID;
    public string displayName;

    [Header("Presentation")]
    [TextArea(3, 6)] public string description;
    public Sprite icon;

    [Header("Deduction")]
    public string pointsToName;
    public string kind;

    [Header("Optional")]
    public ClueDefinition[] requiredClues;
}
