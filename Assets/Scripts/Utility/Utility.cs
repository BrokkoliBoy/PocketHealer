using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gavi.Utility
{
    public class Utility
    {
        public static float MapValuePercent(float minValue, float maxValue, float value)
        {
            if (minValue == maxValue)
                return 0.5f;

            if (minValue > maxValue)
            {
                Debug.Log("Warning: minValue > maxValue! Values have been swapped!");
                float min = minValue;
                minValue = maxValue;
                maxValue = min;
            }

            float v = 0;

            float range = maxValue - minValue;
            v = (value - minValue) / range;

            return v;
        }
        public static Component GetComponentInParents<T>(Transform startTransform) where T : Component
        {
            Transform currentTransform = startTransform;
            Component component = currentTransform.GetComponent(typeof(T));
            while (currentTransform != null && component == null)
            {
                currentTransform = currentTransform.parent;
                if (currentTransform == null)
                    continue;

                component = currentTransform.GetComponent(typeof(T));
            }

            return component;
        }
        public static int ExpInt(int b, int e)
        {
            if (e == 0)
                return 1;
            else if (e < 0)
            {
                Debug.Log("Warning: ExpInt doesn't take negative exponentials at the moment!");
                return b;
            }

            int result = b;
            for (int i = 0; i < e - 1; i++)
            {
                result *= b;
            }

            return result;
        }
        public static float ExpInt(float b, int e)
        {
            if (e == 0)
                return 1;
            else if (e < 0)
            {
                Debug.Log("Warning: ExpInt doesn't take negative exponentials at the moment!");
                return b;
            }

            float result = b;
            for (int i = 0; i < e - 1; i++)
            {
                result *= b;
            }

            return result;
        }
        public static int IntToLayerMask(int layer)
        {
            return ExpInt(2, layer);
        }
        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }
        public static Vector3 RandomVector(Vector3 min, Vector3 max)
        {
            Vector3 vector = Vector3.zero;
            vector.x = Random.Range(min.x, max.x);
            vector.y = Random.Range(min.y, max.y);
            vector.z = Random.Range(min.z, max.z);
            return vector;
        }

        public static Vector3 MultiplyVector3(Vector3 v0, Vector3 v1)
        {
            return new Vector3(v0.x * v1.x, v0.y * v1.y, v0.z * v1.z);
        }
        public static Vector3 DivideVector3(Vector3 divisor, Vector3 denominator)
        {
            if (denominator.x == 0) denominator.x = 1;
            if (denominator.y == 0) denominator.y = 1;
            if (denominator.z == 0) denominator.z = 1;

            return new Vector3(divisor.x / denominator.x, divisor.y / denominator.y, divisor.z / denominator.z);
        }
        public static Vector3 ScaleVector(Vector3 vector, float scale)
        {
            vector.x *= scale;
            vector.y *= scale;
            vector.z *= scale;
            return vector;
        }

        public static float Remap(float oldMin, float oldMax, float newMin, float newMax, float value)
        {
            return newMin + (newMax - newMin) * ((value - oldMin) / (oldMax - oldMin));
        }

        public static float GetFpsEqualizer()
        {
            return Application.targetFrameRate * Time.deltaTime;
        }

        //        public static string GetAssetPath(TextAsset data)
        //        {
        //            if (!Application.isEditor)
        //                return null;

        //            string path = "";
        //#if UNITY_EDITOR
        //            path = Path.GetFullPath(Application.dataPath);
        //            path = path.Substring(0, path.Length - 6) + UnityEditor.AssetDatabase.GetAssetPath(data);
        //#endif

        //            return path;

        //        }

        public static float getAngle(Vector3 v1, Vector3 v2)
        {
            float dot = Vector3.Dot(v1.normalized, v2.normalized);
            float cos = Mathf.Acos(dot) * 180 / Mathf.PI;
            return cos;
        }

        public static string SecondsToMinutes(float time, int decimals = 0)
        {
            float seconds = (int)(time % 60);
            string secondsString = (int)seconds + "";
            float minutes = time / 60f;
            string minutesString = minutes < 1 ? "" : ((int)minutes + ":");

            // 0.0
            // 00.0
            if (time < 10)
            {
                string format = "{0:0.0}";
                secondsString = String.Format(format, time);
            }
            else if (time > 60)
            {
                string format = "{0:00}";
                secondsString = String.Format(format, seconds);
            }

            return minutesString + secondsString;
        }

        public static string GetHierarchy(Transform transform)
        {
            if (transform.parent == null)
                return "";

            return GetHierarchy(transform.parent) + " -> " + transform.gameObject.name;

        }
    }
}