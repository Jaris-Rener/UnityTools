namespace AudioCollections
{
    using System.Collections;
    using UnityEngine;

    public class AudioPlayer
        : MonoBehaviour
    {
        public CollectionClip Clip;
        public AudioSource Source;

        public float Delay = 1;

        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Delay);
                Source.PlayClip(Clip, 1);
            }
        }
    }

}