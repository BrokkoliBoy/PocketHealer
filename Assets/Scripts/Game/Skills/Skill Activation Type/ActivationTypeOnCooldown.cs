using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Skills
{
    public class ActivationTypeOnCooldown : ActivationType
    {
        public override bool IsActivated()
        {
            return true;
        }
    }
}
