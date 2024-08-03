using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Characters
{
    public class FloatingTextManager : MonoBehaviour
    {
        [SerializeField] private Color _colorHpGain = Color.green;
        [SerializeField] private Color _colorHpLoss = Color.red;
        [SerializeField] private Color _colorBarrierGain = Color.cyan;

        [Tooltip("The parent and position that texts should spawn at.")]
        [SerializeField] private Transform _spawnTransform;
        [SerializeField] private FloatingText _textPrefab;
    

        private List<FloatingText> _textPool = new List<FloatingText>();
        private List<FloatingText> _textsActive = new List<FloatingText>();

        public FloatingText EmitText(string text)
        {
            if (_spawnTransform == null || _textPrefab == null)
                return null;

            FloatingText floatingText = null;
            if (_textPool.Count > 0)
            {
                floatingText = _textPool[0];
                floatingText.gameObject.SetActive(true);
                _textPool.RemoveAt(0);
            }
            else
            {
                floatingText = Instantiate(_textPrefab.gameObject, _spawnTransform).GetComponent<FloatingText>();
            }

            floatingText.Initialize(this);
            floatingText.Text = text;
            floatingText.transform.position = _spawnTransform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
            floatingText.transform.rotation = _spawnTransform.rotation;

            _textsActive.Add(floatingText);
            return floatingText;
        }

        public FloatingText EmitHpChange(float hpChange)
        {
            FloatingText floatingText = EmitText("" + hpChange);
            if (floatingText == null)
                return null;

            if (hpChange >= 0)
                floatingText.Color = _colorHpGain;
            else
                floatingText.Color = _colorHpLoss;

            return floatingText;
        }

        public void EmitBarrierChange(float barrierChange)
        {
            FloatingText floatingText = EmitText("" + barrierChange);
            if (floatingText == null)
                return;
            floatingText.Color = _colorBarrierGain;
        }
        
        public void ReturnText(FloatingText floatingText)
        {
            if (!_textsActive.Contains(floatingText))
            {
                Utility.Debugger.LogAssertionFail("!_textsActive.Contains(floatingText) in FloatingText.ReturnText(...)");
                return;
            }

            _textsActive.Remove(floatingText);
            floatingText.gameObject.SetActive(false);
            _textPool.Add(floatingText);
        }
    }
}
