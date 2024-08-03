using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Control;
using Gavi.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gavi.Control
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance;
        
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private EventSystem _eventSystem;
        
        private List<MouseClick> _clickedDown = new List<MouseClick>();
        private List<MouseHover> _hovered = new List<MouseHover>();
        public List<UiTarget> UiTargets = new List<UiTarget>();
        public List<UiTargetCharacter> CharacterTargets = new List<UiTargetCharacter>();
        public List<UiTargetSkill> SkillTargets = new List<UiTargetSkill>();
        // public List<UiTargetSkill> SkillTargets = new List<UiTargetSkill>();
        
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();


        #region Mono
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Update()
        {
            UpdatePlayerInput();
        }
        #endregion


        #region Update Player Input
        private void UpdatePlayerInput()
        {
            //Set up the new Pointer Event
            _pointerEventData = new PointerEventData(EventSystem.current);
            //Set the Pointer Event Position to that of the mouse position
            _pointerEventData.position = Input.mousePosition;
            //Create a list of Raycast Results
            _raycastResults.Clear();
            //Raycast using the Graphics Raycaster and mouse click position
            EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

            UpdatePlayerClick();
            UpdatePlayerHover();
            UpdateMouseTargets();
            
            CheckPressed();
        }
        
        private void UpdatePlayerClick()
        {
            if (!Input.GetKeyDown(KeyCode.Mouse0))
                return;
            
            _clickedDown.Clear();
            foreach (RaycastResult result in _raycastResults)
            {
                MouseClick mouseClick = result.gameObject.GetComponent<MouseClick>();
                if (mouseClick == null)
                    continue;
                _clickedDown.Add(mouseClick);
                mouseClick.IsClickDown = true;
            }
        }

        private void UpdatePlayerHover()
        {
            _hovered.Clear();
            foreach (RaycastResult result in _raycastResults)
            {
                MouseHover mouseHover = result.gameObject.GetComponent<MouseHover>();
                if (mouseHover == null)
                    continue;
                _hovered.Add(mouseHover);
                mouseHover.IsHover = true;
            }
        }

        public void UpdateMouseTargets()
        {
            UiTargets.Clear();
            CharacterTargets.Clear();
            foreach (RaycastResult result in _raycastResults)
            {
                UiTarget[] uiTargets = result.gameObject.GetComponents<UiTarget>();
                foreach (UiTarget uiTarget in uiTargets)
                {
                    if (uiTarget == null || !uiTarget.IsTargetable)
                        continue;
                    UiTargets.Add(uiTarget);
                    if (uiTarget is UiTargetCharacter character)
                        CharacterTargets.Add(character);
                    else if (uiTarget is UiTargetSkill skill)
                        SkillTargets.Add(skill);
                    
                    uiTarget.OnMouseHover();
                }
            }
        }

        private void CheckPressed()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                foreach (UiTarget uiTarget in UiTargets)
                    uiTarget.OnMousePressedDown();
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                foreach (UiTarget uiTarget in UiTargets)
                    uiTarget.OnMouseHold();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                foreach (UiTarget uiTarget in UiTargets)
                    uiTarget.OnMousePressedUp();
            }
        }
        #endregion
    }
}
