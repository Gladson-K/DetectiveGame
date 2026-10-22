using System;
using System.Collections.Generic;
using UnityEngine;

public enum ClueKind { Guilty, Herring, Flavor }

public enum ClueShape
{
    Cube, Sphere, Cylinder, Capsule, Flat,
    MarkX, MarkInfinity, MarkCircle, MarkTriangle, MarkClaw, MarkZig, MarkWave
}

public class ClueSpec
{
    public string id, name, desc;
    public ClueShape shape = ClueShape.Cube;
    public Color color = Color.white;
    public float size = 0.35f;
    public ClueKind kind = ClueKind.Guilty;
    public string implicatesId = "";
    public string implicatesName = "";
    public List<string> arcs = new List<string>();

    public bool InArc(string arcId) => arcs == null || arcs.Count == 0 || arcs.Contains(arcId);
}

    public enum CharacterViewMode { Auto, Sprite2D, Model3D }

    public class CharacterDef
    {
        public string id, name, role, city, bio, colorHex, portraitId;
        public int age;
        public bool canBeVictim = true, canBeSuspect = true, canBeCulprit;
        public string alibi = "";
        public List<string> statements = new List<string>();
        public CharacterViewMode viewMode = CharacterViewMode.Auto;
        public string modelPrefab;
        public string modelResourcePath;
        public float modelScale = 1f;
        public Vector3 modelOffset = Vector3.zero;
        public float modelRotationY = 0f;
        public bool billboard3D = true;

        public Color Accent
        {
            get
            {
                Color c;
                if (!string.IsNullOrEmpty(colorHex) && ColorUtility.TryParseHtmlString(colorHex, out c)) return c;
                float h = (Mathf.Abs(GetHashCode()) % 360) / 360f;
                return Color.HSVToRGB(h, 0.45f, 0.85f);
            }
        }

        public string PortraitKey => string.IsNullOrEmpty(portraitId) ? id : portraitId;
        public string LastName
        {
            get
            {
                var parts = name.Split(' ');
                return parts.Length > 1 ? parts[parts.Length - 1] : name;
            }
        }
    }

public class ArcDef
{
    public string id, title, menaceName, markName, markShapeName, markColorHex, accentHex;
    public List<string> cities = new List<string>();
    public ClueShape markShape = ClueShape.MarkX;

    public Color MarkColor
    {
        get
        {
            Color c;
            if (!string.IsNullOrEmpty(markColorHex) && ColorUtility.TryParseHtmlString(markColorHex, out c)) return c;
            return Color.red;
        }
    }

    public Color Accent
    {
        get
        {
            Color c;
            if (!string.IsNullOrEmpty(accentHex) && ColorUtility.TryParseHtmlString(accentHex, out c)) return c;
            return new Color(0.3f, 0.3f, 0.35f);
        }
    }
}

public class TaleDef
{
    public string id, title, subtitle, description;
    public int order = 100, casesPerArc = 3;
    public List<ArcDef> arcs = new List<ArcDef>();
    public List<CharacterDef> characters = new List<CharacterDef>();
    public List<ClueSpec> clues = new List<ClueSpec>();
    public List<string> titleTemplates = new List<string>();
    public List<string> situationTemplates = new List<string>();
    public List<string> briefingTemplates = new List<string>();

    public CharacterDef FindCharacter(string cid)
    {
        if (string.IsNullOrEmpty(cid)) return null;
        return characters.Find(c => c.id == cid);
    }

    public ArcDef FindArc(string aid) => arcs.Find(a => a.id == aid);
}

public class CaseFile
{
    public string id, hash, displayCode, title, city, situation, briefing;
    public string taleId, arcId;
    public int slotIndex;
    public CharacterDef victim, culprit;
    public List<CharacterDef> suspects = new List<CharacterDef>();
    public List<ClueSpec> clues = new List<ClueSpec>();
    public int layoutVariant;
    public int mistakes;

    public string MarkName { get; set; } = "";
    public Color MarkColor { get; set; } = Color.red;
    public ClueShape MarkShape { get; set; } = ClueShape.MarkX;
}

[Serializable]
public class TaleJson
{
    public string id;
    public string title;
    public string subtitle;
    public string description;
    public int order = 100;
    public int casesPerArc = 3;
    public List<ArcJson> arcs = new List<ArcJson>();
    public List<CharacterJson> characters = new List<CharacterJson>();
    public List<ClueJson> clues = new List<ClueJson>();
    public List<string> titleTemplates = new List<string>();
    public List<string> situationTemplates = new List<string>();
    public List<string> briefingTemplates = new List<string>();
}

