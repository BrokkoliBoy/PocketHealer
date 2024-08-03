using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Control;
using Gavi.Encounter;
using UnityEngine;

namespace Gavi.Skills
{
    public class SkillRangePlayerMouseHover : SkillRange
    {
        [SerializeField] private bool _canTargetAlly = true;
        [SerializeField] private bool _canTargetEnemy;
        private List<Character> _pool = new List<Character>();
        
        public override List<Character> GetTargets()
        {
            List<Character> targets = new List<Character>();

            foreach (UiTargetCharacter uiTarget in PlayerInputManager.Instance.CharacterTargets)
            {
                Character target = uiTarget.Character;
                if (target == null)
                {
                    Debug.LogError("character == null in SkillRangePlayerMouseHover.GetTargets()");
                    continue;
                }
                if (!_canTargetAlly && target.IsAlly)
                    continue;
                if (!_canTargetEnemy && target.IsEnemy)
                    continue;
                targets.Add(target);
            }
            
            ValidateTargets(targets);
            return targets;
        }

        public override bool IsCastValid()
        {
            foreach (UiTargetCharacter uiTarget in PlayerInputManager.Instance.CharacterTargets)
            {
                Character character = uiTarget.Character;
                if (TargetIsValid(character))
                    return true;
            }
            return false;
        }
    }
}
