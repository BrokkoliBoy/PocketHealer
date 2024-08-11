using Gavi.Skills;
using Gavi.Utility;
using UnityEngine;

namespace Gavi
{
    public class PlayerSkillPrefab : MonoBehaviour
    {
        [SerializeField] private bool _generateGuid;
        public string GUID => _guid;
        [SerializeField] private string _guid;
        public Skill Skill => _skill;
        [SerializeField] private Skill _skill;
        
        private void OnValidate()
        {
            if (_generateGuid && _guid == "")
            {
                _generateGuid = false;
                _guid = System.Guid.NewGuid().ToString();
            }
        }
    }
}

