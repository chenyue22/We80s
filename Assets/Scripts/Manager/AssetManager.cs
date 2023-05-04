using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using We80s.Core;
using We80s.GameData;
using We80s.GameData.Table;
using We80s.ResAssist;
using We80s.UI;
using We80s.Utils;
using Object = UnityEngine.Object;

namespace We80s.Managers
{
    public enum AssetType
    {
        UI,
        Sprite,
        Texture,
        Table,
        GameObject,
        Material,
        Mesh,
        Count
    }

    public class AssetManager : LazyInstance<AssetManager>, IManager
    {
        public Action<AssetManager> onLoaded;
        public bool Loaded { get; private set; }
        
        private static TableType[] assetTableTypes;
        
        private Dictionary<Type, Pool> objectPools = new Dictionary<Type, Pool>();
        private Dictionary<object, RefObjectData> refDatas = new Dictionary<object, RefObjectData>();

        private Dictionary<KeyValuePair<AssetType, int>, object> loadedObject =
            new Dictionary<KeyValuePair<AssetType, int>, object>();
        
        private struct RefObjectData
        {
            public AssetType assetType;
            public int id;
            public int count;

            public RefObjectData(AssetType at, int id, int c)
            {
                assetType = at;
                this.id = id;
                count = c;
            }
        }
        
        private static class TablePath
        {
            public static readonly string Event = "Assets/Res/Excel/Binary/Event/Event.bytes";
            public static readonly string Dialog = "Assets/Res/Excel/Binary/Event/Dialog.bytes";
            public static readonly string Choice = "Assets/Res/Excel/Binary/Event/Choose.bytes";
            public static readonly string UI = "Assets/Res/Excel/Binary/UI.bytes";
            public static readonly string Texture = "Assets/Res/Excel/Binary/Texture.bytes";
            public static readonly string Sprite = "Assets/Res/Excel/Binary/Sprite.bytes";
            public static readonly string GameObject = "Assets/Res/Excel/Binary/GameObject.bytes";
            public static readonly string Material = "Assets/Res/Excel/Binary/Material.bytes";
            public static readonly string Mesh = "Assets/Res/Excel/Binary/Mesh.bytes";

            internal static string GetTablePath(TableType tableType)
            {
                switch (tableType)
                {
                    case TableType.Event:
                        return Event;
                    case TableType.Dialog:
                        return Dialog;
                    case TableType.Choose:
                        return Choice;
                    case TableType.UI:
                        return UI;
                    case TableType.Texture:
                        return Texture;
                    case TableType.Sprite:
                        return Sprite;
                    case TableType.GameObject:
                        return GameObject;
                    case TableType.Material:
                        return Material;
                    case TableType.Mesh:
                        return Mesh;
                }

                return null;
            }
        }
        
        private static class Tables
        {
            private static ITable dialogTable;
            public static ITable DialogTable => dialogTable;

            private static ITable eventTable;
            public static ITable EventTable => eventTable;

            private static ITable chooseTable;
            public static ITable ChooseTable => chooseTable;

            private static ITable uiTable;
            public static ITable UITable => uiTable;

            private static ITable textureTable;
            public static ITable TextureTable => textureTable;

            private static ITable spriteTable;
            public static ITable SpriteTable => spriteTable;

            private static ITable gameObjectTable;
            public static ITable GameObjectTable => gameObjectTable;

            private static ITable materialTable;
            public static ITable MaterialTable => materialTable;

            private static ITable meshTable;
            public static ITable MeshTable => meshTable;

            private static ITable[] tables;

            internal static void LoadTables(bool forceInitTables)
            {
                if (!forceInitTables && tables != null)
                {
                    bool needInit = false;
                    foreach (var table in tables)
                    {
                        if (table == null)
                        {
                            needInit = true;
                            break;
                        }
                    }

                    if (!needInit) return;
                }
                
                tables = new ITable[(int) TableType.Count];
                
                dialogTable = Instance.LoadTable(TableType.Dialog);
                tables[(int) TableType.Dialog] = dialogTable;

                eventTable = Instance.LoadTable(TableType.Event);
                tables[(int) TableType.Event] = eventTable;

                chooseTable = Instance.LoadTable(TableType.Choose);
                tables[(int) TableType.Choose] = chooseTable;
                
                uiTable = Instance.LoadTable(TableType.UI);
                tables[(int) TableType.UI] = uiTable;

                textureTable = Instance.LoadTable(TableType.Texture);
                tables[(int) TableType.Texture] = textureTable;

                spriteTable = Instance.LoadTable(TableType.Sprite);
                tables[(int) TableType.Sprite] = spriteTable;

                gameObjectTable = Instance.LoadTable(TableType.GameObject);
                tables[(int) TableType.GameObject] = gameObjectTable;

                materialTable = Instance.LoadTable(TableType.Material);
                tables[(int) TableType.Material] = materialTable;
                
                meshTable = Instance.LoadTable(TableType.Mesh);
                tables[(int) TableType.Mesh] = meshTable;
            }

