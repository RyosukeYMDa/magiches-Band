using UnityEngine;
using System.Collections.Generic;

namespace TechC.MagichesBand.Core
{
    public class Sound : SingletonMonoBehaviour<Sound>
    {
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private SoundDatabase database;

        private Dictionary<SoundType, AudioClip> clipDict;

        private void Start()
        {
            clipDict = new Dictionary<SoundType, AudioClip>();

            foreach (var entry in database.entries)
            {
                if (entry.clip != null && !clipDict.ContainsKey(entry.type))
                {
                    clipDict.Add(entry.type, entry.clip);
                }
            }
        }

        public void Play(SoundType type, bool loop = false)
        {
            if (!clipDict.TryGetValue(type, out var clip) || clip == null)
            {
                Debug.LogWarning($"AudioClip not found for {type}");
                return;
            }

            if (loop)
            {
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.Play();
            }
            else
            {
                seSource.PlayOneShot(clip);
            }
        }
    }
}