[Serializable]
public class ArcJson
{
    public string id;
    public string title;
    public string menaceName;
    public string markName;
    public string markShape;
    public string markColorHex;
    public string accentHex;
    public List<string> cities = new List<string>();
}

[Serializable]
public class CharacterJson
{
    public string id;
    public string name;
    public string role;
    public string city;
    public int age;
    public string bio;
    public string colorHex;
    public string portraitId;
    public bool canBeVictim = true;
    public bool canBeSuspect = true;
    public bool canBeCulprit;
    public string alibi;
    public List<string> statements = new List<string>();
    public string viewMode;
    public string modelPrefab, modelResourcePath;
    public float modelScale = 1f;
    public float[] modelOffset;
    public float modelRotationY = 0f;
    public bool billboard3D = true;
}

[Serializable]
public class ClueJson
{
    public string id;
    public string name;
    public string desc;
    public string shape;
    public string colorHex;
    public float size = 0.35f;
    public string kind;
    public string implicates;
    public List<string> arcs = new List<string>();
}

public static class TaleParser
{
    public static TaleDef FromJson(TaleJson j)
    {
        var tale = new TaleDef
        {
            id = j.id, title = j.title, subtitle = j.subtitle, description = j.description,
            order = j.order, casesPerArc = Mathf.Max(1, j.casesPerArc),
            titleTemplates = j.titleTemplates ?? new List<string>(),
            situationTemplates = j.situationTemplates ?? new List<string>(),
            briefingTemplates = j.briefingTemplates ?? new List<string>()
        };

        foreach (var a in j.arcs)
        {
            var arc = new ArcDef
            {
                id = a.id, title = a.title, menaceName = a.menaceName, markName = a.markName,
                markShapeName = a.markShape, markColorHex = a.markColorHex, accentHex = a.accentHex,
                cities = a.cities ?? new List<string>()
            };
            arc.markShape = ParseShape(a.markShape, ClueShape.MarkX);
            tale.arcs.Add(arc);
        }

        foreach (var c in j.characters)
        {
            var ch = new CharacterDef
            {
                id = c.id, name = c.name, role = c.role, city = c.city, age = c.age,
                bio = c.bio, colorHex = c.colorHex, portraitId = c.portraitId,
                canBeVictim = c.canBeVictim, canBeSuspect = c.canBeSuspect, canBeCulprit = c.canBeCulprit,
                alibi = c.alibi, statements = c.statements ?? new List<string>(),
                modelPrefab = c.modelPrefab, modelResourcePath = c.modelResourcePath,
                modelScale = c.modelScale <= 0 ? 1f : c.modelScale, modelRotationY = c.modelRotationY,
                billboard3D = c.billboard3D
            };
            if (!string.IsNullOrEmpty(c.viewMode))
                Enum.TryParse(c.viewMode, true, out ch.viewMode);
            if (c.modelOffset != null && c.modelOffset.Length == 3)
                ch.modelOffset = new Vector3(c.modelOffset[0], c.modelOffset[1], c.modelOffset[2]);
            tale.characters.Add(ch);
        }

        foreach (var cl in j.clues)
        {
            tale.clues.Add(new ClueSpec
            {
                id = cl.id, name = cl.name, desc = cl.desc,
                shape = ParseShape(cl.shape, ClueShape.Cube),
                color = ParseColor(cl.colorHex, Color.white),
                size = cl.size <= 0 ? 0.35f : cl.size,
                kind = ParseKind(cl.kind),
                implicatesId = cl.implicates ?? "",
                arcs = cl.arcs ?? new List<string>()
            });
        }

        return tale;
    }

    public static ClueShape ParseShape(string s, ClueShape fallback)
    {
        ClueShape shape;
        if (!string.IsNullOrEmpty(s) && Enum.TryParse(s, true, out shape)) return shape;
        return fallback;
    }

    public static ClueKind ParseKind(string s)
    {
        ClueKind k;
        if (!string.IsNullOrEmpty(s) && Enum.TryParse(s, true, out k)) return k;
        return ClueKind.Guilty;
    }

    public static Color ParseColor(string hex, Color fallback)
    {
        Color c;
        if (!string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out c)) return c;
        return fallback;
    }
}
