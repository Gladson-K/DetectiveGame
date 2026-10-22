using System.Collections.Generic;
using UnityEngine;

public static class PortraitLibrary
{
    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite Get(CharacterDef c)
    {
        if (c == null) return Placeholder("?", Color.grey);
        return Get(c.PortraitKey, c.Accent);
    }

    public static Sprite Get(string key, Color accent)
    {
        if (string.IsNullOrEmpty(key)) return Placeholder("?", accent);
        Sprite cached;
        if (cache.TryGetValue(key, out cached)) return cached;

        Sprite s = Resources.Load<Sprite>("Portraits/" + key);
        if (s == null) s = Resources.Load<Sprite>("Portraits/" + key.ToLowerInvariant());
        if (s == null) s = Placeholder(key, accent);

        cache[key] = s;
        return s;
    }

    public static void Clear() => cache.Clear();

    static Sprite Placeholder(string key, Color accent)
    {
        const int S = 256, cells = 9, cell = S / cells;
        var tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color dark = Color.Lerp(accent, Color.black, 0.72f);
        Color light = Color.Lerp(accent, Color.white, 0.25f);

        var px = new Color[S * S];
        for (int i = 0; i < px.Length; i++) px[i] = dark;

        int hash = StableHash(key);
        for (int gx = 0; gx < cells; gx++)
        {
            for (int gy = 0; gy < cells; gy++)
            {
                int mirrorX = gx < cells / 2 + 1 ? gx : cells - 1 - gx;
                int bit = (hash >> ((mirrorX * cells + gy) % 30)) & 1;
                if (bit == 0) continue;
                for (int x = 0; x < cell; x++)
                    for (int y = 0; y < cell; y++)
                        px[(gy * cell + y) * S + (gx * cell + x)] = light;
            }
        }

        int border = 6;
        for (int x = 0; x < S; x++)
            for (int b = 0; b < border; b++)
            {
                px[b * S + x] = accent;
                px[(S - 1 - b) * S + x] = accent;
                px[x * S + b] = accent;
                px[x * S + (S - 1 - b)] = accent;
            }

        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
    }

    static int StableHash(string s)
    {
        unchecked
        {
            int h = 23;
            foreach (char ch in s) h = h * 31 + ch;
            return h;
        }
    }
}
