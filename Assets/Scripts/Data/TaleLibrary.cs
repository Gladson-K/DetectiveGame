using System.Collections.Generic;
using UnityEngine;

public static class TaleLibrary
{
    static List<TaleDef> tales;
    static bool loaded;

    public static List<TaleDef> All
    {
        get { Load(); return tales; }
    }

    public static TaleDef Find(string id) => All.Find(t => t.id == id);

    public static void Reload() { tales = null; loaded = false; }

    static void Load()
    {
        if (loaded && tales != null) return;
        tales = new List<TaleDef>();
        loaded = true;

        var assets = Resources.LoadAll<TextAsset>("Tales");
        var parsed = new List<TaleDef>();
        foreach (var ta in assets)
        {
            TaleJson json = null;
            try { json = JsonUtility.FromJson<TaleJson>(ta.text); }
            catch (System.Exception e)
            {
                Debug.LogError($"[TaleLibrary] Failed to parse '{ta.name}': {e.Message}");
                continue;
            }
            if (json == null || string.IsNullOrEmpty(json.id))
            {
                Debug.LogError($"[TaleLibrary] '{ta.name}' has no 'id' field. Skipped.");
                continue;
            }
            if (parsed.Exists(t => t.id == json.id))
            {
                Debug.LogError($"[TaleLibrary] Duplicate tale id '{json.id}' in '{ta.name}'. Skipped.");
                continue;
            }
            var tale = TaleParser.FromJson(json);
            Validate(tale, ta.name);
            parsed.Add(tale);
        }

        parsed.Sort((a, b) => a.order.CompareTo(b.order));
        tales = parsed;

        if (tales.Count == 0)
            Debug.LogError("[TaleLibrary] No tales found. Add JSON files under Assets/Resources/Tales/ (see Docs/V2_AUTHORING_GUIDE.md).");
    }

    static void Validate(TaleDef t, string source)
    {
        if (t.arcs.Count == 0) Debug.LogError($"[TaleLibrary] Tale '{t.id}' ({source}) has no arcs.");
        if (t.characters.Count == 0) Debug.LogError($"[TaleLibrary] Tale '{t.id}' ({source}) has no characters.");
        if (t.clues.Count == 0) Debug.LogError($"[TaleLibrary] Tale '{t.id}' ({source}) has no clues.");

        foreach (var a in t.arcs)
            if (a.cities.Count == 0)
                Debug.LogError($"[TaleLibrary] Tale '{t.id}' arc '{a.id}' has no cities.");

        foreach (var c in t.characters)
        {
            if (string.IsNullOrEmpty(c.id) || string.IsNullOrEmpty(c.name))
                Debug.LogError($"[TaleLibrary] Tale '{t.id}' has a character with missing id or name.");
            if (t.characters.FindAll(x => x.id == c.id).Count > 1)
                Debug.LogError($"[TaleLibrary] Tale '{t.id}' has duplicate character id '{c.id}'.");
        }

        foreach (var cl in t.clues)
        {
            if (string.IsNullOrEmpty(cl.id) || string.IsNullOrEmpty(cl.name))
                Debug.LogError($"[TaleLibrary] Tale '{t.id}' has a clue with missing id or name.");
            if (!string.IsNullOrEmpty(cl.implicatesId) && t.FindCharacter(cl.implicatesId) == null)
                Debug.LogError($"[TaleLibrary] Tale '{t.id}' clue '{cl.id}' implicates unknown character '{cl.implicatesId}'.");
        }
    }
}
