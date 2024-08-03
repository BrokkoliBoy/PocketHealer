using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.UI;
using UnityEngine;

namespace Gavi.Skills
{
    public class ManaManager : MonoBehaviour
    {
        [SerializeField] private float _maxMana;
        [SerializeField] private float _manaGainPerSecond;
        
        [Header("- References -")]
        [SerializeField] private Bar _manaBar;
        
        private float _currentMana;

        private void Update()
        {
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
            if (_manaGainPerSecond < 0)
                return;
            
            CurrentMana += _manaGainPerSecond * Time.deltaTime;
        }

        private void OnEnable()
        {
            EncounterManager.Instance.OnEncounterInitialize.AddListener(OnEncounterInitialize);
        }
        private void OnDisable()
        {
            EncounterManager.Instance.OnEncounterInitialize.RemoveListener(OnEncounterInitialize);
        }

        private void OnEncounterInitialize()
        {
            CurrentMana = _maxMana;
        }
        
        public float CurrentMana
        {
            get => _currentMana;
            set
            {
                _currentMana = Mathf.Clamp(value, 0, _maxMana);
                UpdateUi();
            }
        }

        public bool IsCastValid(SkillMana skillMana)
        {
            if (skillMana == null)
                return true;
            if(skillMana.ManaCost > _currentMana)
                return false;
            return true;
        }

        private void UpdateUi()
        {
            if (_manaBar == null)
                return;
            _manaBar.Value = _maxMana <= 0 ? 0 : CurrentMana / _maxMana;
            _manaBar.Text = (int)CurrentMana + " / " + _maxMana;
        }
    }
}
