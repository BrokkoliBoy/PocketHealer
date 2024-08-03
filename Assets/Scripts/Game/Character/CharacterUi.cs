using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gavi.Characters
{
    public class CharacterUi : MonoBehaviour
    {
        #region Serialized Variables

        [Tooltip("Specifies the initial strength of the heal indication (y, 0-1) depending on the incoming heal strength (x, 0-1).")]
        [SerializeField] private AnimationCurve _healIndicatorStrength = AnimationCurve.Linear(0f, 0.3f, 1f, 0.6f);
        [Tooltip("Specifies the flow of the indication (y) depending on the remaining strength.")]
        [SerializeField] private AnimationCurve _healIndicationProgress = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [Tooltip("Specifies the duration of the indication (y) depending on the incoming heal strength (x).")]
        [SerializeField] private AnimationCurve _healIndicationDuration = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        
        [SerializeField] private Bar _healthBar;
        [SerializeField] private MultiBar _healthMultiBar;
        public RectTransform RootTransform => _rootTransform;
        [SerializeField] private RectTransform _rootTransform;
        [SerializeField] private GameObject _highlightTargetedEnemy;
        [SerializeField] private GameObject _highlightTargetedAlly;
        [SerializeField] private Image _imageHealIndicator;
        #endregion


        #region Non-Serialized Variables
        private Character _character;
        public bool HighlightTargetedEnemy { set { if (_highlightTargetedEnemy != null) _highlightTargetedEnemy.SetActive(value); } }
        public bool HighlightTargetedAlly { set { if (_highlightTargetedAlly != null) _highlightTargetedAlly.SetActive(value); } }

        private Color _healIndicatorDefaultColor;
        private Coroutine _coroutineHealIndication;
        private float _remainingStrength;
        #endregion


        #region Mono
        protected virtual void Awake()
        {
            _character = GetComponent<Character>();
            HighlightTargetedAlly = false;
            HighlightTargetedEnemy= false;
            if (_imageHealIndicator != null)
            {
                _healIndicatorDefaultColor = _imageHealIndicator.color;
                _imageHealIndicator.color = new Color(_imageHealIndicator.color.r, _imageHealIndicator.color.g, _imageHealIndicator.color.b, 0);
            }
        }
        #endregion

        
        #region Life Cycle
        public void UpdateHealth()
        {
            if (_healthBar != null)
            {
                _healthBar.Text = string.Format("{0:0}", Mathf.Ceil(_character.Health.CurrentHealth)) + " / " +
                                  string.Format("{0:0}", Mathf.Ceil(_character.Health.MaxHealth));
                _healthBar.Value = _character.Health.CurrentHealthRate;
            }

            if (_healthMultiBar != null)
            {
                _healthMultiBar.Text =  string.Format("{0:0}", Mathf.Ceil(_character.Health.CurrentHealth)) + " / " + string.Format("{0:0}", Mathf.Ceil(_character.Health.MaxHealth));
                _healthMultiBar.SetValue(1, _character.Health.CurrentHealthWithoutBarrierRate);
                _healthMultiBar.SetValue(0, _character.Health.CurrentHealthWithBarrierRate);
            }
        }
        
        public void DestroyUi()
        {
            if (RootTransform == null)
                return;
            Destroy(RootTransform.gameObject);
        }

        public void OnDied()
        {
            HighlightTargetedAlly = false;
            HighlightTargetedEnemy = false;
        }
        #endregion
        
        
        #region Heal Indication
        public void OnHealed(float healRate)
        {
            float strength = Mathf.Clamp01(healRate);
            EnqueueHealIndication(strength);
        }

        private void EnqueueHealIndication(float strength)
        {
            if (_imageHealIndicator == null)
                return;
            
            if (strength <= _remainingStrength)
                return;
            
            if (_coroutineHealIndication != null)
                StopCoroutine(_coroutineHealIndication);
            _coroutineHealIndication = StartCoroutine(PerformHealIndication(strength));
        }

        private IEnumerator PerformHealIndication(float strength)
        {
            _remainingStrength = _healIndicatorStrength.Evaluate(strength);
            float duration = _healIndicationDuration.Evaluate(strength);
            while (_remainingStrength > 0)
            {
                if (duration <= 0)
                {
                    Debug.LogError("t < 0");
                    duration = 0.001f;
                }
                _remainingStrength -= Simulation.DeltaTime / duration;
                _remainingStrength = Mathf.Max(0, _remainingStrength);
                float a = _healIndicatorDefaultColor.a * _healIndicationProgress.Evaluate(_remainingStrength);
                _imageHealIndicator.color = new Color(_healIndicatorDefaultColor.r, _healIndicatorDefaultColor.g, _healIndicatorDefaultColor.b, a);
                yield return null;
            }
            _imageHealIndicator.color = new Color(_healIndicatorDefaultColor.r, _healIndicatorDefaultColor.g, _healIndicatorDefaultColor.b, 0);
        }
        #endregion
    }
}