using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Encounter;
using Gavi.Utility;

namespace Gavi.Characters
{
    public class Party : MonoBehaviour
    {
        public static Party Instance;

        // member in a fight
        private List<Character> _members = new List<Character>();
        public List<Character> Members => _members;

        // alive member in a fight
        private List<Character> _aliveMembers = new List<Character>();
        public List<Character> AliveMembers => _aliveMembers;

        // those are prefabs that have to be instantiated
        private List<Character> _chosenMembers = new List<Character>();
        public List<Character> ChosenMembers => _chosenMembers;

        [SerializeField] private RectTransform _parent;
        [SerializeField] private List<Character> _INITIALLY_CHOSEN_MEMBERS;

        private void Awake()
        {
            if (Instance != null)
                Utility.Debugger.LogAssertionFail(typeof(Party));
            Instance = this;

            for (int i = 0; i < _INITIALLY_CHOSEN_MEMBERS.Count; i++)
                _chosenMembers.Add(_INITIALLY_CHOSEN_MEMBERS[i]);
        }

        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);
            EncounterManager.Instance.OnEncounterClear.AddListener(OnEncounterClear);
        }

        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnEncounterInitialize);
            EncounterManager.Instance.OnEncounterClear.RemoveListener(OnEncounterClear);
        }

        private void OnEncounterInitialize()
        {
            InitializeParty();
        }

        private void OnEncounterClear()
        {
            // _members.Clear();
            // _aliveMembers.Clear();
            ClearParty();
        }


        private void InitializeParty()
        {
            SpawnParty();
        }

        private void ClearParty()
        {
            for (int i = _members.Count - 1; i >= 0; i--)
                _members[i].Destroy();
            _members.Clear();
            _aliveMembers.Clear();
        }

        private void SpawnParty()
        {
            List<Character> characters = EncounterManager.Instance.CurrentEncounter.PartySettings.ForceCharacters ? 
                EncounterManager.Instance.CurrentEncounter.PartySettings.CharactersForced : 
                _chosenMembers;

            if (characters == null)
            {
                Debugger.LogError("characters == null in SpawnParty()");
                characters = new List<Character>();
            }
            
            if (characters.Count == 0)
                Debugger.LogError("characters.Count == 0 in SpawnParty()");
            
            for(int i = 0; i < characters.Count; i++)
            {
                Character character = Instantiate(characters[i].gameObject, _parent).GetComponent<Character>();
                _members.Add(character);
                _aliveMembers.Add(character);
                character.InitializeForBattle();
            }
            PartyUi.Instance.AllignCharacters(_members);
        }

        public void OnAllyDied(Character member)
        {
            _aliveMembers.Remove(member);
            if (_aliveMembers.Count == 0)
                EncounterManager.Instance.FailEncounter();
        }
    }
}