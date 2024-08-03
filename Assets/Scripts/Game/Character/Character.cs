using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Skills;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Gavi.States;

namespace Gavi.Characters
{
    public class Character : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField] private string _name = "Unnamed";
        [SerializeField] private RaidRole _role;
        public RaidRole Role => _role;
        public string Name => _name;
        public bool IsAlly => Role == RaidRole.Tank || Role == RaidRole.Heal || Role == RaidRole.Dps;
        public bool IsEnemy => Role == RaidRole.Boss;
        public bool IsPlayer => Role == RaidRole.Player;
        
        [Header("- References -")]
        [Tooltip("[Can be null] If null, the gameObject that this script is attached to is the _rootGameObject.")]
        [SerializeField] private GameObject _rootGameObject;
        [SerializeField] private TextMeshProUGUI _textFieldName;
        public SkillManager SkillManager => _skillManager;
        [SerializeField] private SkillManager _skillManager;

        public UnityEvent OnDied;
        #endregion

        
        #region Non-Serialized Varibales
        public CharacterHealth Health => _health;
        private CharacterHealth _health;
        
        public CharacterUi Ui => _ui;
        private CharacterUi _ui;

        public StateManager StateManager => _stateManager;
        private StateManager _stateManager;

        public CharacterStats Stats => _stats;
        private CharacterStats _stats;

        // public CharacterBarrier Barrier => _barrier;
        // private CharacterBarrier _barrier;

        public bool IsDead => _health.IsDead;

        public UnityEvent OnInitialize;

        public enum RaidRole { None, Tank, Dps, Heal, Boss, Player }
        #endregion


        #region Mono
        protected virtual void Awake()
        {
            //if (_rootGameObject == null)
            //    _rootGameObject = gameObject;
            _health = GetComponent<CharacterHealth>();
            _ui = GetComponent<CharacterUi>();
            _stateManager = GetComponent<StateManager>();
            _stats = GetComponent<CharacterStats>();
        }
        #endregion
        
        
        #region Life Cycle
        public void Initialize()
        {
            if (_textFieldName != null)
                _textFieldName.text = Name;
            OnInitialize.Invoke();
        }
        
        public virtual void Die()
        {
            if (_stateManager != null)
                _stateManager.RemoveWithoutEffects();
            _ui.OnDied();
            if (OnDied != null)
                OnDied.Invoke();
        }

        public void Destroy()
        {
            if (_ui != null)
                _ui.DestroyUi();
            Destroy(gameObject);
        }

        public virtual void DestroySelf()
        {
            Destroy(_rootGameObject);
        }
        #endregion
        
        
        #region Bridge Functions
        public float CurrentHealth => _health.CurrentHealth;

        // set => _health.CurrentHealth = value;
        public void TakeDamage(float damage)
        {
            _health.TakeDamage(damage);
        }

        public void GainHeal(float heal)
        {
            _health.GainHeal(heal);
        }
        #endregion
    }
}
