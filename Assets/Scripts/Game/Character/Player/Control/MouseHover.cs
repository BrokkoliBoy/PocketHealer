using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Control
{
    public class MouseHover : MonoBehaviour
    {
        public bool IsHover;

        private void LateUpdate()
        {
            IsHover = false;
        }
    }
}
