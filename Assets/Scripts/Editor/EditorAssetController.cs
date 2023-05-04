using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace We80s.Editor
{
    public class EditorAssetController
    {
        [MenuItem("Assets/Copy Sprite Datas", validate = true)]
        private static bool InvalidCopySpriteDatas()
        {
            if (Selection.objects == null || Selection.objects.Length != 1)
            {
                return false;
            }

            var obj = Selection.objects[0] as Texture2D;
            return obj != null;
        }
        
        [MenuItem("Assets/Copy Sprite Datas")]
        private static void CopySpriteDatas()
        {
            var path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            var objects = AssetDatabase.LoadAllAssetsAtPath(path);

            string buffer = "";
            for (int i = 1; i < objects.Length; ++i)
            {
                var sprite = objects[i] as Sprite;
                buffer += '\t';
                buffer += '\t';
                var rect = sprite.rect;
                buffer += rect.x.ToString() + '|';
                buffer += rect.y.ToString() + '|';
                buffer += rect.width.ToString() + '|';
                buffer += rect.height.ToString() + '|';
                buffer += "\r\n";
            }

            GUIUtility.systemCopyBuffer = buffer;
        }
    }
   
}