using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Skills;
using Gavi.Characters;

namespace Gavi.Bosses
{
    public class SkillEffectDamage : SkillEffect
    {
        [SerializeField] private Utility.FloatRange _damage;
        [SerializeField] private List<DamageClassModifier> _roleModifiers;

        public override void PerformEffect(Character target)
        {
            float damage = _damage.NewValue;
            foreach (DamageClassModifier modifier in _roleModifiers)
            {
                if (modifier.Roles.Contains(target.Role))
                    damage *= modifier.Modifier;
            }
            target.TakeDamage(damage);
        }

        public override string GetDescription(int index)
        {
            return _damage.Min == _damage.Max ? _damage.Min + "" : _damage.Min + "-" + _damage.Max;
        }
    }

    [System.Serializable]
    public class DamageClassModifier
    {
        public List<Character.RaidRole> Roles => _roles;
        [SerializeField] private List<Character.RaidRole> _roles;
        public float Modifier => _modifier;
        [SerializeField] private float _modifier;
    }
}
