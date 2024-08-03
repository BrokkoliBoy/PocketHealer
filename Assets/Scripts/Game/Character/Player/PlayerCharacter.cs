using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Characters
{
    public class PlayerCharacter : Character
    {
        public static PlayerCharacter Instance;


        #region Serialized Variables
        #endregion


        #region Non-Serialized Variables
        #endregion


        #region Mono
        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }
        #endregion


        #region Logic
        //protected override bool IsPlayer()
        //{
        //    return true;
        //}
        #endregion
    }
}
