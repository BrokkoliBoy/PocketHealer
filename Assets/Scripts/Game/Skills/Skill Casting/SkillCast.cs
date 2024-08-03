using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Player.Skills;
using UnityEngine;

namespace Gavi.Skills
{
    public class SkillCast : MonoBehaviour
    {
        #region Serialized Variables
        public List<SkillEffect> EffctsSequenceFinish => _effectsSequenceFinish;
        [SerializeField] private List<SkillEffect> _effectsSequenceFinish;
        
        public float CastTime => _castTime;
        [Header("- Casting -")] 
        [SerializeField] private float _castTime = 1.5f;
        public List<SkillEffect> EffectsCastFinish => _effectsCastFinish;
        [SerializeField] private List<SkillEffect> _effectsCastFinish;
        
        [SerializeField] private bool _canBeCastWhileCasting;
        
        [Header("- Channeling -")]
        [SerializeField] private float _channelTime;
        public float ChannelTime => _channelTime;
        [Tooltip("Those effects will be performed each time when performed while channeling.")]
        [SerializeField] private List<SkillEffect> _effectsChanneling;
        public List<SkillEffect> EffectsChanneling => _effectsChanneling;
        
        [SerializeField] private bool _canBeCastWhileChanneling = true;
        [SerializeField] private int _channelPerformances = 1;
        public int ChannelPerformances => _channelPerformances;
        [Tooltip("This will not increase the amount of performances while channeling by one. It only separates " +
                 "the performances a bit to the front.")]
        [SerializeField] private bool _channelPerformOneAtStart;
        public bool ChannelPerformOneAtStart => _channelPerformOneAtStart;
        #endregion

        
        #region Non-Serialized Variables
        private Skill _skill;
        #endregion


        #region Mono
        private void Awake()
        {
            _skill = GetComponent<Skill>();
        }
        #endregion


        #region Logic
        public bool IsCastValid()
        {
            if (!_canBeCastWhileChanneling && _skill.SkillManager.CastManager.IsChanneling)
                return false;
            if (!_canBeCastWhileCasting && _skill.SkillManager.CastManager.IsCasting)
                return false;
            return true;
        }

        public float HasteRate(Skill skill)
        {
            return _skill.SkillManager.CastManager.HasteRate(skill);
        }
        #endregion
    }
}
