using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using UnityEngine;

namespace Gavi.Skills
{
    public class SkillEffectHeal : SkillEffect
    {
        [SerializeField] private float _healAmount;

        public override void PerformEffect(Character target)
        {
            target.GainHeal(_healAmount);
        }

        public override string GetDescription(int index)
        {
            return _healAmount + "";
        }
    }
}
