using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using CyRayTracingSystem.Utils;
using OfficeOpenXml;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using We80s.GameActor;
using We80s.GameData;
using We80s.GameData.Table;
using We80s.GameEvent;

namespace We80s.Editor.GameData
{
    public static class ExcelWriter
    {
        private static class Attributes
        {
            internal static Dictionary<string, PlayerAttributeType> playerAttributeTypeLut = new Dictionary<string, PlayerAttributeType>
            {
                {"iq", PlayerAttributeType.IQ},
                {"eq", PlayerAttributeType.EQ},
                {"app", PlayerAttributeType.Appearance},
                {"face", PlayerAttributeType.Face},
                {"hap", PlayerAttributeType.Happiness},
                {"hea", PlayerAttributeType.Health},
                {"mem", PlayerAttributeType.Memory},
                {"pat", PlayerAttributeType.Patience},
                {"vit", PlayerAttributeType.Vitality},
                {"voi", PlayerAttributeType.Voice},
            };

            internal static string moneyAttribute = "money";
            internal static string eventAttribute = "evt";
            
            internal static Dictionary<string, EventModuleType> eventModuleTypeLut = new Dictionary<string, EventModuleType>
            {
                {"dia", EventModuleType.Dialog}, 
                {"cho", EventModuleType.MakeAChoice}, 
                {"sto", EventModuleType.PlayAStory}, 
                {"mg", EventModuleType.MiniGame}, 
            };
        }

        unsafe private static void WriteInt(DynamicCyPtr<Byte>* bytes, int value)
        {
            int* xPtr = &value;
            bytes->Add((byte*) xPtr, 4);
        }

        unsafe private static void WriteInt(DynamicCyPtr<Byte>* bytes, object value)
        {
            int x = Convert.ToInt32(value);
            int* xPtr = &x;
            bytes->Add((byte*) xPtr, 4);
        }
        
        unsafe private static void WriteInt2(DynamicCyPtr<Byte>* bytes, int x, int y)
        {
            int* xPtr = &x;
            int* yPtr = &y;
            bytes->Add((byte*) xPtr, 4);
            bytes->Add((byte*) yPtr, 4);
        }

        unsafe private static void WriteInt2(DynamicCyPtr<Byte>* bytes, object value)
        {
            var str = (string) value;
            int x = 0;
            int* xPtr = &x;
            string[] components = str.Split('|');
            for (int i = 0; i < 2; ++i)
            {
                if (i >= components.Length || !int.TryParse(components[i], out x))
                {
                    x = 0;
                }

                bytes->Add((byte*) xPtr, 4);
            }
        }
        
        unsafe private static void WriteInt3(DynamicCyPtr<Byte>* bytes, Vector3Int v)
        {
            Vector3Int* xPtr = &v;
            bytes->Add((byte*) xPtr, 12);
        }
        
        unsafe private static void WriteInt3(DynamicCyPtr<Byte>* bytes, object value)
        {
            var str = (string) value;
            int x = 0;
            int* xPtr = &x;
            string[] components = str.Split('|');
            for (int i = 0; i < 3; ++i)
            {
                if (i >= components.Length || !int.TryParse(components[i], out x))
                {
                    x = 0;
                }

                bytes->Add((byte*) xPtr, 4);
            }
        }
        
        unsafe private static void WriteInt4(DynamicCyPtr<Byte>* bytes, object value)
        {
            var str = (string) value;
            int x = 0;
            int* xPtr = &x;
            string[] components = str.Split('|');
            for (int i = 0; i < 4; ++i)
            {
                if (i >= components.Length || !int.TryParse(components[i], out x))
                {
                    x = 0;
                }

                bytes->Add((byte*) xPtr, 4);
            }
        }

        unsafe private static void WriteString(DynamicCyPtr<Byte>* bytes, object value)
        {
            var str = (string) value;
            var strBytes = (byte*) Marshal.StringToHGlobalAnsi(str);
            int n = Encoding.Default.GetByteCount(str);
            int* nPtr = &n;
            bytes->Add((byte*) nPtr, sizeof(int));
            bytes->Add(strBytes, n);
        }

