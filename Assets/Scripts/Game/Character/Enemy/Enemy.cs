using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using UnityEngine;
using Gavi.Encounter;
using UnityEngine.Serialization;

namespace Gavi.Characters
{
    public class Enemy : Character
    {
        #region Serialized Variables
        private EnemyUi _enemyUi;
        public EnemyUi EnemyUi => _enemyUi;
        #endregion


        #region Mono
        protected override void Awake()
        {
            base.Awake();
            _enemyUi = GetComponent<EnemyUi>();
        }
        #endregion
        

        #region Life Cycle
        public override void Die()
        {
            EncounterManager.Instance.CurrentEncounter.OnEnemyDied(this);
            base.Die();
        }

        public override void DestroySelf()
        {
            _enemyUi.DestroyUi();
            base.DestroySelf();
        }
        #endregion

        //protected override bool IsEnemyM()
        //{
        //    return true;
        //}
    }
}
