using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Control;
using Gavi.Encounter;
using UnityEngine;

namespace Gavi.Skills
{
    public class SkillRangeRandomOpponent : SkillRange
    {
        [Header("------ Random Opponent ------")]
        [SerializeField] private int _numberTargets = 1;
        [SerializeField] private float _changeTargetCooldown;
        [SerializeField] private bool _cannotTargetTwiceInARow;
        [SerializeField] private bool _canHitTargetsMultipleTime;
        [SerializeField] private bool _priorizeTank;
        private List<Character> _aliveMembers = new List<Character>();
        private List<Character> _tanks = new List<Character>();

        private List<Character> _targets = new List<Character>();
        private float _timeForChangeTarget;

        public override List<Character> GetTargets()
        {
            if (_timeForChangeTarget > EncounterManager.Instance.TimeSinceEncounterStart)
                return _targets;
            _timeForChangeTarget = EncounterManager.Instance.TimeSinceEncounterStart + _changeTargetCooldown;

            _aliveMembers.Clear();
            _tanks.Clear();

            if (Skill.SkillManager.Character.IsAlly)
            {
                foreach (Enemy enemy in EncounterManager.Instance.CurrentEncounter.AliveEnemies)
                {
                    if (!_cannotTargetTwiceInARow || !_targets.Contains(enemy))
                        _aliveMembers.Add(enemy);
                }
            }
            else if (Skill.SkillManager.Character.IsEnemy)
            {
                foreach (Character ally in Party.Instance.AliveMembers)
                {
                    if (!_cannotTargetTwiceInARow || !_targets.Contains(ally))
                        _aliveMembers.Add(ally);
                }
            }
            else
            {
                Debug.Log("!Skill.SkillManager.Character.IsAlly && !Skill.SkillManager.Character.IsEnemy_skill.SkillManager.Character.IsEnemy " +
                          "in SkillRangeRandomOpponent.GetTargets()");
            }

            if (_priorizeTank)
            {
                for (int i = 0; i < _aliveMembers.Count; i++)
                {
                    if (_aliveMembers[i].Role == Character.RaidRole.Tank && (!_cannotTargetTwiceInARow || !_targets.Contains(_aliveMembers[i])))
                        _tanks.Add(_aliveMembers[i]);
                }
            }
            ValidateTargets(_aliveMembers);

            _targets.Clear();
            for (int i = _numberTargets - 1; i >= 0; i--)
            {
                bool isTanks = _priorizeTank && _tanks.Count > 0;
                List<Character> pool = isTanks ? _tanks : _aliveMembers;
                if (pool.Count == 0)
                    break;

                int randomIndex = Random.Range(0, pool.Count);
                _targets.Add(pool[randomIndex]);
                if (!_canHitTargetsMultipleTime)
                {
                    if (isTanks)
                        _tanks.RemoveAt(randomIndex);
                    else
                        _aliveMembers.RemoveAt(randomIndex);
                }
            }
            
            if (_targets.Count == 0)
                Debug.Log("Oops! targets.Count == 0 in SkillRangeRandomOpponent.GetTargets()");

            return _targets;
        }

        public override bool IsCastValid()
        {
            if (Skill.SkillManager.Character.IsAlly)
                return EncounterManager.Instance.CurrentEncounter.AliveEnemies.Count > 0;
            if (Skill.SkillManager.Character.IsEnemy)
                return Party.Instance.AliveMembers.Count > 0;
            return false;
        }
    }
}
