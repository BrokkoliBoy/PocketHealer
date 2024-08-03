using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;

namespace Gavi.Skills
{
    public class CooldownManager : MonoBehaviour
    {
        private SkillManager _skillManager;

        private void Awake()
        {
            _skillManager = GetComponent<SkillManager>();
        }

        public void ApplyGlobalCooldown(float globalCooldown)
        {
            foreach (Skill skill in _skillManager.ActiveSkillsPrioSorted)
                skill.Cooldown.SetGlobalCooldown(globalCooldown);
        }

        public void AbortGlobalCooldown()
        {
            foreach (Skill skill in _skillManager.ActiveSkillsPrioSorted)
                skill.Cooldown.SetGlobalCooldown(0, true);
        }

        public void AbortLocalCooldown()
        {
            foreach (Skill skill in _skillManager.ActiveSkillsPrioSorted)
                skill.Cooldown.SetGlobalCooldown(0, true);
        }

        public float CooldownRate(Skill skill)
        {
            float rate = _skillManager.Character.Stats.CooldownRateAdditive;
            if (rate <= 0)
                rate = 0.0001f;
            return 1 / rate;
        }
    }
}
