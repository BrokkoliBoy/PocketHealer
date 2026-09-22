using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;
using Gavi.Encounter;
using Gavi.Utility;

namespace Gavi.Skills
{
    public class SkillRangeCustom : SkillRange
    {

        #region Serialized Variables
        [Header("--- SkillRangeCustom ---")]
        [Tooltip("Defines the amount of enemies hit. If it exceeds this number the size of the pool, multiple enemies will be hit at once " +
            "unless other rules forbit that.")]
        [SerializeField] private int _numberTargets = 1;
        [Tooltip("Only targets of these TargetTypes will be considered targetable. What is considered as enemy/ally is evaluated from " +
            "the character using the skill range.")]
        [SerializeField] private List<TargetType> _targetTypes = new List<TargetType>()
        {
            TargetType.Enemy
        };
        [Tooltip("Only targets of the either of these Rolees will be considered targetable. If no roles are defined, all roles will be accepted.")]
        [SerializeField] private List<Character.RaidRole> _roles = new List<Character.RaidRole>();
        //[SerializeField] private List<Character.RaidRole> _rolePriorities = new List<Character.RaidRole>();
        [SerializeField] private List<JaggedList<Character.RaidRole>> _prioOrder = new List<JaggedList<Character.RaidRole>>();

        [Header("- Hit Same Target -")]
        [Tooltip("Defines how often a character can be targeted within a single targetment. If 0, there is no limit.")]
        [SerializeField]private int _maxNumberHitSameTarget = 1;
        [Tooltip("Defines how many characters must be targeted first before targeting that character another time " +
            "(within the same targetment).")]
        [SerializeField] private int _numberDontHitSameInARow;
        [Tooltip("If this is set to true, if the skill hits multiple targets and if there is at least one character of a prio " +
            "role available, no characters from other prio roles will be chosen." +
            "\n\n" +
            "You could for example imagine a skill that targets 10 characters but should not hit the same character more than once " +
            "within the same targeting. Now imagine having the prio order having tanks on the first place and having exactly two " +
            "tanks. If _doNotLeavePrioOrder is set to false, the 2 tanks plus up to 8 non-tanks will be chosen. But if set to true, " +
            "the non-tanks will never be considered targetable (unless there are no tanks available of course).")]
        [SerializeField] private bool _doNotLeavePrioOrder = true;

        [Header("- Target Switching -")]
        [Tooltip("Defines in seconds after how much time the target should be switched. In the meantime, the same targets will be targeted " +
            "all the time.")]
        [SerializeField] private float _timeSwitchTarget;
        [Tooltip("Defines in amount after how many characters targeted the target should be switched. In the meantime, the same targets will be targeted " +
            "all the time.")]
        [SerializeField] private int _targetsForSwitchTargets;
        [Tooltip("Defines in amount after how many targetings the target should be switched. In the meantime, the same targets will be targeted " +
            "all the time.")]
        [SerializeField] private int _targetingsForSwitchTargets;

        [Header("- Misc -")]
        [Tooltip("If true, the skill can be cast even if there is no pool. The skill will (most likely) be cast as usual but with no target")]
        [SerializeField] private bool _canBeCastIfNoPool;
        #endregion


        #region Non-Serialized Variables
        private enum TargetType { None, Self, NotSelf , Ally, Enemy, Player }
        private List<Character> _targets = new List<Character>();
        private List<Character> _pool = new List<Character>();
        //private List<List<Character>> _prioPool = new List<List<Character>>();
        private Dictionary<Character.RaidRole, List<Character>> _roleDictRaw = new Dictionary<Character.RaidRole, List<Character>>();
        // role dictionary in which for this targeting unavailable characters will be removed from. We need the raw dictionary intact
        // to be able to handle _doNotLeavePrioOrder.
        private Dictionary<Character.RaidRole, List<Character>> _roleDictRemoved = new Dictionary<Character.RaidRole, List<Character>>();
        private Dictionary<Character, int> _characterHits = new Dictionary<Character, int>();
        private List<Character> _lastCharactersHit = new List<Character>();
        private float _timeSwitchTargetRdy;
        private int _charactersTargetedAfterSwitchingTarget;
        private int _targetingsDoneAfterSwitchingTarget;
        #endregion


