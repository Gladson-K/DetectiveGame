using UnityEngine;
using UnityEngine.UI;

public class CaseTimer : MonoBehaviour
{
    public Text label;
    public bool Running;
    public float Elapsed { get; private set; }

    private void Update()
    {
        if (!Running) return;
        Elapsed += Time.deltaTime;
        if (label != null) label.text = Format(Elapsed);
    }

    public void AddSeconds(float seconds) { Elapsed = Mathf.Max(0f, Elapsed + seconds); }

    public static string Format(float s) => $"{(int)(s / 60):00}:{(int)(s % 60):00}";
}
