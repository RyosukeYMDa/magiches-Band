using UnityEngine;
using System.Collections.Generic;

namespace TechC.MagichesBand.Core
{
    public class Sound : SingletonMonoBehaviour<Sound>
    {
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioSource bgmSource;

        [SerializeField] private AudioClip atkUpClip;

        [SerializeField] private AudioClip slimeBgmClip;
        
        private Dictionary<SoundType, AudioClip> clipDict;

        private void Start()
        {
            clipDict = new Dictionary<SoundType, AudioClip>
            {
                { SoundType.AtkUp, atkUpClip },
                { SoundType.SlimeBGM, slimeBgmClip}
            };
        }

        /// <summary>
        /// 汎用再生関数：ループ有無も指定可能
        /// </summary>
        public void Play(SoundType type, bool loop = false)
        {
            if (!clipDict.TryGetValue(type, out var clip))
            {
                Debug.LogWarning($"Clip not found for {type}");
                return;
            }

            if (loop)
            {
                // BGMなど：ループ再生
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.Play();
            }
            else
            {
                // 効果音など：1回だけ再生
                seSource.PlayOneShot(clip);
            }
        }
    }
}
