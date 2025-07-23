using UnityEngine;
using System.Collections.Generic;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// 各種類のサウンドとAudioClipを紐付けて管理するデータベース
    /// ScriptableObjectとしてプロジェクト内にデータ資源として保存できる
    /// ゲーム内のSoundTypeに対応する効果音やBGMを一括で管理するために使用する
    /// </summary>
    [CreateAssetMenu(fileName = "SoundDatabase", menuName = "Scriptable Objects/SoundDatabase")]
    public class SoundDatabase : ScriptableObject
    {
        /// <summary>
        /// サウンドの種類とAudioClipをペアで保持する構造体
        /// Inspector上で登録しやすいようにSerializableにしてある
        /// </summary>
        [System.Serializable]
        public class SoundEntry
        {
            public SoundType type;
            public AudioClip clip;
        }

        //SoundEntryを複数保持するリスト
        public List<SoundEntry> entries;
    }
}
