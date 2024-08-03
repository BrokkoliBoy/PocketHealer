using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Control
{
    public class MouseClick : MonoBehaviour
    {
        [HideInInspector] public bool IsClickDown;

        private void LateUpdate()
        {
            IsClickDown = false;
        }
    }
}
