using UnityEngine;

public static class SeedService
{
    const string KeySeed = "user_seed";
    const string KeyRollPrefix = "roll_";
    const string KeySolvedPrefix = "solved_";
    const string KeyArrestPrefix = "arrest_";

    static int? cachedSeed;

    public static int UserSeed
    {
        get
        {
            if (cachedSeed.HasValue) return cachedSeed.Value;
            if (PlayerPrefs.HasKey(KeySeed)) cachedSeed = PlayerPrefs.GetInt(KeySeed);
            else
            {
                cachedSeed = NewSeed();
                PlayerPrefs.SetInt(KeySeed, cachedSeed.Value);
                PlayerPrefs.Save();
            }
            return cachedSeed.Value;
        }
    }

    public static string IdentityCode => (UserSeed & 0x7FFFFFFF).ToString("X6");

    public static int NewSeed()
    {
        int g = System.Guid.NewGuid().GetHashCode();
        return unchecked(g ^ System.Environment.TickCount ^ Random.Range(int.MinValue, int.MaxValue));
    }

    public static void ResetIdentity()
    {
        var keys = new System.Collections.Generic.List<string>();
        for (int i = 0; i < PlayerPrefs.keys.Length; i++)
        {
            var k = PlayerPrefs.keys[i];
            if (k == KeySeed || k.StartsWith(KeyRollPrefix) || k.StartsWith(KeySolvedPrefix) || k.StartsWith(KeyArrestPrefix))
                keys.Add(k);
        }
        foreach (var k in keys) PlayerPrefs.DeleteKey(k);
        cachedSeed = null;
        PlayerPrefs.SetInt(KeySeed, NewSeed());
        PlayerPrefs.Save();
    }

    public static int GetRoll(string taleId, string arcId) => PlayerPrefs.GetInt(KeyRollPrefix + taleId + "_" + arcId, 0);

    public static void Reshuffle(string taleId, string arcId)
    {
        PlayerPrefs.SetInt(KeyRollPrefix + taleId + "_" + arcId, GetRoll(taleId, arcId) + 1);
        PlayerPrefs.Save();
    }

    public static bool IsSolved(string caseKey) => PlayerPrefs.GetInt(KeySolvedPrefix + caseKey, 0) == 1;

    public static void MarkSolved(string caseKey)
    {
        PlayerPrefs.SetInt(KeySolvedPrefix + caseKey, 1);
        PlayerPrefs.Save();
    }

    public static bool IsArrested(string taleId, string arcId) => PlayerPrefs.GetInt(KeyArrestPrefix + taleId + "_" + arcId, 0) == 1;

    public static void MarkArrested(string taleId, string arcId)
    {
        PlayerPrefs.SetInt(KeyArrestPrefix + taleId + "_" + arcId, 1);
        PlayerPrefs.Save();
    }

    public static string CaseKey(string taleId, string arcId, int slot, string caseHash)
        => taleId + "_" + arcId + "_" + slot + "_" + caseHash;

    public static int StableHash(params object[] parts)
    {
        unchecked
        {
            int h = 17;
            foreach (var p in parts)
                h = h * 31 + (p == null ? 0 : p.GetHashCode());
            return h;
        }
    }
}