        unsafe private static void WriteArray<T>(DynamicCyPtr<Byte>* bytes, object value) where T : unmanaged
        {
            var array = (T[]) value;
            CyPtr<T> ptr = new CyPtr<T>(array);
            int n = ptr.Length;
            int* nPtr = &n;
            bytes->Add((byte*) nPtr, sizeof(int));
            bytes->Add((byte*) ptr.Ptr, n * sizeof(T));
        }
        
        //0 - playerAttribute
        //1 - money
        //2 - event
        //3 - eventModule
        private static bool TryParseExpression(string str, out Vector3Int expression)
        {
            var splited = str.Split(':');
            if (splited.Length == 2)
            {
                int iValue = 0;
                if (!int.TryParse(splited[1], out iValue))
                {
                    expression = default;
                    return false;
                }
                
                PlayerAttributeType playerAttributeType;
                if (Attributes.playerAttributeTypeLut.TryGetValue(splited[0], out playerAttributeType))
                {
                    expression = new Vector3Int(0, (int) playerAttributeType, iValue);
                    return true;
                }

                if (splited[0] == Attributes.moneyAttribute)
                {
                    expression = new Vector3Int(1, iValue, 0);
                    return true;
                }

                if (splited[0] == Attributes.eventAttribute)
                {
                    expression = new Vector3Int(2, iValue, 0);
                    return true;
                }

                EventModuleType eventModuleType;
                if (Attributes.eventModuleTypeLut.TryGetValue(splited[0], out eventModuleType))
                {
                    expression = new Vector3Int(3, (int) eventModuleType, iValue);
                    return true;
                }
            }
            
            expression = default;
            return false;
        }

        unsafe private static void WriteChooseColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            for (int i = 2; i <= worksheet.Dimension.End.Column; ++i)
            {
                var str = (string) worksheet.GetValue(row, i);
                var splited = str.Split('|');
                if (splited.Length > 0)
                {
                    WriteInt(bytes, splited.Length);
                    WriteString(bytes, splited[0]);
                    for (int j = 1; j < splited.Length; ++j)
                    {
                        Vector3Int expression;
                        if (TryParseExpression(splited[j], out expression))
                        {
                            WriteInt3(bytes, expression);   
                        }
                    }
                }
            }
        }
        
