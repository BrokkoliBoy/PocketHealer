using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Encounter;

namespace Gavi.Characters
{
    public class EnemyUi : CharacterUi
    {
        #region Mono
        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);
        }
        
        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnEncounterInitialize);
        }
        #endregion

        
        #region Life Cycle
        private void OnEncounterInitialize()
        {
            RootTransform.SetParent(EncounterManager.Instance.BossUiParentTransform);
            RootTransform.localScale = Vector3.one;
            RootTransform.localPosition = Vector3.zero;
        }
        #endregion
    }
}
