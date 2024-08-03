using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Skills;

namespace Gavi.States
{
    //[CreateAssetMenu(fileName = "SampleState", menuName = "ScriptableObjects/State", order = 1)]
    public class StateData : MonoBehaviour
    {
        public string Description => _description;
        [SerializeField][TextArea] private string _description;
        
        public float MaxDuration => _maxDuration;
        [Header("- Duration -")]
        [SerializeField] private float _maxDuration = 15f;

        public float PeriodicCooldown => _periodicCooldown;
        [SerializeField] private float _periodicCooldown = 3f;

        public ReaplyRule ReaplyType => _reapplyType;
        [Header("- Reapply Type -")] 
        [SerializeField] private ReaplyRule _reapplyType;

        public List<SkillEffect> OnStateEnterEffects => _onStateEnterEffects;
        [Header("- Effects -")]
        [SerializeField] private List<SkillEffect> _onStateEnterEffects;

        public List<SkillEffect> OnStateExitEffects => _onStateExitEffects;
        [SerializeField] private List<SkillEffect> _onStateExitEffects;

        public List<SkillEffect> OnStatePeriodicEffects => _onStatePeriodicEffects;
        [SerializeField] private List<SkillEffect> _onStatePeriodicEffects;

        [Header("- Auras -")]
        [SerializeField] private bool _removeStateOnRemoveLastAura = true;
        public bool RemoveStateOnRemoveLastAura => _removeStateOnRemoveLastAura;
        public List<StateAura> Auras => _auras;
        [SerializeField] private List<StateAura> _auras;
        
        [Header("- Tags -")]
        [SerializeField] private bool _isDispellable = true;
        public bool IsDispellable => _isDispellable;

        public Sprite Icon => _icon;
        [Header("- Visual -")]
        [SerializeField] private Sprite _icon;

        public enum ReaplyRule { None, Ignore, RefreshDuration, AddDuration, AddNewState, RemoveOldAndAddNewState }
    }
}
