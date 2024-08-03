using System;
using System.Collections;
using System.Collections.Generic;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class HoverInfoTargetSkill : HoverInfoTarget
    {
        [SerializeField] private SkillUi _skillUi;
        // [SerializeField] private HoverInfoPanelSkill _infoPanelPrefab;
        
        private InfoPanelSkill InfoPanel
        {
            get
            {
                if (_panel == null)
                {
                    _panel = Instantiate(HoverInfoMaster.PrefabPanelSkill, HoverInfoMaster.ParentHoverInfoPanel);
                    _panel.Root.gameObject.SetActive(false);
                }
                return _panel;
            }
        }

        private static InfoPanelSkill _panel;
        

        protected override void ShowInfoPanel()
        {
            base.ShowInfoPanel();
            if (!InfoPanel.IsShowing)
                InfoPanel.ShowPanel(_skillUi.Skill);
        }

        public override void OnMouseHover()
        {
            base.OnMouseHover();
            Vector2 mousePosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIMaster.Mastercanvas.GetComponent<RectTransform>(), (Vector2) Input.mousePosition, UIMaster.Camera, out mousePosition);
            Vector2 targetPositon = mousePosition;
            Rect rootRect = InfoPanel.Root.rect;
            targetPositon.x -= rootRect.width * 0.5f * (mousePosition.x > 0 ? 1 : -1);
            targetPositon.y -= rootRect.height * 0.5f * (mousePosition.y > 0 ? 1 : -1);
            InfoPanel.Root.position = UIMaster.Mastercanvas.transform.TransformPoint(targetPositon);
        }

        protected override void HideInfoPanel()
        {
            base.HideInfoPanel();
            InfoPanel.HidePanel();
        }
    }
}
