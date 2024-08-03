using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.States;

namespace Gavi.Characters
{
    public class CharacterStats : MonoBehaviour
    {
        #region Serialized Variables
        #endregion


        #region Non-Serialized Variables
        public float AttackRateAdditive { get; set; } = 1;
        //public float AttackRateMultiplicative { get; set; } = 1;
        public float HasteRateAdditive { get; set; } = 1;
        //public float HasteRateMultiplicative { get; set; } = 1;
        public float CooldownRateAdditive { get; set; } = 1;
        //public float CooldownRateMultiplicative { get; set; } = 1;

        private StateManager _stateManager;
        #endregion


        #region Mono
        private void Awake()
        {
            _stateManager = GetComponent<StateManager>();
        }
        #endregion


        #region Logic

        #endregion
    }
}
