using System.Collections.Generic;
using UnityEngine;

public static class CrimeSceneBuilder
{
    class Layout
    {
        public Vector3 tableTop, tableScale, tableBase, tableBaseScale;
        public Vector3 couchSeat, couchSeatScale, couchBack, couchBackScale;
        public Vector3 shelfBack, shelfScale;
        public float[] plankYs;
        public Vector3[] slots;
    }

    static readonly Layout[] Layouts =
    {
        new Layout
        {
            tableTop = new Vector3(-2.3f, 0.75f, 0.6f), tableScale = new Vector3(2.8f, 0.1f, 1.3f),
            tableBase = new Vector3(-2.3f, 0.37f, 0.6f), tableBaseScale = new Vector3(2.3f, 0.7f, 0.9f),
            couchSeat = new Vector3(2.4f, 0.3f, 0.6f), couchSeatScale = new Vector3(2.6f, 0.6f, 1.2f),
            couchBack = new Vector3(2.4f, 0.85f, 1.15f), couchBackScale = new Vector3(2.6f, 1.0f, 0.3f),
            shelfBack = new Vector3(0, 1.3f, 2.95f), shelfScale = new Vector3(3.4f, 2.6f, 0.1f),
            plankYs = new[] { 0.5f, 1.1f, 1.7f, 2.3f },
            slots = new[]
            {
                new Vector3(-2.9f, 0.98f, 0.6f), new Vector3(-1.7f, 0.98f, 0.7f), new Vector3(-2.3f, 0.98f, 0.25f),
                new Vector3(2.2f, 0.80f, 0.5f), new Vector3(3.4f, 0.80f, 0.7f), new Vector3(2.4f, 1.55f, 1.15f),
                new Vector3(3.6f, 0.20f, -0.4f), new Vector3(-3.6f, 0.20f, -0.8f), new Vector3(0.2f, 0.20f, -0.9f),
                new Vector3(1.0f, 0.20f, -1.6f), new Vector3(-0.9f, 1.28f, 2.55f), new Vector3(0.9f, 0.68f, 2.55f)
            }
        },
        new Layout
        {
            tableTop = new Vector3(2.4f, 0.75f, 0.7f), tableScale = new Vector3(2.6f, 0.1f, 1.2f),
            tableBase = new Vector3(2.4f, 0.37f, 0.7f), tableBaseScale = new Vector3(2.1f, 0.7f, 0.8f),
            couchSeat = new Vector3(-2.5f, 0.3f, 0.5f), couchSeatScale = new Vector3(2.4f, 0.6f, 1.1f),
            couchBack = new Vector3(-2.5f, 0.85f, 1.05f), couchBackScale = new Vector3(2.4f, 1.0f, 0.3f),
            shelfBack = new Vector3(-0.6f, 1.3f, 2.95f), shelfScale = new Vector3(3.0f, 2.6f, 0.1f),
            plankYs = new[] { 0.45f, 1.05f, 1.65f, 2.25f },
            slots = new[]
            {
                new Vector3(3.2f, 0.98f, 0.7f), new Vector3(2.0f, 0.98f, 0.4f), new Vector3(2.5f, 0.98f, 1.1f),
                new Vector3(-2.2f, 0.80f, 0.4f), new Vector3(-3.5f, 0.80f, 0.6f), new Vector3(-2.5f, 1.55f, 1.05f),
                new Vector3(-3.8f, 0.20f, -0.5f), new Vector3(3.8f, 0.20f, -0.9f), new Vector3(-0.3f, 0.20f, -1.0f),
                new Vector3(1.4f, 0.20f, -1.7f), new Vector3(-1.5f, 1.18f, 2.55f), new Vector3(0.6f, 0.62f, 2.55f)
            }
        },
        new Layout
        {
            tableTop = new Vector3(0f, 0.75f, 1.4f), tableScale = new Vector3(3.0f, 0.1f, 1.1f),
            tableBase = new Vector3(0f, 0.37f, 1.4f), tableBaseScale = new Vector3(2.5f, 0.7f, 0.8f),
            couchSeat = new Vector3(-3.6f, 0.3f, 0.2f), couchSeatScale = new Vector3(2.0f, 0.6f, 1.4f),
            couchBack = new Vector3(-4.2f, 0.85f, 0.2f), couchBackScale = new Vector3(0.3f, 1.0f, 1.4f),
            shelfBack = new Vector3(3.9f, 1.4f, 0.6f), shelfScale = new Vector3(0.1f, 2.8f, 2.4f),
            plankYs = new[] { 0.5f, 1.1f, 1.7f, 2.3f },
            slots = new[]
            {
                new Vector3(-0.9f, 0.98f, 1.2f), new Vector3(0.9f, 0.98f, 1.6f), new Vector3(0f, 0.98f, 0.7f),
                new Vector3(-3.5f, 0.80f, -0.2f), new Vector3(-3.0f, 1.55f, 0.6f), new Vector3(-4.0f, 0.80f, 0.9f),
                new Vector3(3.0f, 0.20f, -0.6f), new Vector3(-1.2f, 0.20f, -1.1f), new Vector3(1.8f, 0.20f, -1.3f),
                new Vector3(3.75f, 0.90f, 1.0f), new Vector3(3.75f, 1.70f, -0.2f), new Vector3(3.75f, 0.35f, -0.5f)
            }
        }
    };

