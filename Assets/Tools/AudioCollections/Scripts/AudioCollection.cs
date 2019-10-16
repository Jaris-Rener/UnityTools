using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "New Audio Collection", fileName = "NewAudioCollection")]
[Serializable]
public class AudioCollection
    : ScriptableObject
{
    [SerializeField] public List<AudioBundle> Collection = new List<AudioBundle>();

    public bool TryGetValue(string key, out AudioClip value)
    {
        value = null;
        var bundle = Collection.FirstOrDefault(x => x.Key == key);
        if (bundle == null)
            return false;
        
        value = bundle.Clip;
        return true;
    }
}

[Serializable]
public class AudioBundle
{
    public string Key;
    public AudioClip Clip;
}