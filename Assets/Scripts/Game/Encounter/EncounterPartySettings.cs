using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using UnityEngine;
using Gavi.Skills;

namespace Gavi.Encounter
{
    public class EncounterPartySettings : MonoBehaviour
    {
        public bool ForceCharacters => _forceCharacters;
        [Header("- Characters -")]
        [SerializeField] private bool _forceCharacters;

        public List<Character> CharactersForced => _charactersForced;
        [SerializeField] private List<Character> _charactersForced;

        // public bool ForcePlayerSkills => _forcePlayerSkills;
        // [Header("- Player Skills -")]
        // [SerializeField] private bool _forcePlayerSkills = true;
        // [SerializeField] private List<Skill> _playerSkillsForced;
        // public List<Skill> PlayerSkillsForced => _playerSkillsForced;
    }
}