        #region Mono
        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterInitialize.AddListener(ResetTargetSwitchCache);
        }

        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(ResetTargetSwitchCache);
        }

        // Skills (especially the player's) live across encounter boundaries, but their targets
        // (enemies, party members) get destroyed and recreated for every encounter. Without this,
        // GetTargets()'s target-switch cache below could hand back references to characters from a
        // fight that has already ended - TargetIsValid() can't tell a destroyed target from a null
        // one, so the stale reference is never filtered out.
        private void ResetTargetSwitchCache()
        {
            _targets.Clear();
            _timeSwitchTargetRdy = 0;
            _targetingsDoneAfterSwitchingTarget = 0;
            _charactersTargetedAfterSwitchingTarget = 0;
        }
        #endregion


        #region Logic
        public override List<Character> GetTargets()
        {
            if (EncounterManager.Instance.TimeSinceEncounterStart < _timeSwitchTargetRdy
                || _targetingsDoneAfterSwitchingTarget < _targetingsForSwitchTargets
                || _charactersTargetedAfterSwitchingTarget < _targetsForSwitchTargets)
            {
                _targetingsDoneAfterSwitchingTarget++;
                _charactersTargetedAfterSwitchingTarget += _targets.Count;
                ValidateTargets(_targets);
                if (_targets.Count > 0)
                    return _targets;
            }
            _timeSwitchTargetRdy = _timeSwitchTarget + EncounterManager.Instance.TimeSinceEncounterStart;
            _targetingsDoneAfterSwitchingTarget = 1;
            _charactersTargetedAfterSwitchingTarget = 0;

            GetPool();
            //_prioPool = ApplyPrioOrder(_pool);
            FillRoleDict();
            SelectTargets();

            return _targets;
        }

        #region Control Pool
        private void GetPool()
        {
            Character thisCharacter = Skill.SkillManager.Character;

            // fill pool with all possible targets
            _pool.Clear();
            if (_targetTypes.Contains(TargetType.None))
            {
                ;
            }
            if (_targetTypes.Contains(TargetType.Self))
            {
                AddToPool(thisCharacter);
            }
            if (_targetTypes.Contains(TargetType.Ally))
            {
                if (thisCharacter.IsPlayer || thisCharacter.IsAlly)
                {
                    foreach (Character ally in Party.Instance.AliveMembers)
                        AddToPool(ally);
                }
                else if (thisCharacter.IsEnemy)
                {
                    foreach (Character enemy in EncounterManager.Instance.CurrentEncounter.AliveEnemies)
                        AddToPool(enemy);
                }
            }
            if (_targetTypes.Contains(TargetType.Enemy))
            {
                if (thisCharacter.IsPlayer || thisCharacter.IsAlly)
                {
                    foreach (Character enemy in EncounterManager.Instance.CurrentEncounter.AliveEnemies)
                        AddToPool(enemy);
                }
                else if (thisCharacter.IsEnemy)
                {
                    // meant are the global allies, not the current characters allies
                    foreach (Character ally in Party.Instance.AliveMembers)
                        AddToPool(ally);
                }
            }
            if (_targetTypes.Contains(TargetType.Player))
            {
                AddToPool(PlayerCharacter.Instance);
            }

            // sort out characters that do not match the rules
            // start with not self
            if (_targetTypes.Contains(TargetType.NotSelf))
            {
                RemoveFromPool(PlayerCharacter.Instance);
            }
            // roles
            for (int i = _pool.Count - 1; i >= 0; i--)
            {
                if (_roles.Count > 0 && !_roles.Contains(_pool[i].Role))
                    _pool.RemoveAt(i);
            }
            // parent validation
            ValidateTargets(_pool);
            //return _pool;
        }

        private void AddToPool(Character character)
        {
            if (!_pool.Contains(character))
                _pool.Add(character);
        }

        private void RemoveFromPool(Character character)
        {
            if (_pool.Contains(character))
                _pool.Remove(character);
        }
        #endregion


        #region Prio
        private void FillRoleDict(/*List<Character> pool*/)
        {
            _roleDictRaw.Clear();
            _roleDictRemoved.Clear();
            foreach (Character character in _pool)
            {
                AddToRoleDict(character, _roleDictRaw);
                AddToRoleDict(character, _roleDictRemoved);
            }
            //return _roleDictRaw;
        }

        private void AddToRoleDict(Character character, Dictionary<Character.RaidRole, List<Character>> roleDict)
        {
            if (!roleDict.ContainsKey(character.Role))
                roleDict.Add(character.Role, new List<Character>());
            if (!roleDict[character.Role].Contains(character))
                roleDict[character.Role].Add(character);
        }

        private void RemoveFromRoleDict(Character character)
        {
            if (!_roleDictRemoved.ContainsKey(character.Role))
                return;
            if (_roleDictRemoved[character.Role].Contains(character))
                _roleDictRemoved[character.Role].Remove(character);
        }

        private Character GetRandomPrioCharacter(/*Dictionary<Character.RaidRole, List<Character>> roleDict, List<JaggedList<Character.RaidRole>> prioOrder*/)
        {
            List<Character> pool = new List<Character>();
            int highestPrioRemoved = 0;
            for (int prioIndex = 0; prioIndex < _prioOrder.Count; prioIndex++)
            {
                JaggedList<Character.RaidRole> prioList = _prioOrder[prioIndex];
                bool doBreak = false;
                for  (int i = 0; i < prioList.Count; i++)
                {
                    // if no character of the prio role is available, continue
                    if (!_roleDictRemoved.ContainsKey(prioList[i]))
                        continue;
                    if (_roleDictRemoved[prioList[i]].Count <= 0)
                        continue;

                    // else add them to the pool of possible characters
                    foreach (Character character in _roleDictRemoved[prioList[i]])
                        pool.Add(character);
                    highestPrioRemoved = prioIndex;
                    doBreak = true;
                }
                if (doBreak)
                    break;
            }

            // handle _doNotLeavePrioOrder
            if (_doNotLeavePrioOrder)
            {
                // get the highest available role from the prio order
                int highestPrioRaw = 0;
                for(int prioIndex = 0; prioIndex < _prioOrder.Count; prioIndex++)
                {
                    bool doBreak = false;
                    JaggedList<Character.RaidRole> prioList = _prioOrder[prioIndex];
                    for (int i = 0; i < prioList.Count; i++)
                    {
                        if (!_roleDictRaw.ContainsKey(prioList[i]))
                            continue;
                        if (_roleDictRaw[prioList[i]].Count <= 0)
                            continue;

                        highestPrioRaw = prioIndex;
                        doBreak = true;
                    }
                    if (doBreak)
                        break;
                }

                if (highestPrioRaw < highestPrioRemoved)
                    return null;
            }

            // if there is at least one character in the pool, choose a random one
            if (pool.Count > 0)
                return pool[Random.Range(0, pool.Count)];

            // else no available character has a role of the roles in the prio order.
            // Chose a random character from a non-prio role.

            // first count the amount of characters of a role to select random role that the random character will be out of
            int index = 0;
            Dictionary<Character.RaidRole, int> roleDistribution = new Dictionary<Character.RaidRole, int>();
            foreach (KeyValuePair<Character.RaidRole, List<Character>> roleList in _roleDictRemoved)
            {
                if (roleList.Value.Count <= 0)
                    continue;
                index += roleList.Value.Count;
                roleDistribution.Add(roleList.Key, index);
            }

            // then select the role and add a random character of the role
            int randomIndex = Random.Range(0, index);
            foreach (KeyValuePair<Character.RaidRole, int> pair in roleDistribution)
            {
                if (randomIndex >= pair.Value)
                    continue;
                return _roleDictRemoved[pair.Key][Random.Range(0, _roleDictRemoved[pair.Key].Count)];
            }

            return null;
        }
        #endregion


        #region Selecting Targets
        private void SelectTargets(/*Dictionary<Character.RaidRole, List<Character>> roleDict, List<JaggedList<Character.RaidRole>> prioOrder*/)
        {
            // choose random targets from that pool
            _targets.Clear();
            _characterHits.Clear();
            _lastCharactersHit.Clear();
            for (int i = 0; i < _numberTargets; i++)
            {
                Character target = GetRandomPrioCharacter();
                if (target == null)
                    break;

                _targets.Add(target);
                _charactersTargetedAfterSwitchingTarget++;

                // manage the last hit characters
                if (_numberDontHitSameInARow > 0)
                {
                    // add the character that was hit long time ago again
                    if (_lastCharactersHit.Count >= _numberDontHitSameInARow)
                    {
                        AddToRoleDict(_lastCharactersHit[_lastCharactersHit.Count - 1], _roleDictRaw);
                        AddToRoleDict(_lastCharactersHit[_lastCharactersHit.Count - 1], _roleDictRemoved);
                        _lastCharactersHit.RemoveAt(_lastCharactersHit.Count - 1);
                    }
                    _lastCharactersHit.Insert(0, target);
                    RemoveFromRoleDict(target);
                }

                // remove target if max number was reached
                if(_maxNumberHitSameTarget > 0)
                {
                    if (!_characterHits.ContainsKey(target))
                        _characterHits.Add(target, 0);
                    _characterHits[target]++;
                    if (_characterHits[target] >= _maxNumberHitSameTarget)
                        RemoveFromRoleDict(target);
                }
            }

            //return _targets;
        }
        #endregion


        public override bool IsCastValid()
        {
            if (_canBeCastIfNoPool)
                return true;
            GetPool();
            //Debug.Log(_targets.Count);
            return _pool.Count > 0;
        }
        #endregion
    }
}
