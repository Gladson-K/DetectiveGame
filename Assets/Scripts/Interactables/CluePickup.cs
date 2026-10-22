using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CluePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ClueDefinition clueData;
    [SerializeField] private bool deactivateOnCollect = true;
    private bool isCollected;

    public ClueDefinition ClueData => clueData;

    public void Setup(ClueDefinition data) { clueData = data; }

    private void Start()
    {
        if (clueData == null) Debug.LogError($"[CluePickup] Missing ClueDefinition on '{name}'.", this);
        else if (ClueManager.Instance != null) ClueManager.Instance.RegisterPickup(this);
    }

    public void OnInteract()
    {
        if (isCollected || clueData == null) return;
        isCollected = true;

        if (ClueManager.Instance != null) ClueManager.Instance.CollectClue(clueData);
        else Debug.LogError("[CluePickup] ClueManager.Instance is null.");

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        if (deactivateOnCollect) gameObject.SetActive(false);
    }

    public string GetPrompt() => clueData != null ? $"Examine {clueData.displayName}" : "Examine";
}
