using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CyRayTracingSystem.Utils;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using We80s.GameActor;
using We80s.Utils;
using We80s.GameData.Table;
using We80s.GameEvent;

namespace We80s.GameData
{
    unsafe public struct BinaryExcelSheetHeader
    {
        public static readonly int MaxColumnCount = 256;
        public static readonly int nTotalBytes = 256 + sizeof(int) + 1;

        public int row;
        public byte collumn;
        public byte* rowType;

        public byte* ToBytes(out int n)
        {
            byte* bytes = (byte*) UnsafeUtility.Malloc(sizeof(int) + 1 + MaxColumnCount, 1, Allocator.Temp);
            int offset = 0;

            fixed (int* rowPtr = &row)
            {
                UnsafeUtility.MemCpy(bytes, rowPtr, sizeof(int));
                offset += sizeof(int);
            }

            fixed (byte* columnPtr = &collumn)
            {
                UnsafeUtility.MemCpy(bytes + offset, columnPtr, sizeof(int));
                offset += 1;
            }

            UnsafeUtility.MemCpy(bytes + offset, rowType, MaxColumnCount);
            n = offset + MaxColumnCount;
            return bytes;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    unsafe public struct BinaryExcelInfoHeader
    {
        public static readonly int MaxSheetCount = 8;
        public static readonly int nTotalBytes = sizeof(long) * (MaxSheetCount * 2 + 1) + sizeof(int);

        [FieldOffset(0)] public int tableType;
        [FieldOffset(4)] public long sheetCount;
        [FieldOffset(12)] public long* sheetRange;

        public byte* ToBytes(out int n)
        {
            n = nTotalBytes;
            byte* bytes = (byte*) UnsafeUtility.Malloc(n, sizeof(long), Allocator.Temp);
            int* intBytes = (int*) bytes;
            intBytes[0] = tableType;
            long* longBytes = (long*) (intBytes + 1);
            longBytes[0] = sheetCount;
            long s1 = sheetRange[0];
            long s2 = sheetRange[1];
            UnsafeUtility.MemCpy(longBytes + 1, sheetRange, MaxSheetCount * 2 * sizeof(long));
            return bytes;
        }
    }

    public class ExcelReader
    {
        public unsafe static T ReadBinaryTable<T>(byte[] bytes) where T : ITable
        {
            CyPtr<byte> fileBytes = new CyPtr<byte>(bytes, Allocator.Temp);
            var excelInfoHeader = ExcelUtils.DecodeInfoHeader(fileBytes);
            ITable table = null;

            byte* bodyPtr = fileBytes + BinaryExcelInfoHeader.nTotalBytes +
                            BinaryExcelSheetHeader.nTotalBytes * excelInfoHeader.sheetCount;

            for (int i = 0; i < excelInfoHeader.sheetCount; ++i)
            {
                var sheetHeaderPtrStart = fileBytes + i * BinaryExcelSheetHeader.nTotalBytes +
                                          BinaryExcelInfoHeader.nTotalBytes;
                var sheetHeader = ExcelUtils.DecodeSheetHeader(sheetHeaderPtrStart);
                
                long sheetRangeLower = *(excelInfoHeader.sheetRange + i * 2);
                long sheetRangeUpper = *(excelInfoHeader.sheetRange + i * 2 + 1);

                ByteStream bodyByteStream =
                    new ByteStream(bodyPtr + sheetRangeLower, sheetRangeUpper - sheetRangeLower);

                switch ((TableType) excelInfoHeader.tableType)
                {
                    case TableType.Event:
                        table = ReadEventBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.Dialog:
                        table = ReadDialogBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.Choose:
                        table = ReadChooseBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.UI:
                        table = ReadUIBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.Texture:
                        table = ReadTextureBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.Sprite:
                        table = ReadSpriteBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.GameObject:
                        table = ReadGameObjectBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.Material:
                        table = ReadMaterialBinaryData(&bodyByteStream, sheetHeader);
                        break;
                    case TableType.Mesh:
                        table = ReadMeshBinaryData(&bodyByteStream, sheetHeader);
                        break;
                }
            }

            return table == null ? default : (T) table;
        }
        
        unsafe public static T[] ReadArray<T>(ByteStream* byteStream) where T : unmanaged
        {
            int n = *(int*) byteStream->Get(4);
            CyPtr<T> ptr = new CyPtr<T>((T*) byteStream->Get(n), n / sizeof(T));
            T[] array = ptr.ToArray();
            ptr.Free();
            return array;
        }

        unsafe public static T[] ReadArray<T>(ByteStream* byteStream, out int n) where T : unmanaged
        {
            n = *(int*) byteStream->Get(4);
            CyPtr<T> ptr = new CyPtr<T>((T*) byteStream->Get(n), n / sizeof(T));
            T[] array = ptr.ToArray();
            ptr.Free();
            return array;
        }

        unsafe public static string ReadString(ByteStream* byteStream)
        {
            int n = *(int*) byteStream->Get(4);
            return Marshal.PtrToStringAnsi(new IntPtr(byteStream->Get(n)), n);
        }
        
        unsafe public static string ReadString(ByteStream* byteStream, out int n)
        {
            n = *(int*) byteStream->Get(4);
            return Marshal.PtrToStringAnsi(new IntPtr(byteStream->Get(n)), n);
        }

        unsafe public static int ReadInt(ByteStream* byteStream)
        {
            return *(int*) byteStream->Get(4);
        }

        unsafe public static Vector3Int ReadInt3(ByteStream* byteStream)
        {
            return *(Vector3Int*) byteStream->Get(12);
        }
        
        unsafe public static Vector4 ReadVector4(ByteStream* byteStream)
        {
            var b = (int*) byteStream->Get(16);
            return new Vector4(b[0], b[1], b[2], b[3]);
        }

        unsafe public static EventTable ReadEventBinaryData(ByteStream* bodyByteStream, BinaryExcelSheetHeader sheetHeader)
        {
            EventColumn[] eventColumns = new EventColumn[sheetHeader.row];

            for (int r = 0; r < sheetHeader.row; ++r)
            {
                EventColumn eventColumn = new EventColumn();
                eventColumn.id = ReadInt(bodyByteStream);
                eventColumn.eventModuleIds = new EventModuleID[sheetHeader.collumn - 1];
                for (int i = 0; i < sheetHeader.collumn - 1; ++i)
                {
                    EventModuleID eventModuleId = new EventModuleID();
                    eventModuleId.eventModuleType = (EventModuleType)ReadInt(bodyByteStream);
                    eventModuleId.id = ReadInt(bodyByteStream);
                    eventColumn.eventModuleIds[i] = eventModuleId;
                }

                eventColumns[r] = eventColumn;
            }
            
            EventTable eventTable = new EventTable();
            eventTable.eventColumns = eventColumns;
            eventTable.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = eventTable.idToIdxLut;
            
            for (int i = 0; i < eventColumns.Length; ++i)
            {
                idToIdxLut[eventColumns[i].id] = i;
            }

            return eventTable;
        }

        unsafe public static DialogTable ReadDialogBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            DialogColumn[] dialogColumns = new DialogColumn[sheetHeader.row];

            for (int r = 0; r < sheetHeader.row; ++r)
            {
                DialogColumn dialogColumn = new DialogColumn();
                dialogColumn.id = ReadInt(bodyByteStream);
                int nSpeaches = ReadInt(bodyByteStream);
                Speach[] speaches = new Speach[nSpeaches];
                for (int i = 0; i < nSpeaches; ++i)
                {
                    speaches[i] = new Speach(ReadInt(bodyByteStream), ReadString(bodyByteStream));
                }
                dialogColumn.speaches = speaches;
                dialogColumns[r] = dialogColumn;
            }

            DialogTable dialogTable = new DialogTable();
            dialogTable.dialogColumns = dialogColumns;
            dialogTable.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = dialogTable.idToIdxLut;
            for (int i = 0; i < dialogColumns.Length; ++i)
            {
                idToIdxLut[dialogColumns[i].id] = i;
            }

            return dialogTable;
        }

        unsafe public static ChooseTable ReadChooseBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            ChooseTable chooseTable = new ChooseTable();
            
            ChooseColumn[] chooseColumns = new ChooseColumn[sheetHeader.row];

            for (int r = 0; r < sheetHeader.row; ++r)
            {
                ChooseColumn chooseColumn = new ChooseColumn();
                chooseColumn.id = ReadInt(bodyByteStream);
                chooseColumn.choices = new Choice[sheetHeader.collumn - 1];
                for (int i = 0; i < chooseColumn.choices.Length; ++i)
                {
                    Choice choice = new Choice();
                    int n = ReadInt(bodyByteStream);
                    choice.content = ReadString(bodyByteStream);
                    PlayerAttributes playerAttributes = new PlayerAttributes();
                    for (int j = 1; j < n; ++j)
                    {
                        Vector3Int i3 = ReadInt3(bodyByteStream);
                        switch (i3.x)
                        {
                            case 0:
                                playerAttributes.AddAttribute((PlayerAttributeType) i3.y, i3.z);
                                break;
                            case 1:
                                choice.money = i3.y;
                                break;
                            case 2:
                                choice.evtId = i3.y;
                                break;
                            case 3:
                                choice.eventModuleType = (EventModuleType) i3.y;
                                choice.moduleId = i3.z;
                                break;
                        }
                    }

                    chooseColumn.choices[i] = choice;
                }

                chooseColumns[r] = chooseColumn;
            }

            Dictionary<int, int> idToIdxLut = new Dictionary<int, int>();
            for (int i = 0; i < chooseColumns.Length; ++i)
            {
                idToIdxLut[chooseColumns[i].id] = i;
            }
            
            chooseTable.chooseColumns = chooseColumns;
            chooseTable.idToIdxLut = idToIdxLut;
            return chooseTable;
        }
        
