using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Characters
{
    public class FloatingText : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI _textField;
        private FloatingTextManager _manager;

        public Color Color
        {
            set => _textField.color = value;
        }


        public string Text
        {
            set => _textField.text = value;
        }



        private void Awake()
        {
            _textField = GetComponent<TMPro.TextMeshProUGUI>();
        }

        public void Initialize(FloatingTextManager manager)
        {
            _manager = manager;
            StartCoroutine(UpdateText());
        }

        private IEnumerator UpdateText()
        {
            float lifeTime = 2f;
            float timeFadeOut = 1.5f;
            Vector3 movement = new Vector3(0, 3, 0);

            float timeElapsed = 0;
            while (timeElapsed < lifeTime)
            {
                _textField.color = new Color(_textField.color.r, _textField.color.g, _textField.color.b, (lifeTime - timeElapsed) * (lifeTime / timeFadeOut));
                transform.position += movement * Simulation.DeltaTime;
                timeElapsed += Simulation.DeltaTime;
                yield return null;
            }

            _manager.ReturnText(this);
        }
    }
}