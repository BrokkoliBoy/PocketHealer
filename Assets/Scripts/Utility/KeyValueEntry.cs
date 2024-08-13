using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Utility
{
    [System.Serializable]
    public class KeyValueEntry<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public KeyValueEntry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
