using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Player.Skills;
using UnityEngine;

namespace Gavi.Skills
{
    public class SkillPerformanceResult
    {
        public SkillPerformance Performance;
        public List<Character> Targets;

        public SkillPerformanceResult()
        {
            Targets = new List<Character>();
        }
    }
    
    public class SkillPerformance
    {
        private Skill _skill;
        public Skill Skill { get => _skill; set => _skill = value; }
        private SkillManager _origin;

        public List<Character> Targets => _targets;
        private List<Character> _targets = new List<Character>();
        
        public List<SkillEffect> Effects = new List<SkillEffect>();

        private bool _isFirstPerform = true;
        
        public SkillPerformanceResult Perform(List<SkillEffect> effects = null)
        {
            SkillPerformanceResult result = new SkillPerformanceResult();
            result.Performance = this;
            if (effects != null)
                Effects = effects;

            // if (Effects == null || Effects.Count < 1)
            //     return;
            
            _origin = _skill.SkillManager;
            
            // get targets if SkillRange.Activision == OnPerformance
            if (_skill.Range.ActivationPoint == SkillRange.ActivisionPoint.OnPerformance)
                SetTargets(GetTargets());
            // validate the targets again (the targets can potentially be chosen when a skill has been started casting and now might be a few seconds later)
            _skill.Range.ValidateTargets(_targets);
            // set targets to result
            result.Targets = new List<Character>(_targets);
            
            // manage mana
            if (Skill.SkillMana != null && Skill.SkillMana.Activation == SkillMana.ActivationType.OnFirstPerform && _isFirstPerform)
                Skill.ManaManager.CurrentMana -= Skill.SkillMana.ManaCost;
            _isFirstPerform = false;
            if (Skill.SkillMana != null && Skill.SkillMana.Activation == SkillMana.ActivationType.OnEachPerform)
                Skill.ManaManager.CurrentMana -= Skill.SkillMana.ManaCost;

            // skill effects
            for (int effectId = 0; effectId < Effects.Count; effectId++)
            {
                for (int targetId = 0; targetId < _targets.Count; targetId++)
                {
                    Effects[effectId].PerformEffect(_targets[targetId]);
                }
            }

            return result;
        }

        public void SetTargets(List<Character> targets)
        {
            _targets = targets;
        }

        public List<Character> GetTargets()
        {
            if (_skill.Range.ActivationPoint == SkillRange.ActivisionPoint.OnPerformance)
                return _skill.Range.GetTargets();
            return _targets;
        }
    }
}
