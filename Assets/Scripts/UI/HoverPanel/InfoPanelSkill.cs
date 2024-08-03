using System.Collections;
using System.Collections.Generic;
using Gavi.Skills;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Gavi
{
    public class InfoPanelSkill : InfoPanel
    {
        [SerializeField] private bool _useDynamicSize = true;
        [SerializeField] private Vector2 _staticSize = new Vector2(-1, -1);
        [SerializeField] private float _nonDescriptionHeight = 173f;
        
        [Header("- References -")]
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textManaCost;
        [SerializeField] private TextMeshProUGUI _textCooldown;
        [SerializeField] private TextMeshProUGUI _textCastTime;
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private Image _imageIcon;

        public bool IsShowing => Root.gameObject.activeSelf;
        public float Width => Root.sizeDelta.x;
        
        public void ShowPanel(Skill skill)
        {
            Root.gameObject.SetActive(true);
            
            if (skill == null)
            {
                _textName.text = "SKILL IS NULL";
                _textManaCost.text = "";
                _textCooldown.text = "";
                _textDescription.text = "";
                _imageIcon = null;
                return;
            }
            
            // name
            _textName.text = skill.Name;

            // mana
            if (skill.SkillMana != null)
            {
                _textManaCost.text = skill.SkillMana.ManaCost + "";
                if (_textManaCost.text == "0")
                    _textManaCost.text = "-";
            }
            else
                _textManaCost.text = "-";
            
            // cooldown
            if (skill.Cooldown != null)
            {
                Utility.FloatRange cooldown = skill.Cooldown.LocalCooldown;
                if (cooldown.Min == cooldown.Max)
                    _textCooldown.text = cooldown.Max + "";
                else
                    _textCooldown.text = cooldown.Min + " - " + cooldown.Max;
                if (_textCooldown.text == "0")
                    _textCooldown.text = "-";
            }
            else
                _textCooldown.text = "-";
            
            // cast time
            if (skill.Cast != null)
            {
                float castTime = skill.Cast.CastTime;
                float channelTime = skill.Cast.ChannelTime;
                float totalTime = castTime + channelTime;
                string text = totalTime <= 0 ? "-" : totalTime + "";
                _textCastTime.text = text;
            }

            // description
            _textDescription.text = skill.Description;
            _textDescription.ForceMeshUpdate(false, true);
            float textHeight = _textDescription.text == "" ? 0 : _textDescription.textBounds.size.y;
            _imageIcon.sprite = skill.IconSkill;

            // panel size
            if (_useDynamicSize)
                Root.sizeDelta = new Vector2(Root.sizeDelta.x, textHeight + _nonDescriptionHeight);
            else
                Root.sizeDelta = new Vector2(_staticSize.x > 0 ? _staticSize.x : Root.sizeDelta.x,
                    _staticSize.y > 0 ? _staticSize.y : Root.sizeDelta.y);
        }

        public void HidePanel()
        {
            Root.gameObject.SetActive(false);
        }
    }
}
