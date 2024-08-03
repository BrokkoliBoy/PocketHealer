using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gavi.Skills
{
    public class SkillRange : MonoBehaviour
    {
        #region Serialized Variables
        [Header("------ Skill Range ------")]
        
        [SerializeField] protected bool _canTargetDead;
        public ActivisionPoint ActivationPoint => _activationPoint;
        [SerializeField] private ActivisionPoint _activationPoint;
        #endregion

        
        #region Non-Serialized Variables
        protected Skill Skill;
        #endregion

        
        #region Enums
        public enum ActivisionPoint { OnActivision, OnPerformance }
        #endregion

        protected void Awake()
        {
            Skill = GetComponent<Skill>();
        }
        
        public virtual List<Character> GetTargets()
        {
            Debugger.LogInfo("Oops!");
            return null;
        }

        public virtual bool IsCastValid()
        {
            Debugger.LogInfo("Oops!");
            return false;
        }

        protected bool TargetIsValid(Character target)
        {
            return target == null || _canTargetDead || !target.Health.IsDead;
        }

        public void ValidateTargets(List<Character> targets)
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (!TargetIsValid(targets[i]))
                    targets.RemoveAt(i);
            }
        }
    }
}
