using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// サウンドを一括管理するシングルトンコンポーネント
    /// BGMとSEの再生を統一的に制御するために使用する
    /// SoundDatabaseに登録されたAudioClipを種類に応じて再生する
    /// </summary>
    public class Sound : SingletonMonoBehaviour<Sound>
    {
        [SerializeField] private AudioSource seSource;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private SoundDatabase database;
        
        private Dictionary<SoundType, AudioClip> clipDict;
        
        /// <summary>
        /// 初期化処理 シングルトンのインスタンス登録とSoundDatabaseから辞書作成を行う
        /// </summary>
        protected override void Awake()
        {
            base.Awake(); // SingletonMonoBehaviour で Instance を設定

            clipDict = new Dictionary<SoundType, AudioClip>(); // 空の辞書を初期化する

            if (!database)
            {
                Debug.LogError("SoundDatabase が設定されていません！");
                return;
            }

            // 有効なclipをclipDictに登録する
            foreach (var entry in database.entries.Where(entry => entry.clip && !clipDict.ContainsKey(entry.type)))
            {
                // 同じtypeが既に存在しない場合のみ追加する
                clipDict.Add(entry.type, entry.clip);
            }
        }

        /// <summary>
        /// 音声を再生するメソッド SoundTypeに対応するAudioClipを再生する
        /// loop引数がtrueのときはBGMとして再生 falseのときはSEとして一度だけ再生される
        /// </summary>
        /// <param name="type">再生するサウンドの種類</param>
        /// <param name="loop">ループ再生するかどうか（デフォルトはfalse）</param>
        public void Play(SoundType type, bool loop = false)
        {
            if (!clipDict.TryGetValue(type, out var clip) || !clip)
            {
                Debug.LogWarning($"AudioClip not found for {type}");
                return;
            }

            if (loop)
            {
                // BGM再生処理 ループ設定を有効にして再生
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.Play();
            }
            else
            {
                // SE再生処理
                seSource.PlayOneShot(clip);
            }
        }
    }
}
