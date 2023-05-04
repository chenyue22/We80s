using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace We80s.Utils
{
    public static class GameUtils
    {
        public static void SafeRelease(this Object o)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(o);
#else
            GameObject.Destroy(o);
#endif
        }

        public static string AssetPathToAbsPath(string assetPath)
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - 6) + assetPath;
        }
    }
}