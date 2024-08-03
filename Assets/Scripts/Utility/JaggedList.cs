using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Utility
{
    [System.Serializable]
    public class JaggedList<T>
    {
        [SerializeField] private List<T> _data;
        public List<T> Data => _data;

        private void CreateData()
        {
            _data = new List<T>();
        }



        #region Bridge Functions

        #region Adding
        public bool Add(T element)
        {
            if (_data == null)
                CreateData();
            if (_data.Contains(element))
                return false;
            _data.Add(element);
            return true;
        }

        public void Add(List<T> elements)
        {
            for (int i = 0; i < elements.Count; i++)
                Add(elements[i]);
        }

        public void Add(JaggedList<T> elements)
        {
            for (int i = 0; i < elements.Count; i++)
                Add(elements[i]);
        }

        public bool Insert(int index, T element)
        {
            if (_data == null && index == 0)
                CreateData();

            if (_data == null)
                return false;

            if (index < 0 || index >= _data.Count)
                return false;

            _data.Insert(index, element);
            return true;
        }
        #endregion


        #region Removing
        public bool Remove(T element)
        {
            if (_data == null)
                return false;

            if (!_data.Contains(element))
                return false;

            _data.Remove(element);
            return true;
        }

        public bool RemoveAt(int index)
        {
            if (_data == null)
                return false;

            if (index < 0 || index >= _data.Count)
                return false;

            _data.RemoveAt(index);
            return true;
        }
        #endregion


        #region Misc
        public bool Contains(T element)
        {
            if (_data == null)
                return false;

            return _data.Contains(element);
        }

        public int IndexOf(T element)
        {
            if (_data == null)
                return -1;

            return _data.IndexOf(element);
        }

        public void Clear()
        {
            if (_data == null)
                return;

            _data.Clear();
        }

        public int Count
        {
            get
            {
                if (_data == null)
                    return -1;

                return _data.Count;
            }
        }

        public override string ToString()
        {
            if (_data == null || _data.Count == 0)
                return "{ }";

            string s = "{ " + _data[0].ToString();
            for (int i = 1; i < _data.Count; i++)
                s += ", " +_data[i].ToString();

            return s;
        }
        #endregion

        #endregion


        #region Operators
        public T this[int i]
        {
            get { return _data[i]; }
            set { _data[i] = value; }
        }
        #endregion
    }
}
