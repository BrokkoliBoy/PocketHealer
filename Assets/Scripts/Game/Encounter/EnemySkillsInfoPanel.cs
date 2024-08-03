using System.Collections;
using System.Collections.Generic;
using Gavi.Characters;
using Gavi.Skills;
using UnityEngine;

namespace Gavi
{
    public class EnemySkillsInfoPanel : MonoBehaviour
    {
        public static EnemySkillsInfoPanel Instance;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _panelsContent;
        [SerializeField] private InfoPanelSkillEncounterInfoPanel _panelPrefab;
        
        private List<InfoPanelSkillEncounterInfoPanel> _panels = new List<InfoPanelSkillEncounterInfoPanel>();
        
        
        #region Mono
        private void Awake()
        {
            Instance = this;
        }
        #endregion

        public void ShowPanel(RectTransform parent)
        {
            SetupInfoPanel(parent);
            FillInfoPanel();
        }

        private void SetupInfoPanel(RectTransform parent)
        {
            _panel.parent = parent;
            _panel.sizeDelta = Vector2.zero;
        }
        
        private void FillInfoPanel()
        {
            foreach (InfoPanelSkillEncounterInfoPanel panel in _panels)
                Destroy(panel.gameObject);
            _panels.Clear();
            foreach (Enemy enemy in Encounter.EncounterManager.Instance.CurrentEncounter.Enemies)
            {
                foreach (Skill skill in enemy.SkillManager.ActiveSkillsPrioSorted)
                {
                    InfoPanelSkillEncounterInfoPanel panel = Instantiate(_panelPrefab, _panelsContent);
                    panel.ShowPanel(skill);
                    _panels.Add(panel);
                }
            }
        }
    }
}
