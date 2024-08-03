using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;

namespace Gavi.Skills
{
    public class SkillEffect : MonoBehaviour
    {
        public virtual void PerformEffect(Character target)
        {
            
        }

        public virtual string GetDescription(int index)
        {
            return "";
        }
    }
}