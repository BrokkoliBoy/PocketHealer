using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gavi.Utility
{
    public class QuickDebugger : MonoBehaviour
    {
        public void Log(string s)
        {
            Debugger.LogInfo(s);
        }

        public void Log(float f)
        {
            Debugger.LogInfo(f);
        }
    }
}
