using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;

namespace Gavi.Skills
{
    public class SkillEffectAlterStats : SkillEffect
    {
        #region Serialized Variables
        [Tooltip("0 means no change.")]
        [SerializeField] private float _alterAttackRateAdditive;
        [SerializeField] private float _alterHasteRateAdditive;
        [SerializeField] private float _alterCooldownRateAdditive;
        #endregion


        #region Non-Serialized Variables
        #endregion


        #region Mono
        #endregion


        #region Logic
        public override void PerformEffect(Character target)
        {
            if (target == null)
            {
                Utility.Debugger.LogAssertionFail("target == null in SkillEffecttAlterStats.PerformEffect(...) on " + target.gameObject.name);
                return;
            }

            CharacterStats stats = target.Stats;
            if (stats == null)
            {
                Utility.Debugger.LogAssertionFail("stats == null in SkillEffecttAlterStats.PerformEffect(...) on " + target.gameObject.name);
                return;
            }
            stats.AttackRateAdditive += _alterAttackRateAdditive;
            stats.HasteRateAdditive += _alterHasteRateAdditive;
            stats.CooldownRateAdditive += _alterCooldownRateAdditive;
        }
        #endregion


        #region Description

        public override string GetDescription(int index)
        {
            string text = "";
            if (_alterAttackRateAdditive != 0)
            {
                text += "Attack: " + (_alterAttackRateAdditive * 100) + "%";
            }
            if (_alterHasteRateAdditive != 0)
            {
                if (text != "")
                    text += "\n";
                text += "Haste: " + (_alterHasteRateAdditive * 100) + "%";
            }

            if (_alterCooldownRateAdditive != 0)
            {
                if (text != "")
                    text += "\n";
                text += "Cooldown Reduction: " + (_alterCooldownRateAdditive * 100) + "%";
            }
            return text;
        }

        #endregion
    }
}
