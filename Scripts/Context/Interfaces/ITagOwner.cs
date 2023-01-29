using System;

public interface ITagOwner : ICastable
{
    event Action<ITagOwner,string> onTagAdded;
    event Action<ITagOwner,string> onTagRemoved;
    event Action<ITagOwner,string,bool> onTagChanged; 
    bool ContainsTag(string t);
    void AddTag(string t);
    void RemoveTag(string t);
}