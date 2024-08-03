using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Game
{
    public class GameStart : MonoBehaviour
    {

        [SerializeField] private int m_targetFrameRate = 144;
        [SerializeField] private float m_targetFixedDeltaTime = 0.02f;

        public static float TargetFrameRate;

        void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = m_targetFrameRate > 0 ? m_targetFrameRate : 999;
            if (m_targetFixedDeltaTime > 0)
                Time.fixedDeltaTime = m_targetFixedDeltaTime;
        }
    }
}