        unsafe public static UITable ReadUIBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            UIColumn[] uiColumns = new UIColumn[sheetHeader.row];

            for (int r = 0; r < sheetHeader.row; ++r)
            {
                UIColumn uiColumn = new UIColumn();
                uiColumn.id = ReadInt(bodyByteStream);
                uiColumn.assetPath = ReadString(bodyByteStream);
                uiColumns[r] = uiColumn;
            }

            UITable uiTable = new UITable();
            uiTable.uiColumns = uiColumns;
            uiTable.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = uiTable.idToIdxLut;
            for (int i = 0; i < uiColumns.Length; ++i)
            {
                idToIdxLut[uiColumns[i].id] = i;
            }

            return uiTable;
        }

        unsafe public static TextureTable ReadTextureBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            TextureColumn[] textureColumns = new TextureColumn[sheetHeader.row];
            for (int r = 0; r < sheetHeader.row; ++r)
            {
                TextureColumn textureColumn = new TextureColumn();
                textureColumn.id = ReadInt(bodyByteStream);
                textureColumn.assetPath = ReadString(bodyByteStream);
                textureColumns[r] = textureColumn;
            }
            
            TextureTable textureTable = new TextureTable();
            textureTable.textureColumns = textureColumns;
            textureTable.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = textureTable.idToIdxLut;
            for (int i = 0; i < textureColumns.Length; ++i)
            {
                idToIdxLut[textureColumns[i].id] = i;
            }

