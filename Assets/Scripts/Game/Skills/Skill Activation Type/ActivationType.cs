using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Skills
{
    public class ActivationType : MonoBehaviour
    {
        public virtual bool IsActivated()
        {
            return true;
        }
        public virtual void OnSuccessfullyActivated()
        {
            
        }
    }
}