        unsafe private static void WriteEventColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            for (int i = 2; i <= worksheet.Dimension.End.Column; ++i)
            {
                WriteInt2(bytes, worksheet.GetValue(row, i));
            }
        }
        
        unsafe private static void WriteDialogColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            List<Speach> speachList = new List<Speach>();
            for (int i = 2; i <= worksheet.Dimension.End.Column; ++i)
            {
                var value = worksheet.GetValue(row, i);
                if (value == null)
                {
                    return;
                }

                var splitted = ((string) value).Split('|');
                if (splitted.Length == 2)
                {
                    int speakerId;
                    if (int.TryParse(splitted[0], out speakerId) && !string.IsNullOrEmpty(splitted[1]))
                    {
                        speachList.Add(new Speach(speakerId, splitted[1]));
                    }
                }
            }

            if (speachList.Count > 0)
            {
                WriteInt(bytes, worksheet.GetValue(row, 1));
                WriteInt(bytes, speachList.Count);
                foreach (var speach in speachList)
                {
                    WriteInt(bytes, speach.speakerId);
                    WriteString(bytes, speach.content);
                }
            }
        }

        unsafe private static void WriteUIColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            WriteString(bytes, worksheet.GetValue(row, 2));
        }
        
        unsafe private static void WriteTextureColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            WriteString(bytes, worksheet.GetValue(row, 2));
        }
        
        unsafe private static void WriteSpriteColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            WriteInt(bytes, worksheet.GetValue(row, 2));
            WriteInt4(bytes, worksheet.GetValue(row, 3));
        }
        
        unsafe private static void WriteGameObjectColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            WriteString(bytes, worksheet.GetValue(row, 2));
        }        
        
        unsafe private static void WriteMaterialColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            WriteString(bytes, worksheet.GetValue(row, 2));
        }   
        
        unsafe private static void WriteMeshColumn(DynamicCyPtr<Byte>* bytes, ExcelWorksheet worksheet, int row)
        {
            WriteInt(bytes, worksheet.GetValue(row, 1));
            WriteString(bytes, worksheet.GetValue(row, 2));
        }   
        
        unsafe public static void WriteTableBinaryData(TableType tableType, string absFilePath, string outputAbsFilePath)
        {
            var file = new FileInfo(absFilePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                BinaryExcelInfoHeader infoHeader = new BinaryExcelInfoHeader();
                infoHeader.tableType = (int) tableType;
                infoHeader.sheetCount = (byte) package.Workbook.Worksheets.Count;
                long* range = (long*) UnsafeUtility.Malloc(BinaryExcelInfoHeader.MaxSheetCount * sizeof(long) * 2, sizeof(long), Allocator.Temp);
                UnsafeUtility.MemSet(range, 0, BinaryExcelInfoHeader.MaxSheetCount * sizeof(long) * 2);
                
                DynamicCyPtr<byte> output = new DynamicCyPtr<byte>();
                DynamicCyPtr<byte> sheetHeaderBytes = new DynamicCyPtr<byte>();
                DynamicCyPtr<byte> sheetBodyBytes = new DynamicCyPtr<byte>();

                for (int s = 1; s <= BinaryExcelSheetHeader.MaxColumnCount && s <= package.Workbook.Worksheets.Count; ++s)
                {
                    int start = sheetBodyBytes.Count;
                    range[(s - 1) * 2] = start;
                    
                    var sheet = package.Workbook.Worksheets[s];
                    if (sheet.Dimension == null) continue;
                    BinaryExcelSheetHeader sheetHeader = new BinaryExcelSheetHeader();
                    sheetHeader.row = sheet.Dimension.End.Row;
                    sheetHeader.collumn = (byte) Mathf.Min(BinaryExcelSheetHeader.MaxColumnCount, sheet.Dimension.End.Column);
                    byte* rowType = (byte*) UnsafeUtility.Malloc(256, 1, Allocator.Temp);
                    rowType[0] = 2; //int
                    rowType[1] = 1; //string
                    sheetHeader.rowType = rowType;

                    int n = 0;
                    byte* shb = sheetHeader.ToBytes(out n);
                    sheetHeaderBytes.Add(shb, n);
                    
                    for (int j = 1; j <= sheet.Dimension.End.Row; ++j)
                    {
                        switch (tableType)
                        {
                            case TableType.Event:
                                WriteEventColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.Dialog:
                                WriteDialogColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.Choose:
                                WriteChooseColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.UI:
                                WriteUIColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.Texture:
                                WriteTextureColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.Sprite:
                                WriteSpriteColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.GameObject:
                                WriteGameObjectColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.Material:
                                WriteMaterialColumn(&sheetBodyBytes, sheet, j);
                                break;
                            case TableType.Mesh:
                                WriteMeshColumn(&sheetBodyBytes, sheet, j);
                                break;
                        }
                    }
                    
                    int nBodyBytes = sheetBodyBytes.Count - start;
                    range[(s - 1) * 2 + 1] = nBodyBytes;
                }

                infoHeader.sheetRange = range;
                int nInfoHeaderBytes = 0;
                output.Add(infoHeader.ToBytes(out nInfoHeaderBytes), nInfoHeaderBytes);
                output.Add(sheetHeaderBytes.Ptr, sheetHeaderBytes.Count);
                output.Add(sheetBodyBytes.Ptr, sheetBodyBytes.Count);
                
                sheetHeaderBytes.Free();
                sheetBodyBytes.Free();

                var outputFolder = outputAbsFilePath.Substring(0,outputAbsFilePath.LastIndexOf('\\'));
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                var outputFile = File.Open(outputAbsFilePath, FileMode.OpenOrCreate);
                BinaryWriter bw = new BinaryWriter(outputFile);
                bw.Write(output.ToArray());
                outputFile.Close();
                output.Free();
            }
        }
    }   
}