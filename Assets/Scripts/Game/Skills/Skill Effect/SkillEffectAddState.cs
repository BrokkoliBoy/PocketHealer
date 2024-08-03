using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;
using Gavi.States;
using Gavi.Utility;

namespace Gavi.Skills
{
    public class SkillEffectAddState : SkillEffect
    {
        [SerializeField] private StateData _stateDataToApply;

        public override void PerformEffect(Character target)
        {
            if (_stateDataToApply == null)
            {
                Debugger.LogAssertionFail("_stateDataToApply == null in SkillEffectAddState.PerformEffect(Character target) on " + gameObject.name);
                return;
            }

            if (target == null)
            {
                Debugger.LogAssertionFail("target == null in SkillEffectAddState.PerformEffect(Character target) on " + gameObject.name);
                return;
            }

            StateManager stateManager = target.StateManager;
            if (stateManager == null)
                return;
            stateManager.AddState(_stateDataToApply);
        }

        public override string GetDescription(int index)
        {
            return _stateDataToApply.Description;
        }
    }
}