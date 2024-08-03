using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Characters
{
    public class Ally : Character
    {
        public override void Die()
        {
            Party.Instance.OnAllyDied(this);
            base.Die();
        }

        //protected override bool IsAllyM()
        //{
        //    return true;
        //}
    }
}
