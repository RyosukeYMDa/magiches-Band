using UnityEngine;
using System.Collections.Generic;

namespace TechC.MagichesBand.Core
{
    [CreateAssetMenu(fileName = "SoundDatabase", menuName = "Scriptable Objects/SoundDatabase")]
    public class SoundDatabase : ScriptableObject
    {
        [System.Serializable]
        public class SoundEntry
        {
            public SoundType type;
            public AudioClip clip;
        }

        public List<SoundEntry> entries;
    }
}
