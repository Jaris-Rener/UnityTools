namespace AudioCollections
{
    using UnityEngine;

    public static class AudioCollectionUtility
    {
        public static void PlayClip(this AudioSource source, CollectionClip clip, float volume = 1)
        {
            source.PlayOneShot(clip.Clip, volume);
        }
    }

}