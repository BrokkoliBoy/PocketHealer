using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gavi.Utility
{
    public class ShowFps : MonoBehaviour
    {
        //public GameObject m_textObject;
        [Header("------- Settings -------")]
        [SerializeField] private int m_considerLastFramesNumber = 10;
        [SerializeField] private float m_refreshTextTime = 0.05f;
        [SerializeField] private string m_prefixText;

        [Header("------- Debug -------")]
        private float m_finalFps;
        private Queue<float> m_lastUpdateTimes;

        [SerializeField] private TextMeshProUGUI m_fpsText;
        private bool m_showText;
        private int m_sizeActual;
        private float m_refreshTextRdyTime;

        #region Mono
        void Start()
        {
            if (m_fpsText == null)
            {
                m_fpsText = GetComponent<TextMeshProUGUI>();
                if (m_fpsText == null)
                {
                    m_showText = false;
                    Debug.Log("Aborted: text object or text component is null!");
                    return;
                }
            }
            m_showText = true;


            m_lastUpdateTimes = new Queue<float>();
            m_sizeActual = m_considerLastFramesNumber;
            if (m_sizeActual < 1)
            {
                Debug.Log("Warning: The parameter m_considerLastFramesNumber of the class ShowFps should be above 0!" +
                            " It has been set to the default value of 1.");
                m_sizeActual = 1;
            }
        }
        void Update()
        {
            ManageFps();
        }
        #endregion

        #region ManageFps
        void ManageFps()
        {
            if (!m_showText)
                return;

            CalculateFps();
            DisplayFps();
        }
        void CalculateFps()
        {
            float fps = 0;
            if (m_finalFps > 0)
                fps = 1 / m_finalFps;

            if (m_lastUpdateTimes.Count < m_sizeActual)
            {
                if (m_lastUpdateTimes.Count > 1)
                    fps *= m_lastUpdateTimes.Count;

                m_lastUpdateTimes.Enqueue(Time.unscaledDeltaTime);
                fps += Time.unscaledDeltaTime;
                fps /= m_lastUpdateTimes.Count;
            }
            else
            {
                float oldValue = m_lastUpdateTimes.Dequeue();
                fps -= oldValue / (m_lastUpdateTimes.Count + 1);
                m_lastUpdateTimes.Enqueue(Time.unscaledDeltaTime);
                fps += Time.unscaledDeltaTime / m_lastUpdateTimes.Count;
            }

            if (fps > 0)
                m_finalFps = 1 / fps;
        }
        void DisplayFps()
        {
            if (m_refreshTextRdyTime > Time.time)
                return;

            m_fpsText.text = m_prefixText + (int)m_finalFps;
            m_refreshTextRdyTime = m_refreshTextTime + Time.time;
        }
        #endregion
    }
}