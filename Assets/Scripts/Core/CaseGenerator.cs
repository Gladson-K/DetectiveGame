using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class CaseGenerator
{
    static readonly Dictionary<string, List<CaseFile>> arcCache = new Dictionary<string, List<CaseFile>>();

    public static void ClearCache() => arcCache.Clear();

    public static List<CaseFile> GetArcCases(TaleDef tale, ArcDef arc)
    {
        string key = tale.id + "|" + arc.id + "|" + SeedService.UserSeed + "|" + SeedService.GetRoll(tale.id, arc.id);
        List<CaseFile> cached;
        if (arcCache.TryGetValue(key, out cached)) return cached;
        var cases = GenerateArc(tale, arc);
        arcCache[key] = cases;
        return cases;
    }

    public static CaseFile GetCase(TaleDef tale, ArcDef arc, int slot)
    {
        var cases = GetArcCases(tale, arc);
        if (slot < 0 || slot >= cases.Count) return null;
        return cases[slot];
    }

    public static int SolvedCount(TaleDef tale, ArcDef arc)
    {
        int n = 0;
        foreach (var c in GetArcCases(tale, arc))
            if (SeedService.IsSolved(SeedService.CaseKey(tale.id, arc.id, c.slotIndex, c.hash))) n++;
        return n;
    }

    public static bool AllSolved(TaleDef tale, ArcDef arc) => SolvedCount(tale, arc) >= GetArcCases(tale, arc).Count;

    static List<CaseFile> GenerateArc(TaleDef tale, ArcDef arc)
    {
        int roll = SeedService.GetRoll(tale.id, arc.id);
        var rnd = new System.Random(SeedService.StableHash(SeedService.UserSeed, tale.id, arc.id, roll));

        var victims = Filter(tale.characters, c => c.canBeVictim);
        var culprits = Filter(tale.characters, c => c.canBeCulprit && c.canBeSuspect);
        var suspectPool = Filter(tale.characters, c => c.canBeSuspect);
        Shuffle(victims, rnd);
        Shuffle(culprits, rnd);

        int n = Mathf.Max(1, tale.casesPerArc);
        var usedVictims = new HashSet<string>();
        var usedCulprits = new HashSet<string>();
        var result = new List<CaseFile>();

        for (int slot = 0; slot < n; slot++)
        {
            string city = arc.cities.Count > 0 ? arc.cities[rnd.Next(arc.cities.Count)] : "Unknown";

            var victim = Pick(victims, usedVictims, c => arc.cities.Count == 0 || arc.cities.Contains(c.city) || string.IsNullOrEmpty(c.city), rnd);
            if (victim == null) victim = Pick(victims, usedVictims, null, rnd);

            var culprit = Pick(culprits, usedCulprits, c => victim == null || c.id != victim.id, rnd);
            if (culprit == null) culprit = Pick(culprits, new HashSet<string>(), c => victim == null || c.id != victim.id, rnd);

            int want = 3 + rnd.Next(3);
            var suspects = new List<CharacterDef>();
            if (culprit != null) suspects.Add(culprit);
            var others = Filter(suspectPool, c => (victim == null || c.id != victim.id) && (culprit == null || c.id != culprit.id));
            Shuffle(others, rnd);
            foreach (var c in others)
            {
                if (suspects.Count >= want) break;
                suspects.Add(c);
            }
            Shuffle(suspects, rnd);

            var cf = BuildCase(tale, arc, slot, rnd, victim != null && !string.IsNullOrEmpty(victim.city) ? victim.city : city, victim, culprit, suspects);
            result.Add(cf);
            if (victim != null) usedVictims.Add(victim.id);
            if (culprit != null) usedCulprits.Add(culprit.id);
        }
        return result;
    }

    static CaseFile BuildCase(TaleDef tale, ArcDef arc, int slot, System.Random rnd, string city,
                              CharacterDef victim, CharacterDef culprit, List<CharacterDef> suspects)
    {
        var cf = new CaseFile
        {
            taleId = tale.id, arcId = arc.id, slotIndex = slot, city = city,
            victim = victim, culprit = culprit, suspects = suspects,
            layoutVariant = rnd.Next(3),
            MarkName = arc.markName, MarkColor = arc.MarkColor, MarkShape = arc.markShape
        };

        var sb = new StringBuilder();
        sb.Append(victim != null ? victim.id : "x");
        sb.Append('|').Append(culprit != null ? culprit.id : "x");
        sb.Append('|').Append(city);
        cf.clues = PickClues(tale, arc, rnd, cf);
        foreach (var cl in cf.clues) sb.Append('|').Append(cl.id);
        cf.hash = (SeedService.StableHash(sb.ToString()) & 0xFFFFFFF).ToString("X6");
        cf.displayCode = "#" + cf.hash.Substring(cf.hash.Length - 4);
        cf.id = tale.id + "_" + arc.id + "_" + slot;

        string victimName = victim != null ? victim.name : "the victim";
        cf.title = Fill(rnd, tale.titleTemplates, victim, city, arc, culprit, suspects, "Unknown");
        cf.situation = Fill(rnd, tale.situationTemplates, victim, city, arc, culprit, suspects, victimName);
        cf.briefing = Fill(rnd, tale.briefingTemplates, victim, city, arc, culprit, suspects, victimName);
        return cf;
    }

    static List<ClueSpec> PickClues(TaleDef tale, ArcDef arc, System.Random rnd, CaseFile cf)
    {
        var picked = new List<ClueSpec>();

        var mark = new ClueSpec
        {
            id = cf.id + "_mark",
            name = "The " + arc.markName + " Mark",
            desc = "The " + arc.markName + " — the killer's signature, left for anyone brave enough to find it.",
            shape = arc.markShape, color = arc.MarkColor, size = 0.55f, kind = ClueKind.Guilty
        };
        var markByShape = tale.clues.Find(c => c.shape == arc.markShape);
        if (markByShape != null) mark.desc = markByShape.desc;
        picked.Add(mark);

        var guiltyPool = Filter(tale.clues, c => c.kind == ClueKind.Guilty && c.InArc(arc.id) && c.shape != arc.markShape
                                                 && (string.IsNullOrEmpty(c.implicatesId) || (cf.culprit != null && c.implicatesId == cf.culprit.id)));
        var herringPool = Filter(tale.clues, c => c.kind == ClueKind.Herring && c.InArc(arc.id)
                                                 && !string.IsNullOrEmpty(c.implicatesId) && cf.suspects.Exists(s => s.id == c.implicatesId)
                                                 && (cf.culprit == null || c.implicatesId != cf.culprit.id));
        var flavorPool = Filter(tale.clues, c => c.kind == ClueKind.Flavor && c.InArc(arc.id));
        Shuffle(guiltyPool, rnd); Shuffle(herringPool, rnd); Shuffle(flavorPool, rnd);

        int wantGuilty = 3 + rnd.Next(2);
        int wantHerring = 2;
        int wantFlavor = 1 + rnd.Next(2);

        foreach (var g in Take(guiltyPool, wantGuilty))
        {
            var clone = Clone(g, cf);
            if (cf.culprit != null) clone.implicatesId = cf.culprit.id;
            picked.Add(clone);
        }
        foreach (var h in Take(herringPool, wantHerring))
            picked.Add(Clone(h, cf));
        foreach (var f in Take(flavorPool, wantFlavor))
            picked.Add(Clone(f, cf));

        int safety = 0;
        while (CountKind(picked, ClueKind.Guilty) < 3 && safety++ < 10)
        {
            var extra = Clone(tale.clues.Find(c => c.kind == ClueKind.Guilty && c.InArc(arc.id) && c.shape != arc.markShape), cf);
            if (extra == null) break;
            if (cf.culprit != null) extra.implicatesId = cf.culprit.id;
            picked.Add(extra);
        }
        safety = 0;
        while (CountKind(picked, ClueKind.Herring) < 2 && safety++ < 10)
        {
            var innocent = cf.suspects.Find(s => cf.culprit == null || s.id != cf.culprit.id);
            var extra = Clone(tale.clues.Find(c => c.kind == ClueKind.Herring && c.InArc(arc.id)), cf);
            if (extra == null || innocent == null) break;
            extra.implicatesId = innocent.id;
            picked.Add(extra);
        }

        foreach (var cl in picked)
        {
            var target = cl.implicatesId == null ? null : cf.suspects.Find(s => s.id == cl.implicatesId);
            cl.implicatesName = target != null ? target.name : "";
        }

        var seen = new HashSet<string>();
        foreach (var cl in picked)
        {
            string baseId = cl.id;
            int suffix = 1;
            while (!seen.Add(cl.id))
            {
                cl.id = baseId + "_" + suffix;
                suffix++;
            }
        }
        return picked;
    }

    static ClueSpec Clone(ClueSpec src, CaseFile cf)
    {
        if (src == null) return null;
        return new ClueSpec
        {
            id = cf.id + "_" + src.id,
            name = src.name, desc = src.desc, shape = src.shape, color = src.color, size = src.size,
            kind = src.kind, implicatesId = src.implicatesId, implicatesName = src.implicatesName
        };
    }

    static int CountKind(List<ClueSpec> list, ClueKind k)
    {
        int n = 0;
        foreach (var c in list) if (c.kind == k) n++;
        return n;
    }

    static string Fill(System.Random rnd, List<string> templates, CharacterDef victim, string city,
                       ArcDef arc, CharacterDef culprit, List<CharacterDef> suspects, string fallback)
    {
        if (templates == null || templates.Count == 0) return fallback;
        string t = templates[rnd.Next(templates.Count)];
        string suspectName = suspects.Count > 0 ? suspects[rnd.Next(suspects.Count)].name : "a witness";
        return t
            .Replace("{VICTIM}", victim != null ? victim.name : "the victim")
            .Replace("{VICTIM_LAST}", victim != null ? victim.LastName : "the victim")
            .Replace("{VICTIM_ROLE}", victim != null ? victim.role : "unknown")
            .Replace("{CITY}", city)
            .Replace("{MENACE}", arc.menaceName)
            .Replace("{MARK}", arc.markName)
            .Replace("{ARC}", arc.title)
            .Replace("{CULPRIT}", culprit != null ? culprit.name : "the killer")
            .Replace("{CULPRIT_LAST}", culprit != null ? culprit.LastName : "the killer")
            .Replace("{SUSPECT}", suspectName);
    }

    static List<T> Filter<T>(List<T> src, System.Predicate<T> pred)
    {
        var outp = new List<T>();
        foreach (var x in src) if (pred(x)) outp.Add(x);
        return outp;
    }

    static List<T> Take<T>(List<T> src, int n)
    {
        var outp = new List<T>();
        for (int i = 0; i < n && i < src.Count; i++) outp.Add(src[i]);
        return outp;
    }

    static T Pick<T>(List<T> pool, HashSet<string> used, System.Predicate<T> extra, System.Random rnd) where T : CharacterDef
    {
        foreach (var c in pool)
        {
            if (used.Contains(c.id)) continue;
            if (extra != null && !extra(c)) continue;
            return c;
        }
        return null;
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
