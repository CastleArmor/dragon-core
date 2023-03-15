using System.Collections.Generic;
using UnityEngine;

public static class TagRegistry
{
    private static readonly Dictionary<string, Dictionary<int, string>> _tagDictionary =
        new Dictionary<string, Dictionary<int, string>>();

    public static void Register(TagHolder tagHolder)
    {
        if (!_tagDictionary.TryGetValue(tagHolder.Category, out Dictionary<int, string> categoryDictionary))
        {
            categoryDictionary = new Dictionary<int, string>();
            _tagDictionary.Add(tagHolder.Category, categoryDictionary);
        }
        
        categoryDictionary[tagHolder.gameObject.GetInstanceID()] = tagHolder.Tag;
    }

    public static void Unregister(TagHolder tagHolder)
    {
        if (_tagDictionary.TryGetValue(tagHolder.Category, out Dictionary<int, string> categoryDictionary))
        {
            categoryDictionary.Remove(tagHolder.gameObject.GetInstanceID());
        }
    }

    public static string GetCategoryTag(string category, GameObject gameObject)
    {
        if (_tagDictionary.TryGetValue(category, out Dictionary<int, string> categoryDictionary))
        {
            if (categoryDictionary.TryGetValue(gameObject.GetInstanceID(), out string tag))
            {
                return tag;
            }
        }

        return "";
    }
}