using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Utility
{
    [System.Serializable]
    public class KeyValueList<TKey, TValue> : IEnumerable<KeyValueEntry<TKey, TValue>>
    {
        public List<KeyValueEntry<TKey, TValue>> _keyValuePairs;

        public KeyValueList()
        {
            _keyValuePairs = new List<KeyValueEntry<TKey, TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            _keyValuePairs.Add(new KeyValueEntry<TKey, TValue>(key, value));
        }

        public void Remove(TKey key)
        {
            for(int i = _keyValuePairs.Count - 1; i >= 0; i--)
            {
                KeyValueEntry<TKey, TValue> entry = _keyValuePairs[i];
                if (entry.Key.Equals(key))
                    _keyValuePairs.RemoveAt(i);
            }
        }

        public TValue GetFirstValue(TKey key)
        {
            TValue value = default;
            bool found = false;
            foreach (KeyValueEntry<TKey, TValue> entry in _keyValuePairs)
            {
                if (entry.Key.Equals(key))
                {
                    if (found)
                    {
                        Debugger.LogError("Warning: While getting first value from given key in KeyValueList, multiple " +
                                          "entries with that key were found. Returning the first found. Key: " + key);
                        continue;
                    }
                    value = entry.Value;
                    found = true;
                }
            }

            if (found)
                return value;
            
            throw new KeyNotFoundException("Key not found while trying to get first value with given key in " +
                                               "KeyValueList.GetFirstValue(TKey) with key: " + key);
        }

        public bool ContainsKey(TKey key)
        {
            foreach (KeyValueEntry<TKey, TValue> entry in _keyValuePairs)
            {
                if (entry.Key.Equals(key))
                    return true;
            }

            return false;
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> dictionary = new ();
            foreach (KeyValueEntry<TKey, TValue> entry in _keyValuePairs)
            {
                if (dictionary.ContainsKey(entry.Key))
                {
                    Debugger.LogError("Could add entry with key '" + entry.Key + "' to dictionary while creating " +
                                      "dictionary from KeyValueList because it is already in the dictionary! Addition " +
                                      "of the entry skipped.");
                    continue;
                }
                dictionary.Add(entry.Key, entry.Value);
            }
            return dictionary;
        }
        
        // Implement IEnumerable<KeyValue<TKey, TValue>>
        public IEnumerator<KeyValueEntry<TKey, TValue>> GetEnumerator()
        {
            return _keyValuePairs.GetEnumerator();
        }

        // Explicit implementation for non-generic IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