            internal static ITable GetTable(TableType tableType)
            {
                return tables[(int) tableType];
            }
        } 

        public void InitAssets(bool forceInitTables = false)
        {
            Tables.LoadTables(forceInitTables);
            assetTableTypes = new TableType[(int) AssetType.Count];
            
            for (int i = 0; i < assetTableTypes.Length; ++i)
            {
                assetTableTypes[i] = TableType.Count;
            }
            
            assetTableTypes[(int) AssetType.UI] = TableType.UI;
            assetTableTypes[(int) AssetType.Texture] = TableType.Texture;
            assetTableTypes[(int) AssetType.Sprite] = TableType.Sprite;
            assetTableTypes[(int) AssetType.GameObject] = TableType.GameObject;
            assetTableTypes[(int) AssetType.Material] = TableType.Material;
            assetTableTypes[(int) AssetType.Mesh] = TableType.Mesh;

            Loaded = true;
            onLoaded?.Invoke(this);
        }
        
        public void Init()
        {

        }
        
        public void Start()
        {
        
        }

        public void Release()
        {
            
        }

        private bool TryGetAssetTableType(AssetType assetType, out TableType tableType)
        {
            tableType = assetTableTypes[(int) assetType];
            return tableType != TableType.Count;
        }

        private void AddRefCount(AssetType assetType, int id, object o)
        {
            RefObjectData r;
            if (refDatas.TryGetValue(o, out r))
            {
                r.count++;
                refDatas[o] = r;
            }
            else
            {
                refDatas[o] = new RefObjectData(assetType, id, 1);
                loadedObject[new KeyValuePair<AssetType, int>(assetType, id)] = o;
            }
        }

        private bool MinusRefCount(object o)
        {
            RefObjectData r;
            int count = -1;
            if (refDatas.TryGetValue(o, out r))
            {
                count = --r.count;
                refDatas[o] = r;
            }

            if (count == 0)
            {
                loadedObject.Remove(new KeyValuePair<AssetType, int>(r.assetType, r.id));
            }

            return count == 0;
        }

        public T LoadComponentFromPool<T>(int id) where T : Component, IPoolElement
        {
            Pool pool;
            if (!objectPools.TryGetValue(typeof(T), out pool))
            {
                var source = LoadObject<T>(id) as Component;
                var op = new ObjectPool<T>(source.gameObject);
                op.instantiateFunc = (source) =>
                {
                    var go = (GameObject) Object.Instantiate(source);
                    return go.GetComponentInChildren<T>();
                };

                op.releaseFunc = (e) => e.gameObject.SafeRelease();
                pool = op;
                objectPools[typeof(T)] = pool;
            }

            return (T) pool.Get();
        }

        private string GetAssetPath(TableType tableType, int id)
        {
            var table = Tables.GetTable(tableType);
            var column = table.GetColumn(id) as IAssetColumn;
            if (column == null) return null;
            return column.AssetPath;
        }

        private GameObject LoadGameObject(AssetType assetType, int id)
        {
            TableType tableType;
            if (!TryGetAssetTableType(assetType, out tableType)) return null;
            
            var assetPath = GetAssetPath(tableType, id);
            if (string.IsNullOrEmpty(assetPath)) return null;

            GameObject go;
            
#if UNITY_EDITOR
            go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
#else
#endif
            AddRefCount(assetType, id, go);
            return go;
        }

