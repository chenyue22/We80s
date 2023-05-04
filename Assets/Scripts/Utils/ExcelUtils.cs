using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using We80s.GameData;

namespace We80s.Utils
{
    public static class ExcelUtils
    {
        private static readonly Type[] ExcelDataTypes = new[]
        {
            typeof(double),
            typeof(string),
            typeof(int),
        };
        
        public static byte DataTypeToByte(Type type)
        {
            for (int i = 0; i < ExcelDataTypes.Length; ++i)
            {
                if (type == ExcelDataTypes[i]) return (byte)i;
            }

            return 255;
        }

        unsafe public static BinaryExcelInfoHeader DecodeInfoHeader(byte* fileBytes)
        {
            long* sheetRangePtr = (long*) UnsafeUtility.Malloc(BinaryExcelInfoHeader.MaxSheetCount * 2 * sizeof(long),
                sizeof(int), Allocator.Temp);
            UnsafeUtility.MemCpy(sheetRangePtr, fileBytes + sizeof(long),
                BinaryExcelInfoHeader.MaxSheetCount * 2 * sizeof(long));
            
            BinaryExcelInfoHeader excelInfoHeader = new BinaryExcelInfoHeader();
            BinaryExcelInfoHeader* excelInfoHeaderPtr = &excelInfoHeader;
            UnsafeUtility.MemCpy(excelInfoHeaderPtr, fileBytes, sizeof(int) + sizeof(long));
            excelInfoHeader.sheetRange = sheetRangePtr;
            return excelInfoHeader;
        }

        unsafe public static BinaryExcelSheetHeader DecodeSheetHeader(byte* ptr)
        {
            BinaryExcelSheetHeader sheetHeader = new BinaryExcelSheetHeader();
            BinaryExcelSheetHeader* sheetHeaderPtr = &sheetHeader;
            UnsafeUtility.MemCpy(sheetHeaderPtr,ptr, sizeof(int));
            UnsafeUtility.MemCpy((byte*) sheetHeaderPtr + sizeof(int),ptr + sizeof(int), 1);
            byte* rowTypePtr = (byte*) UnsafeUtility.Malloc(BinaryExcelSheetHeader.MaxColumnCount, 1, Allocator.Temp);
            UnsafeUtility.MemCpy(rowTypePtr, ptr + sizeof(int) + 1, BinaryExcelSheetHeader.MaxColumnCount);
            sheetHeader.rowType = rowTypePtr;
            return sheetHeader;
        }
    }   
}