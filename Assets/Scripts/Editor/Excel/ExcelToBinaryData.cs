using System.IO;
using UnityEditor;
using We80s.GameData.Table;
using We80s.Managers;
using We80s.Utils;

namespace We80s.Editor.GameData
{
    public class ExcelToBinaryData
    {
        private const string RawExcelPath = "Assets\\Res\\Excel\\Raw";
        private const string BinaryExcelPath = "Assets\\Res\\Excel\\Binary";

        private static string[] tableNames;

        private static string RawPathToBinaryPath(string rawPath)
        {
            var binPath = rawPath.Replace(RawExcelPath, BinaryExcelPath);
            if (binPath.EndsWith(".xlsx"))
            {
                binPath = binPath.Substring(0, binPath.Length - 5) + ".bytes";
            }

            return binPath;
        }

        private static void ReqZip(DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles())
            {
                if (file.Extension == ".meta") continue;
                var binFile = RawPathToBinaryPath(file.FullName);
                if (File.Exists(binFile))
                {
                    File.Delete(binFile);
                }

                TableType tableType = TableType.Count;
                for (int i = 0; i < tableNames.Length; ++i)
                {
                    if (file.Name == tableNames[i] + ".xlsx")
                    {
                        tableType = (TableType) i;
                        break;
                    }
                }
                
                if (tableType != TableType.Count)
                    ExcelWriter.WriteTableBinaryData(tableType, file.FullName, binFile);
            }

            foreach (var d in dir.GetDirectories())
            {
                ReqZip(d);
            }
        }

        [MenuItem("Game/Data/Zip Excels")]
        public static void ZipExcels()
        {
            if (tableNames == null)
            {
                tableNames = new string[(int) TableType.Count];
                tableNames[(int) TableType.Event] = "Event";
                tableNames[(int) TableType.Dialog] = "Dialog";
                tableNames[(int) TableType.Choose] = "Choose";
                tableNames[(int) TableType.UI] = "UI";
                tableNames[(int) TableType.Texture] = "Texture";
                tableNames[(int) TableType.Sprite] = "Sprite";
                tableNames[(int) TableType.GameObject] = "GameObject";
                tableNames[(int) TableType.Material] = "Material";
                tableNames[(int) TableType.Mesh] = "Mesh";
            }
            
            DirectoryInfo dir = new DirectoryInfo(GameUtils.AssetPathToAbsPath(RawExcelPath));
            if (dir.Exists)
            {
                ReqZip(dir);
                AssetDatabase.Refresh();
                AssetManager.Instance.InitAssets();
            }
        }
    }
}