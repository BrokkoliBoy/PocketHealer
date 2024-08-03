using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;
using Gavi.States;

namespace Gavi.Skills
{
    public class SkillEffectRemoveState : SkillEffect
    {
        [SerializeField] private int _dispellCount = 1;

        public override void PerformEffect(Character target)
        {
            List<State> dispellableStates = new List<State>();
            foreach (State state in  target.StateManager.States)
            {
                if (state.StateData.IsDispellable)
                    dispellableStates.Add(state);
            }

            for (int i = 0; i < _dispellCount; i++)
            {
                if (target.StateManager.States.Count == 0)
                    break;
                target.StateManager.RemoveState(dispellableStates[Random.Range(0, dispellableStates.Count)].StateData);
            }
        }
        #region Serialized Variables
        #endregion


        #region Non-Serialized Variables
        #endregion


        #region Mono
        #endregion


        #region Logic
        #endregion

        
        #region Description

        public override string GetDescription(int index)
        {
            return _dispellCount + "";
        }

        #endregion
    }
}
