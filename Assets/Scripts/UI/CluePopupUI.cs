using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CluePopupUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Text titleText;
    public float showSeconds = 1.8f, fadeSeconds = 0.25f;
    private Coroutine running;

    private void OnEnable() { if (ClueManager.Instance != null) ClueManager.Instance.OnClueCollected += Show; }
    private void OnDisable() { if (ClueManager.Instance != null) ClueManager.Instance.OnClueCollected -= Show; }
    private void Start() { if (canvasGroup != null) { canvasGroup.alpha = 0f; canvasGroup.blocksRaycasts = false; } }

    private void Show(ClueDefinition clue)
    {
        if (clue == null || canvasGroup == null) return;
        if (titleText != null) titleText.text = "Clue Found: " + clue.displayName;
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(Routine());
    }

    private IEnumerator Routine()
    {
        yield return Fade(1f, fadeSeconds);
        yield return new WaitForSeconds(showSeconds);
        yield return Fade(0f, fadeSeconds);
    }

    private IEnumerator Fade(float target, float duration)
    {
        float start = canvasGroup.alpha, t = 0f;
        while (t < duration) { t += Time.deltaTime; canvasGroup.alpha = Mathf.Lerp(start, target, t / duration); yield return null; }
        canvasGroup.alpha = target;
    }
}
