using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Skills
{
    public class SkillMana : MonoBehaviour
    {
        [SerializeField] private float _manaCost;
        public float ManaCost => _manaCost;
        [SerializeField] private ActivationType _activationType = ActivationType.OnFirstPerform;
        public ActivationType Activation => _activationType;
        
        public enum ActivationType { OnActivision, OnFirstPerform, OnEachPerform }
    }
}
