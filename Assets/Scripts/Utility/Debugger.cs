using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Gavi.Utility
{
    public class Debugger : MonoBehaviour
    {
        [Header("--- References ---")]
        [SerializeField] private int _numberShowMessages = 10;
        //[SerializeField] private TextMeshProUGUI _textChat;
        [SerializeField] private TextMeshProUGUI _textError;
        [SerializeField] private TextMeshProUGUI _textInfo;
        [SerializeField] private TextMeshProUGUI _textAssertionFail;

        [SerializeField] private KeyCode _keyCodeShowLog;
        [SerializeField] private GameObject _logObject;

        public static Debugger Instance;

        //private List<string> _messagesChat;
        private List<string> _messsageError = new List<string>();
        private List<string> _messagesInfo = new List<string>();
        private List<string> _messageesAssertionFail = new List<string>();

        private Queue<int> _indicesInfo = new Queue<int>();
        private Queue<int> _indicesError = new Queue<int>();
        private Queue<int> _indicesAssertionFail = new Queue<int>();

        private int _currnetMessageNumberInfo;
        private int _currnetMessageNumberError;
        private int _currnetMessageNumberAssertionFail;

        private void Awake()
        {
            Instance = this;
            if (_textInfo != null)
                _textInfo.text = "";
            if (_textError != null)
                _textError.text = "";
            if (_textAssertionFail != null)
                _textAssertionFail.text = "";
        }

        private void Update()
        {
            if (Input.GetKeyDown(_keyCodeShowLog))
                ToggleLog();
        }

        private void ToggleLog()
        {
            if (_logObject == null)
                return;
            _logObject.gameObject.SetActive(!_logObject.activeSelf);
        }

        #region Logging
        public static void LogInfo(object o, Object context = null)
        {
            if(Instance == null)
            {
                Debug.Log(o);
                return;
            }
            Instance.AddTextInfo(o.ToString());

#if UNITY_EDITOR
            Debug.Log(o.ToString());
#endif
        }
        public static void LogError(object o, Object context = null)
        {
            if (Instance == null)
            {
                Debug.Log(o);
                return;
            }
            Instance.AddTextError(o.ToString());

#if UNITY_EDITOR
            Debug.LogError("Error: " + o.ToString());
#endif
        }
        public static void LogAssertionFail(object o, Object context = null)
        {
            if (Instance == null)
            {
                Debug.Log(o);
                return;
            }
            Instance.AddTextAssertionFail(o.ToString());

#if UNITY_EDITOR
            Debug.LogError("AssertionFail: " + o.ToString());
#endif
        }
        
        public static void LogInstanceError(System.Type type)
        {
            if (Instance == null)
            {
                Debug.Log(type.ToString());
                return;
            }
            LogError("Instance already exists: " + type.ToString());
        }
        #endregion

        #region Display Control
        private void AddTextInfo(string text)
        {
            if (_textInfo == null)
                return;

            if (text == "")
                return;

            string preText = _currnetMessageNumberInfo.ToString();
            _currnetMessageNumberInfo++;

            int addLength = text.Length + 2 + preText.Length + 2;
            int removeLength = 1;

            if (_indicesInfo.Count >= _numberShowMessages)
                removeLength = _indicesInfo.Dequeue();
            _indicesInfo.Enqueue(addLength);
            _textInfo.text = _textInfo.text.Substring(removeLength - 1);
            _textInfo.text += preText + ": " + text + "\n";
        }
        private void AddTextError(string text)
        {
            if (_textError == null)
                return;

            if (text == "")
                return;

            string preText = _currnetMessageNumberError.ToString();
            _currnetMessageNumberError++;

            int addLength = text.Length + 2 + preText.Length + 2;
            int removeLength = 1;

            if (_indicesError.Count >= _numberShowMessages)
                removeLength = _indicesError.Dequeue();
            _indicesError.Enqueue(addLength);
            _textError.text = _textError.text.Substring(removeLength - 1);
            _textError.text += preText + ": " + text + "\n";
        }
        private void AddTextAssertionFail(string text)
        {
            if (_textAssertionFail == null)
                return;

            if (text == "")
                return;

            string preText = _currnetMessageNumberAssertionFail.ToString();
            _currnetMessageNumberAssertionFail++;

            int addLength = text.Length + 2 + preText.Length + 2;
            int removeLength = 1;

            if (_indicesAssertionFail.Count >= _numberShowMessages)
                removeLength = _indicesAssertionFail.Dequeue();
            _indicesAssertionFail.Enqueue(addLength);
            _textAssertionFail.text = _textAssertionFail.text.Substring(removeLength - 1);
            _textAssertionFail.text += preText + ": " + text + "\n";
        }
        #endregion
    }
}