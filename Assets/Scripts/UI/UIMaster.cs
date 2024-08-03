using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi
{
    public class UIMaster : MonoBehaviour
    {
        public static UIMaster Instance;
        
        [SerializeField] private Canvas _masterCanvas;
        public static Canvas Mastercanvas => Instance._masterCanvas;

        public static Camera Camera => Instance._camera;
        [SerializeField] private Camera _camera;


        private void Awake()
        {
            Instance = this;
        }
    }
}
