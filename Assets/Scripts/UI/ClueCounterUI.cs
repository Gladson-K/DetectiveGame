using UnityEngine;
using UnityEngine.UI;

public class ClueCounterUI : MonoBehaviour
{
    public Text counterText;

    private void OnEnable() { if (ClueManager.Instance != null) ClueManager.Instance.OnProgressChanged += UpdateText; }
    private void OnDisable() { if (ClueManager.Instance != null) ClueManager.Instance.OnProgressChanged -= UpdateText; }
    private void Start() { if (ClueManager.Instance != null) UpdateText(ClueManager.Instance.CollectedCount, ClueManager.Instance.TotalClues); }

    private void UpdateText(int collected, int total)
    {
        if (counterText != null) counterText.text = $"{collected} / {total} Clues Found";
    }
}
