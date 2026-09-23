using UnityEngine;

namespace Gavi
{
    public class DebugModeButtonVisibility : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(DebugMode.Instance.EnableDebugModeButton);
        }
    }
}
