using System;
using System.Collections;
using Gavi.Characters;
using Gavi.Skills;
using UnityEngine;
namespace Gavi.Player.Skills
{
    public class KeyBinding : UiTarget
    {
        [SerializeField] private KeyCode _keyCodePrimary;
        [SerializeField] private KeyCode _keyCodeSecondary;
        [SerializeField] private TMPro.TextMeshProUGUI _textHotKey;
        
        private float _lastPressed;
        private const float DELAYED_INPUT_THRESHOLD = 0.4f;

        
        #region Mono
        private void Awake()
        {
            UpdateHotKeyText();
        }
        #endregion


        #region Pressing KeyCode
        public bool IsPressed()
        {
            bool isPressedDelayed = Simulation.Time < _lastPressed + DELAYED_INPUT_THRESHOLD;

            bool isPressed = Input.GetKeyDown(_keyCodePrimary);
            if (_keyCodeSecondary == KeyCode.None)
            {
                if (GetSecondaryHold() != KeyCode.None)
                    isPressed = false;
            }
            else if (GetSecondaryHold() != _keyCodeSecondary)
                isPressed = false;
            
            if (isPressed)
                _lastPressed = Simulation.Time;

            return isPressed || isPressedDelayed;
        }

        public void OnSucessfullyActivated()
        {
            _lastPressed = 0;
        }
        #endregion


        #region Change KeyCode
        public override void OnMouseHover()
        {
            base.OnMouseHover();
            CheckChangeKeyCode();
        }

        private void CheckChangeKeyCode()
        {
            if (!PlayerSkillConfiguration.Instance.IsInConfigurationMenu)
                return;

            KeyCode primary = GetPrimaryUp();
            if (primary == KeyCode.None)
                return;
            KeyCode secondary = GetSecondaryHold();

            _keyCodePrimary = primary;
            _keyCodeSecondary = secondary;
            
            UpdateHotKeyText();
        }

        public void CopyKeyCodeFrom(KeyBinding other)
        {
            if (other == null)
                return;
            _keyCodePrimary = other._keyCodePrimary;
            _keyCodeSecondary = other._keyCodeSecondary;
            UpdateHotKeyText();
        }

        public void SetKeyCodePrimary(KeyCode primary)
        {
            _keyCodePrimary = primary;
            UpdateHotKeyText();
        }

        private KeyCode GetPrimaryUp()
        {
            KeyCode k;
            k = KeyCode.Alpha1; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha2; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha3; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha4; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha5; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha6; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha7; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha8; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha9; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Alpha0; if (Input.GetKeyUp(k)) return k;
            
            k = KeyCode.F1; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F2; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F3; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F4; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F5; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F6; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F7; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F8; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F9; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F10; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F11; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F12; if (Input.GetKeyUp(k)) return k;
            
            k = KeyCode.Q; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.W; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.E; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.R; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.T; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Z; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.U; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.I; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.O; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.P; if (Input.GetKeyUp(k)) return k;
            
            k = KeyCode.A; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.S; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.D; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.F; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.G; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.H; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.J; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.K; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.L; if (Input.GetKeyUp(k)) return k;
            
            k = KeyCode.Y; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.X; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.C; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.V; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.B; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.N; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.M; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Comma; if (Input.GetKeyUp(k)) return k;
            k = KeyCode.Period; if (Input.GetKeyUp(k)) return k;
            
            k = KeyCode.Caret; if (Input.GetKeyUp(k)) return k;
            return KeyCode.None;
        }

        private KeyCode GetSecondaryHold()
        {
            KeyCode k;
            k = KeyCode.LeftShift; if (Input.GetKey(k)) return k;
            k = KeyCode.LeftControl; if (Input.GetKey(k)) return k;
            k = KeyCode.LeftAlt; if (Input.GetKey(k)) return k;
            return KeyCode.None;
        }
        #endregion
        
        
        #region Misc
        public string GetKeyCodeString()
        {
            if (_keyCodePrimary == KeyCode.None)
                return "";
                
            string keyCodeText = "";
            
            if (_keyCodeSecondary == KeyCode.LeftShift)
                keyCodeText += "shift + ";
            if (_keyCodeSecondary == KeyCode.LeftControl)
                keyCodeText += "ctrl + ";
            if (_keyCodeSecondary == KeyCode.LeftAlt)
                keyCodeText += "alt + ";
                
            
            if (_keyCodePrimary == KeyCode.Caret)
                keyCodeText += "^";
            else if (_keyCodePrimary == KeyCode.Comma)
                keyCodeText += ",";
            else if (_keyCodePrimary == KeyCode.Period)
                keyCodeText += ".";
            else
                keyCodeText += _keyCodePrimary.ToString();
            keyCodeText = keyCodeText.Replace("Alpha", "");

            
            return keyCodeText;
        }

        private void UpdateHotKeyText()
        {
            if (_textHotKey != null)
                _textHotKey.text = GetKeyCodeString();
        }
        #endregion
        
        
    }
}