using System.Collections.Generic;
using UnityEngine;

public static class SfxBank
{
    static AudioSource source;
    static readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    const int Rate = 22050;

    public static bool Enabled = true;

    public static void Init(GameObject host)
    {
        if (source != null) return;
        source = host.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.volume = 0.45f;
        source.spatialBlend = 0f;
    }

    public static void Play(string name, float volume = 1f)
    {
        if (!Enabled || source == null) return;
        var clip = Clip(name);
        if (clip != null) source.PlayOneShot(clip, volume);
    }

    public static void Toggle()
    {
        Enabled = !Enabled;
    }

    static AudioClip Clip(string name)
    {
        AudioClip c;
        if (clips.TryGetValue(name, out c)) return c;
        switch (name)
        {
            case "click": c = Make(name, new[] { 900f }, 0.05f, 60f, 0.35f, false); break;
            case "open": c = Make(name, new[] { 520f }, 0.07f, 30f, 0.35f, false); break;
            case "found": c = Make(name, new[] { 700f, 1040f }, 0.09f, 22f, 0.4f, false); break;
            case "wrong": c = Make(name, new[] { 180f, 130f }, 0.18f, 9f, 0.4f, true); break;
            case "solved": c = Make(name, new[] { 523f, 659f, 784f, 1046f }, 0.13f, 12f, 0.4f, false); break;
            case "arrest": c = Make(name, new[] { 392f, 523f, 659f, 784f, 1046f }, 0.2f, 6f, 0.4f, false); break;
            case "accuse": c = Make(name, new[] { 300f }, 0.12f, 10f, 0.4f, true); break;
            default: return null;
        }
        clips[name] = c;
        return c;
    }

    static AudioClip Make(string name, float[] freqs, float noteDur, float decay, float vol, bool square)
    {
        int noteSamples = Mathf.Max(1, (int)(noteDur * Rate));
        int total = noteSamples * freqs.Length;
        var data = new float[total];
        for (int n = 0; n < freqs.Length; n++)
        {
            float f = freqs[n];
            for (int i = 0; i < noteSamples; i++)
            {
                float t = i / (float)Rate;
                float wave = Mathf.Sin(2f * Mathf.PI * f * t);
                if (square) wave = Mathf.Sign(wave) * 0.5f + wave * 0.5f;
                float env = Mathf.Exp(-t * decay) * Mathf.Clamp01(1f - i / (float)noteSamples + 0.05f);
                data[n * noteSamples + i] = wave * env * vol;
            }
        }
        var clip = AudioClip.Create(name, total, 1, Rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
