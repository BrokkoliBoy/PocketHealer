using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Characters
{
    public class PartyUi : MonoBehaviour
    {
        public static PartyUi Instance;

        #region Serialized Variables
        [SerializeField] private float _width = 1250;
        [SerializeField] private Vector2 _padding = new Vector2(350, 100);

        [SerializeField] private bool _updateEveryFrame;
        #endregion

        
        #region Non-Serialized Variables
        private List<CharacterUi> _characterUis = new List<CharacterUi>();
        #endregion

        
        #region Mono
        private void Awake()
        {
            if (Instance != null)
                Utility.Debugger.LogInstanceError(typeof(PartyUi));
            Instance = this;
        }

        private void Update()
        {
            if(_updateEveryFrame)
                UpdateUi();
        }
        #endregion

        
        private void UpdateUi()
        {
            // ClearPartyUi();
            AllignCharacters(Party.Instance.Members);
        }

        public void AllignCharacters(List<Character> characters)
        {
            ClearPartyUi();
            foreach (Character character in characters)
                AddCharacter(character);
        }
        
        public void ClearPartyUi()
        {
            for (int i = _characterUis.Count - 1; i >= 0; i--)
                _characterUis[i].DestroyUi();
            _characterUis.Clear();
        }


        public void AddCharacter(Character character)
        {
            CharacterUi ui = character.Ui;
            _characterUis.Add(ui);
            AlignCharacter(ui, _characterUis.Count - 1);
        }

        private void AlignCharacter(CharacterUi ui, int index)
        {
            int uisPerRow = (int)(_width / _padding.x);
            float positionX = (index % uisPerRow) * _padding.x;
            float positionY = index / uisPerRow * _padding.y;

            ui.RootTransform.localPosition = new Vector3(positionX, -positionY, 0);
        }
    }
}