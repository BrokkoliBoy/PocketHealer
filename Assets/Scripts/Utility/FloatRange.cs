using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Utility
{
    [System.Serializable]
    public class FloatRange
    {
        public float Max => _max;
        [SerializeField] private float _min;
        public float Min => _min;
        [SerializeField] private float _max;
        [SerializeField] private RoundingType _roundingType;

        private enum RoundingType { None, Floor, Ceil, ToNearestInteger }

        // makes sure that Value is always initialized
        private bool _isInitialized;

        private float _value;
        public float Value
        {
            get
            {
                if (!_isInitialized)
                    _value = NewValue;
                return _value;
            }
        }

        public float NewValue
        {
            get
            {
                _value = Random.Range(_min, _max);
                if (_roundingType == RoundingType.None)
                    ;
                if (_roundingType == RoundingType.Floor)
                    _value = Mathf.Floor(_value);
                if (_roundingType == RoundingType.Ceil)
                    _value = Mathf.Ceil(_value);
                if (_roundingType == RoundingType.ToNearestInteger)
                    _value = Mathf.RoundToInt(_value);

                _isInitialized = true;
                return Value;
            }
        }
    }
}