        public T LoadObject<T>(int id) where T : Object
        {
            var type = typeof(T);
            if (type.IsAssignableFrom(typeof(UIBase)))
            {
                object o;
                if (loadedObject.TryGetValue(new KeyValuePair<AssetType, int>(AssetType.UI, id), out o))
                {
                    AddRefCount(AssetType.UI, id, o);
                    return (T) o;
                }
                var obj = LoadGameObject(AssetType.UI, id);
                if (!obj) return null;
                T t = obj.GetComponentInChildren<T>();
                AddRefCount(AssetType.UI, id, t);
                return t;
            }

            if (typeof(T) == typeof(Texture2D))
            {
                object o;
                if (loadedObject.TryGetValue(new KeyValuePair<AssetType, int>(AssetType.Texture, id), out o))
                {
                    AddRefCount(AssetType.Texture, id, o);
                    return (T) o;
                }
                var assetPath = GetAssetPath(TableType.Texture, id);
                Object obj = LoadObject<Texture2D>(assetPath);
                AddRefCount(AssetType.Texture, id, obj);
                return (T) obj;
            }

            if (typeof(T) == typeof(Sprite))
            {
                var table = (SpriteTable) Tables.GetTable(TableType.Sprite);
                var column = (SpriteColumn) table.GetColumn(id);
                if (column.textureId == 0) return null;
                
                object o;
                TextureAtlas textureAtlas = null;
                if (loadedObject.TryGetValue(new KeyValuePair<AssetType, int>(AssetType.Texture, column.textureId), out o))
                {
                    textureAtlas = (TextureAtlas) o;
                }
                else
                {
                    textureAtlas = new TextureAtlas();
                    var columns = table.GetTextureColumns(column.textureId);
                    if (columns == null) return null;
                    textureAtlas.SetupColumns(columns);
                }
                
                AddRefCount(AssetType.Texture, column.textureId, textureAtlas);
                Sprite sprite;
                if (textureAtlas != null && textureAtlas.TryGetSpriteById(id, out sprite))
                {
                    Object obj = sprite;
                    return (T) obj;
                }
            }

            if (typeof(T) == typeof(Mesh))
            {
                object o;
                if (loadedObject.TryGetValue(new KeyValuePair<AssetType, int>(AssetType.Mesh, id), out o))
                {
                    AddRefCount(AssetType.Mesh, id, o);
                    return (T) o;
                }
                
                var assetPath = GetAssetPath(TableType.Mesh, id);
                Object obj = LoadObject<Mesh>(assetPath);
                AddRefCount(AssetType.Mesh, id, obj);
                return (T) obj;
            }
            
            if (typeof(T) == typeof(Material))
            {
                object o;
                if (loadedObject.TryGetValue(new KeyValuePair<AssetType, int>(AssetType.Material, id), out o))
                {
                    AddRefCount(AssetType.Material, id, o);
                    return (T) o;
                }
                
                var assetPath = GetAssetPath(TableType.Material, id);
                Object obj = LoadObject<Material>(assetPath);
                AddRefCount(AssetType.Material, id, obj);
                return (T) obj;
            }
            
            if (typeof(T) == typeof(GameObject))
            {
                object o;
                if (loadedObject.TryGetValue(new KeyValuePair<AssetType, int>(AssetType.GameObject, id), out o))
                {
                    AddRefCount(AssetType.GameObject, id, o);
                    return (T) o;
                }
                
                var assetPath = GetAssetPath(TableType.GameObject, id);
                Object obj = LoadObject<GameObject>(assetPath);
                AddRefCount(AssetType.GameObject, id, obj);
                return (T) obj;
            }

            return null;
        }
        
        public void Release(object obj)
        {
            var e = obj as IPoolElement;
            if (e != null)
            {
                Pool pool;
                if (objectPools.TryGetValue(obj.GetType(), out pool))
                {
                    pool.Release(e);
                }
            }
            else if (MinusRefCount(obj))
            {
                refDatas.Remove(obj);
                var O = obj as Object;
                if (O) O.SafeRelease();
            }
        }

        public ITableColumn LoadColumn(TableType tableType, int id)
        {
            ITable table = Tables.GetTable(tableType);
            if (table != null)
            {
                return table.GetColumn(id);
            }

            return default;
        }

        private ITable LoadTable(TableType tableType)
        {
            var tablePath = TablePath.GetTablePath(tableType);
            if (string.IsNullOrEmpty(tablePath))
            {
                return default;
            }

            var bytes = LoadTextAsset(tablePath);
            if (bytes == null) return null;
            
            ITable table = null;

            switch (tableType)
            {
                case TableType.Event:
                    table = ExcelReader.ReadBinaryTable<EventTable>(bytes);
                    break;
                case TableType.Dialog:
                    table = ExcelReader.ReadBinaryTable<DialogTable>(bytes);
                    break;
                case TableType.Choose:
                    table = ExcelReader.ReadBinaryTable<ChooseTable>(bytes);
                    break;
                case TableType.UI:
                    table = ExcelReader.ReadBinaryTable<UITable>(bytes);
                    break;
                case TableType.Texture:
                    table = ExcelReader.ReadBinaryTable<TextureTable>(bytes);
                    break;
                case TableType.Sprite:
                    table = ExcelReader.ReadBinaryTable<SpriteTable>(bytes);
                    break;
                case TableType.GameObject:
                    table = ExcelReader.ReadBinaryTable<GameObjectTable>(bytes);
                    break;
                case TableType.Material:
                    table = ExcelReader.ReadBinaryTable<MaterialTable>(bytes);
                    break;
                case TableType.Mesh:
                    table = ExcelReader.ReadBinaryTable<MeshTable>(bytes);
                    break;
            }

            return table;
        }

        private T LoadObject<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else

#endif
        }

        private byte[] LoadTextAsset(string path)
        {
            
#if UNITY_EDITOR
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            return textAsset != null ? textAsset.bytes : null;
#else

#endif
        }
    }   
}
