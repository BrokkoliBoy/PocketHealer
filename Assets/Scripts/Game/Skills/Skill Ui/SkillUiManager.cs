using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gavi.Characters;

namespace Gavi.Skills
{
    public class SkillUiManager : MonoBehaviour
    {
        // [SerializeField] private float _padding = 3.5f;

        [Header("- References -")]
        [SerializeField] protected SkillBar _skillBar;
        private Character _character;

        private Dictionary<Skill, SkillUi> _uis = new Dictionary<Skill, SkillUi>();


        #region Mono
        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        private void Start()
        {
            if (_character != null)
                _character.OnDied.AddListener(OnDied);
            if (_skillBar != null)
                _skillBar.Initialize();
        }

        private void OnDestroy()
        {
            if (_character != null)
                _character.OnDied.RemoveListener(OnDied);
        }
        #endregion


        #region Life Cycle
        public virtual void AddUi(Skill skill, int skillIndex = -1) 
        {
            if (skill.DontUseUi)
                return;

            if (_uis.ContainsKey(skill))
            {
                Debug.LogError("_uis.ContainsKey(skill) in SkillUiManager.AddUi(...) on " + gameObject.name);
                return;
            }
            
            _skillBar.AddSkillManually(skill, skillIndex);
            SkillUi ui = skill.UI;
            _uis.Add(skill, ui);
            // UpdateUi();
        }

        public void RemoveUi(Skill skill)
        {
            if (!_uis.ContainsKey(skill))
            {
                Debug.LogError("!_uis.ContainsKey(skill) in SkillUiManager.RemoveUi(...) on " + gameObject.name);
                return;
            }

            _uis.Remove(skill);
            _skillBar.EjectZone(skill);
            Destroy(skill.UI.gameObject);
        }

        private void OnDied()
        {
            List<Skill> skills = new List<Skill>();
            foreach (KeyValuePair<Skill, SkillUi> pair in _uis)
                skills.Add(pair.Key);
            for (int i = skills.Count - 1; i >= 0; i--)
                RemoveUi(skills[i]);
            _uis.Clear();
        }
        #endregion
    }
}