            return textureTable;
        }
        
        unsafe public static GameObjectTable ReadGameObjectBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            GameObjectColumn[] columns = new GameObjectColumn[sheetHeader.row];
            for (int r = 0; r < sheetHeader.row; ++r)
            {
                GameObjectColumn column = new GameObjectColumn();
                column.id = ReadInt(bodyByteStream);
                column.assetPath = ReadString(bodyByteStream);
                columns[r] = column;
            }
            
            GameObjectTable table = new GameObjectTable();
            table.columns = columns;
            table.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = table.idToIdxLut;
            for (int i = 0; i < columns.Length; ++i)
            {
                idToIdxLut[columns[i].id] = i;
            }

            return table;
        }
        
        unsafe public static MaterialTable ReadMaterialBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            MaterialColumn[] columns = new MaterialColumn[sheetHeader.row];
            for (int r = 0; r < sheetHeader.row; ++r)
            {
                MaterialColumn column = new MaterialColumn();
                column.id = ReadInt(bodyByteStream);
                column.assetPath = ReadString(bodyByteStream);
                columns[r] = column;
            }
            
            MaterialTable table = new MaterialTable();
            table.columns = columns;
            table.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = table.idToIdxLut;
            for (int i = 0; i < columns.Length; ++i)
            {
                idToIdxLut[columns[i].id] = i;
            }

            return table;
        }
        
        unsafe public static MeshTable ReadMeshBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            MeshColumn[] columns = new MeshColumn[sheetHeader.row];
            for (int r = 0; r < sheetHeader.row; ++r)
            {
                MeshColumn column = new MeshColumn();
                column.id = ReadInt(bodyByteStream);
                column.assetPath = ReadString(bodyByteStream);
                columns[r] = column;
            }
            
            MeshTable table = new MeshTable();
            table.columns = columns;
            table.idToIdxLut = new Dictionary<int, int>();
            var idToIdxLut = table.idToIdxLut;
            for (int i = 0; i < columns.Length; ++i)
            {
                idToIdxLut[columns[i].id] = i;
            }

            return table;
        }

        unsafe public static SpriteTable ReadSpriteBinaryData(ByteStream* bodyByteStream,
            BinaryExcelSheetHeader sheetHeader)
        {
            SpriteColumn[] columns = new SpriteColumn[sheetHeader.row];
            for (int r = 0; r < sheetHeader.row; ++r)
            {
                SpriteColumn spriteColumn = new SpriteColumn();
                spriteColumn.id = ReadInt(bodyByteStream);
                spriteColumn.textureId = ReadInt(bodyByteStream);
                var v4 = ReadVector4(bodyByteStream);
                spriteColumn.rect = new Rect(v4.x, v4.y, v4.z, v4.w);
                columns[r] = spriteColumn;
            }
            
            SpriteTable spriteTable = new SpriteTable();
            spriteTable.columns = columns;
            spriteTable.idToIdxLut = new Dictionary<int, int>();
            spriteTable.spriteRangeLut = new Dictionary<int, Vector2Int>();

            var spriteRangeLut = spriteTable.spriteRangeLut;
            var idToIdxLut = spriteTable.idToIdxLut;
            int textureId = int.MinValue;
            Vector2Int range = Vector2Int.zero;

            if (columns.Length > 0)
            {
                range.x = 0;
                textureId = columns[0].textureId;
            }
            
            for (int i = 0; i < columns.Length; ++i)
            {
                var column = columns[i];
                idToIdxLut[column.id] = i;
                if (textureId != column.textureId)
                {
                    textureId = column.textureId;
                    range.y = i - range.x;
                    spriteRangeLut[textureId] = range;
                    range.x = i;
                }
            }

            if (columns.Length > 0)
            {
                var last = columns[columns.Length - 1];
                range.y = columns.Length - range.x;
                spriteRangeLut[last.textureId] = range;
            }

            return spriteTable;
        }
    }
}