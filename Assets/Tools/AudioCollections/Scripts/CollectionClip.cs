namespace AudioCollections
{
    using System;
    using UnityEngine;

#pragma warning disable 649
    [Serializable]
    public class CollectionClip
    {
        public AudioClip Clip => _useCollection
            ? _collection.Collection[_selected].Clip
            : _singleClip;

        [SerializeField] private AudioCollection _collection;
        [SerializeField] private AudioClip _singleClip;
        [SerializeField] private bool _useCollection;
        [SerializeField] private int _selected;
    }

}