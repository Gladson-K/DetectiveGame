#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ContentValidator
{
    [MenuItem("Detective Game/Validate Tale Content")]
    static void Validate()
    {
        TaleLibrary.Reload();
        var tales = TaleLibrary.All;
        var report = new StringBuilder();
        report.AppendLine($"Validating {tales.Count} tale(s)...\n");

        int errors = 0;
        foreach (var tale in tales)
        {
            report.AppendLine($"TALE: {tale.title} ({tale.id})  -  {tale.arcs.Count} arcs, {tale.characters.Count} characters, {tale.clues.Count} clues");

            foreach (var arc in tale.arcs)
            {
                int victims = 0, suspects = 0, culprits = 0;
                foreach (var c in tale.characters)
                {
                    if (c.canBeVictim) victims++;
                    if (c.canBeSuspect) suspects++;
                    if (c.canBeCulprit && c.canBeSuspect) culprits++;
                }
                int need = Mathf.Max(1, tale.casesPerArc);
                if (victims < 1) { report.AppendLine($"  ERROR arc '{arc.id}': no characters can be victims."); errors++; }
                if (suspects < 3) { report.AppendLine($"  ERROR arc '{arc.id}': need at least 3 possible suspects."); errors++; }
                if (culprits < 1) { report.AppendLine($"  ERROR arc '{arc.id}': no characters can be culprits."); errors++; }
                if (culprits < Mathf.Min(need, 3)) report.AppendLine($"  WARN  arc '{arc.id}': only {culprits} culprit-capable characters for {need} cases.");

                int guilty = 0, herring = 0, flavor = 0;
                foreach (var cl in tale.clues)
                {
                    if (!cl.InArc(arc.id)) continue;
                    if (cl.kind == ClueKind.Guilty) guilty++;
                    else if (cl.kind == ClueKind.Herring) herring++;
                    else flavor++;
                }
                if (guilty < 4) { report.AppendLine($"  ERROR arc '{arc.id}': needs 4+ 'Guilty' clues, found {guilty}."); errors++; }
                if (herring < 2) { report.AppendLine($"  WARN  arc '{arc.id}': only {herring} 'Herring' clues."); }
                if (flavor < 1) { report.AppendLine($"  WARN  arc '{arc.id}': no 'Flavor' clues."); }
                if (tale.clues.FindIndex(c => c.shape == arc.markShape) < 0)
                    report.AppendLine($"  WARN  arc '{arc.id}': no clue uses the mark shape '{arc.markShape}' (a default mark description will be used).");

                report.AppendLine($"    arc ok: {victims} victims / {suspects} suspects / {culprits} culprits - clues G{guilty} H{herring} F{flavor}");
            }

            var missing = new List<string>();
            foreach (var c in tale.characters)
            {
                if (Resources.Load<Sprite>("Portraits/" + c.PortraitKey) == null) missing.Add(c.PortraitKey);
                string pp = c.modelResourcePath ?? c.modelPrefab;
                if (!string.IsNullOrEmpty(pp) && Resources.Load<GameObject>(pp) == null)
                    report.AppendLine($"  WARN: model prefab not found in Resources/{pp}.prefab for character {c.id}");
            }
            if (missing.Count > 0)
                report.AppendLine($"  NOTE: {missing.Count} portrait(s) not found in Assets/Resources/Portraits/ - placeholders will be used:\n    {string.Join(", ", missing)}");

            report.AppendLine();
        }

        report.AppendLine(errors == 0 ? "Validation finished with no errors." : $"Validation finished with {errors} error(s).");
        Debug.Log(report.ToString());
    }
}
#endif
