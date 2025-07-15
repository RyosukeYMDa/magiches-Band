using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TechC.MagichesBand.Core
{
    public class Sound : SingletonMonoBehaviour<Sound>
    {
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private SoundDatabase database;

        private Dictionary<SoundType, AudioClip> clipDict;
        
        protected override void Awake()
        {
            base.Awake(); // SingletonMonoBehaviour で Instance を設定

            clipDict = new Dictionary<SoundType, AudioClip>();

            if (!database)
            {
                Debug.LogError("SoundDatabase が設定されていません！");
                return;
            }

            foreach (var entry in database.entries.Where(entry => entry.clip && !clipDict.ContainsKey(entry.type)))
            {
                clipDict.Add(entry.type, entry.clip);
            }
        }

        public void Play(SoundType type, bool loop = false)
        {
            if (!clipDict.TryGetValue(type, out var clip) || !clip)
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