    static Color Shade(Color c, float f) => new Color(c.r * f, c.g * f, c.b * f, 1f);

    static GameObject Box(Transform p, string n, Vector3 pos, Vector3 scale, Color col, Vector3? euler = null)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n; g.transform.SetParent(p, false);
        g.transform.localPosition = pos; g.transform.localScale = scale;
        if (euler.HasValue) g.transform.localRotation = Quaternion.Euler(euler.Value);
        g.GetComponent<Renderer>().material.color = col;
        Object.Destroy(g.GetComponent<Collider>());
        return g;
    }

    public static void Build(CaseFile cf, TaleDef tale, Transform parent)
    {
        var arc = tale != null ? tale.FindArc(cf.arcId) : null;
        Color accent = arc != null ? arc.Accent : new Color(0.3f, 0.3f, 0.35f);

        float h, s, v;
        Color.RGBToHSV(accent, out h, out s, out v);
        h = Mathf.Repeat(h + (cf.layoutVariant - 1) * 0.03f + (SeedService.StableHash(cf.hash) % 100) / 2000f, 1f);
        s = Mathf.Clamp(s * 0.75f + 0.05f, 0.08f, 0.75f);
        v = Mathf.Clamp(v * 0.5f + 0.07f, 0.07f, 0.45f);
        Color theme = Color.HSVToRGB(h, s, v);

        RenderSettings.ambientLight = new Color(0.32f, 0.32f, 0.38f);
        var room = new GameObject("Room").transform; room.SetParent(parent, false);
        var L = Layouts[Mathf.Abs(cf.layoutVariant) % Layouts.Length];
        var wood = new Color(0.35f, 0.22f, 0.12f);

        Box(room, "Floor", new Vector3(0, -0.1f, 0), new Vector3(14, 0.2f, 10), Shade(theme, 0.7f));
        Box(room, "BackWall", new Vector3(0, 2.5f, 3.1f), new Vector3(14, 5, 0.2f), Shade(theme, 1.2f));
        Box(room, "LeftWall", new Vector3(-7f, 2.5f, 0), new Vector3(0.2f, 5, 10), Shade(theme, 1.0f));
        Box(room, "RightWall", new Vector3(7f, 2.5f, 0), new Vector3(0.2f, 5, 10), Shade(theme, 1.0f));

        Box(room, "TableTop", L.tableTop, L.tableScale, wood);
        Box(room, "TableBase", L.tableBase, L.tableBaseScale, Shade(wood, 0.7f));
        Box(room, "CouchSeat", L.couchSeat, L.couchSeatScale, new Color(0.3f, 0.3f, 0.4f));
        Box(room, "CouchBack", L.couchBack, L.couchBackScale, new Color(0.26f, 0.26f, 0.36f));
        Box(room, "ShelfBack", L.shelfBack, L.shelfScale, Shade(wood, 0.8f));
        foreach (float y in L.plankYs)
            Box(room, "Plank", new Vector3(L.shelfBack.x, y, L.shelfBack.z - 0.35f), new Vector3(L.shelfScale.z > 1.5f ? 0.7f : 3.4f, 0.08f, 0.7f), wood);

        var sun = new GameObject("Light").AddComponent<Light>();
        sun.type = LightType.Directional; sun.intensity = 0.7f; sun.transform.SetParent(room, false);
        sun.transform.rotation = Quaternion.Euler(50, -30, 0);
        var lamp = new GameObject("PointLight").AddComponent<Light>();
        lamp.type = LightType.Point; lamp.range = 9; lamp.intensity = 1.25f;
        lamp.transform.SetParent(room, false); lamp.transform.localPosition = new Vector3(cf.layoutVariant == 1 ? 1.5f : -1f, 3f, -0.5f);

        BuildVictimFrame(room, cf);

        var slots = new List<Vector3>(L.slots);
        Shuffle(slots, new System.Random(SeedService.StableHash(cf.hash, "slots")));
        for (int i = 0; i < cf.clues.Count; i++)
            SpawnClue(cf.clues[i], parent, slots[i % slots.Count], i / slots.Count);
    }

    static void BuildVictimFrame(Transform room, CaseFile cf)
    {
        var victim = cf.victim;
        Color frameCol = new Color(0.15f, 0.12f, 0.08f);
        Box(room, "Frame", new Vector3(-3.9f, 2.35f, 3.0f), new Vector3(1.35f, 1.75f, 0.09f), frameCol);
        Box(room, "FrameMat", new Vector3(-3.9f, 2.35f, 2.95f), new Vector3(1.15f, 1.55f, 0.05f), new Color(0.9f, 0.88f, 0.82f));

        var sprite = PortraitLibrary.Get(victim);
        if (sprite != null)
        {
            var srGo = new GameObject("VictimPortrait");
            srGo.transform.SetParent(room, false);
            srGo.transform.localPosition = new Vector3(-3.9f, 2.35f, 2.9f);
            var sr = srGo.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            float scale = 1.0f / sprite.bounds.size.x * 1.0f;
            srGo.transform.localScale = Vector3.one * scale;
            sr.sortingOrder = 2;
        }

        var plaque = new GameObject("Plaque");
        plaque.transform.SetParent(room, false);
        plaque.transform.localPosition = new Vector3(-3.9f, 1.32f, 2.92f);

        var label = new GameObject("VictimName");
        label.transform.SetParent(room, false);
        label.transform.localPosition = new Vector3(-3.9f, 2.35f, 2.8f);
        label.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        var tm = label.AddComponent<TextMesh>();
        tm.text = victim != null ? victim.name + "\n" + victim.role : "UNIDENTIFIED";
        tm.characterSize = 0.045f;
        tm.fontSize = 64;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(0.95f, 0.9f, 0.8f);
    }

    static void SpawnClue(ClueSpec s, Transform parent, Vector3 pos, int stack)
    {
        var def = ScriptableObject.CreateInstance<ClueDefinition>();
        def.clueID = s.id; def.displayName = s.name; def.description = s.desc; def.name = s.id;
        def.pointsToName = s.implicatesName; def.kind = s.kind.ToString();

        GameObject g;
        float sz = s.size;
        switch (s.shape)
        {
            case ClueShape.Sphere:
                g = GameObject.CreatePrimitive(PrimitiveType.Sphere); g.transform.localScale = Vector3.one * sz; break;
            case ClueShape.Cylinder:
                g = GameObject.CreatePrimitive(PrimitiveType.Cylinder); g.transform.localScale = new Vector3(sz * 0.6f, sz * 0.5f, sz * 0.6f); break;
            case ClueShape.Capsule:
                g = GameObject.CreatePrimitive(PrimitiveType.Capsule); g.transform.localScale = new Vector3(sz * 0.4f, sz * 0.6f, sz * 0.4f);
                g.transform.rotation = Quaternion.Euler(0, 0, 90); break;
            case ClueShape.Flat:
                g = GameObject.CreatePrimitive(PrimitiveType.Cube); g.transform.localScale = new Vector3(sz, 0.05f, sz * 1.3f);
                if (pos.y < 0.5f) pos.y = 0.04f; break;
            case ClueShape.MarkX:
            case ClueShape.MarkInfinity:
            case ClueShape.MarkCircle:
            case ClueShape.MarkTriangle:
            case ClueShape.MarkClaw:
            case ClueShape.MarkZig:
            case ClueShape.MarkWave:
                g = BuildMark(s.shape, sz, s.color); g.transform.rotation = Quaternion.Euler(40, 0, 0);
                if (pos.y < 0.5f) pos.y = 0.3f; break;
            default:
                g = GameObject.CreatePrimitive(PrimitiveType.Cube); g.transform.localScale = Vector3.one * sz; break;
        }

        pos += new Vector3(stack * 0.12f, 0f, stack * -0.1f);
        g.name = s.name; g.transform.SetParent(parent, false); g.transform.localPosition = pos;
        var r = g.GetComponent<Renderer>(); if (r != null) r.material.color = s.color;
        g.AddComponent<CluePickup>().Setup(def);
    }

    static GameObject BuildMark(ClueShape shape, float size, Color col)
    {
        var root = new GameObject("Mark");
        var box = root.AddComponent<BoxCollider>();
        switch (shape)
        {
            case ClueShape.MarkX:
                Part(root, col, Vector3.zero, new Vector3(size * 1.4f, size * 0.18f, 0.06f), 45);
                Part(root, col, Vector3.zero, new Vector3(size * 1.4f, size * 0.18f, 0.06f), -45);
                box.size = new Vector3(size * 1.1f, size * 1.1f, 0.2f);
                break;
            case ClueShape.MarkInfinity:
            {
                const int N = 40;
                for (int i = 0; i < N; i++)
                {
                    float t = i / (float)N * Mathf.PI * 2f, d = 1f + Mathf.Sin(t) * Mathf.Sin(t);
                    Part(root, col, new Vector3(Mathf.Cos(t) / d * size, Mathf.Sin(t) * Mathf.Cos(t) / d * size, 0),
                         Vector3.one * size * 0.1f, 0);
                }
                box.size = new Vector3(size * 2.1f, size * 0.9f, 0.2f);
                break;
            }
            case ClueShape.MarkCircle:
            {
                const int N = 26;
                for (int i = 0; i < N; i++)
                {
                    float t = i / (float)N * Mathf.PI * 2f;
                    Part(root, col, new Vector3(Mathf.Cos(t) * size, Mathf.Sin(t) * size, 0), Vector3.one * size * 0.14f, 0);
                }
                box.size = new Vector3(size * 2.3f, size * 2.3f, 0.2f);
                break;
            }
            case ClueShape.MarkTriangle:
                Part(root, col, new Vector3(0, -size * 0.7f, 0), new Vector3(size * 1.9f, size * 0.16f, 0.06f), 0);
                Part(root, col, new Vector3(-size * 0.55f, size * 0.1f, 0), new Vector3(size * 1.7f, size * 0.16f, 0.06f), 60);
                Part(root, col, new Vector3(size * 0.55f, size * 0.1f, 0), new Vector3(size * 1.7f, size * 0.16f, 0.06f), -60);
                box.size = new Vector3(size * 1.8f, size * 1.7f, 0.2f);
                break;
            case ClueShape.MarkClaw:
                for (int i = -1; i <= 1; i++)
                    Part(root, col, new Vector3(i * size * 0.5f, 0, 0), new Vector3(size * 1.5f, size * 0.14f, 0.06f), 65);
                box.size = new Vector3(size * 1.8f, size * 1.6f, 0.2f);
                break;
            case ClueShape.MarkZig:
                Part(root, col, new Vector3(-size * 0.25f, size * 0.45f, 0), new Vector3(size * 0.9f, size * 0.16f, 0.06f), 62);
                Part(root, col, new Vector3(size * 0.05f, 0f, 0), new Vector3(size * 0.9f, size * 0.16f, 0.06f), -62);
                Part(root, col, new Vector3(size * 0.25f, -size * 0.45f, 0), new Vector3(size * 0.9f, size * 0.16f, 0.06f), 62);
                box.size = new Vector3(size * 1.2f, size * 1.5f, 0.2f);
                break;
            case ClueShape.MarkWave:
            {
                const int N = 18;
                for (int i = 0; i < N; i++)
                {
                    float t = i / (float)(N - 1);
                    float x = Mathf.Lerp(-size, size, t);
                    float y = Mathf.Sin(t * Mathf.PI * 2f) * size * 0.5f;
                    Part(root, col, new Vector3(x, y, 0), Vector3.one * size * 0.16f, 0);
                }
                box.size = new Vector3(size * 2.2f, size * 1.2f, 0.2f);
                break;
            }
        }
        return root;
    }

    static void Part(GameObject root, Color col, Vector3 lp, Vector3 ls, float zRot)
    {
        var p = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p.transform.SetParent(root.transform, false);
        p.transform.localPosition = lp; p.transform.localScale = ls; p.transform.localRotation = Quaternion.Euler(0, 0, zRot);
        p.GetComponent<Renderer>().material.color = col;
        Object.Destroy(p.GetComponent<Collider>());
    }

    static void Shuffle<T>(List<T> list, System.Random rnd)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            var tmp = list[i]; list[i] = list[j]; list[j] = tmp;
        }
    }
}
