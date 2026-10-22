using System;
using System.Collections;
using UnityEngine;

public class CaseCompletionHandler : MonoBehaviour
{
    public Action onSolved;
    public float delaySeconds = 0.6f;

    private void OnEnable() { if (ClueManager.Instance != null) ClueManager.Instance.OnAllCluesCollected += Handle; }
    private void OnDisable() { if (ClueManager.Instance != null) ClueManager.Instance.OnAllCluesCollected -= Handle; }

    private void Handle() { StartCoroutine(Routine()); }
    private IEnumerator Routine() { yield return new WaitForSeconds(delaySeconds); onSolved?.Invoke(); }
}
