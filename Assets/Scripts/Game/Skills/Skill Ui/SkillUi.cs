using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Encounter;
using Gavi.Player.Skills;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gavi.Skills
{
    public class SkillUi : MonoBehaviour
    {

        #region Serialized Variables
        
        [SerializeField] private Image _iconSkill;
        [SerializeField] private UI.Bar _barCooldown;
        //[SerializeField] private TextMeshProUGUI _textCooldown;
        // [SerializeField] private TextMeshProUGUI _textHotKey;
        public DragAndDroppableSkill DragAndDroppable => _dragAndDroppable;
        [SerializeField] private DragAndDroppableSkill _dragAndDroppable;
        [SerializeField] private Image _imageCastIndicator;
        #endregion


        #region Non-Serialized Variables
        public Skill Skill => _skill;
        private Skill _skill;
        public RectTransform Transform { get { if (_transform == null) _transform = GetComponent<RectTransform>(); return _transform; } }
        private RectTransform _transform;
        #endregion


        #region Mono

        private void Awake()
        {
            DisableCastIndicator();
        }

        private void Update()
        {
            UpdateUi();       
        }
        #endregion


        #region Update Ui
        private void UpdateUi()
        {
            // string keyBindingText = "";
            // KeyBinding keyBinding = _skill.KeyBinding;
            // if (keyBinding != null)
            //     keyBindingText = keyBinding.GetKeyCodeString();
            // _textHotKey.text = keyBindingText;
            
            if (!EncounterManager.Instance.IsInEncounterAndUnpaused)
                return;
            
            if(_barCooldown != null)
            {
                _barCooldown.Text =  _skill.Cooldown.LongestCooldownRemaining <= 0 ? "" : Utility.Utility.SecondsToMinutes(_skill.Cooldown.LongestCooldownRemaining);
                _barCooldown.Value =  _skill.Cooldown.LongestCooldownRate;
            }
        }
        #endregion


        #region Life Cycle
        public void ApplySkill(Skill skill)
        {
            _skill = skill;
            _iconSkill.sprite = skill.IconSkill;
        }
        #endregion


        #region Control
        public void SetParent(RectTransform parent)
        {
            Transform.SetParent(parent);
        }

        public void NeutralizeUi()
        {
            _barCooldown.Value = 0;
            _barCooldown.Text = "";
        }
        #endregion


        #region Cast Detector

        public void EnableCastIndicator()
        {
            if (_imageCastIndicator == null)
                return;
            _imageCastIndicator.gameObject.SetActive(true);
        }

        public void DisableCastIndicator()
        {
            if (_imageCastIndicator == null)
                return;
            _imageCastIndicator.gameObject.SetActive(false);
        }
        #endregion
    }
}
