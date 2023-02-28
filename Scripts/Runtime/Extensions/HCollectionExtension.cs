using System.Collections.Generic;

public static class HCollectionExtension
{
    public static T GetRandom<T>(this List<T> list,T defaultValue = default)
    {
        if (list.Count == 0) return defaultValue;
        int selected = UnityEngine.Random.Range(0, list.Count);
        return list[selected];
    }
}