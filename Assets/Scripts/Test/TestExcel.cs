using UnityEngine;
using We80s.GameData.Table;
using We80s.Managers;

public class TestExcel : MonoBehaviour
{
    public TableType tableType;
    public int id;
    public Sprite sprite;
    public Texture2D texture2D;
    
    [ContextMenu("Test")]
    private void Test()
    {
        AssetManager.Instance.InitAssets(true);
        ITableColumn column = AssetManager.Instance.LoadColumn(tableType, id);

        switch (column)
        {
            case EventColumn eventColumn:
                string str = eventColumn.id + ":";
                for (int i = 0; i < eventColumn.eventModuleIds.Length; ++i)
                {
                    str += eventColumn.eventModuleIds[i].eventModuleType + "-" + eventColumn.eventModuleIds[i].id;
                    if (i < eventColumn.eventModuleIds.Length - 1)
                    {
                        str += ",";
                    }
                }
                Debug.Log(str);
                break;
            case DialogColumn dialogColumn:
                Debug.Log(dialogColumn.id);
                foreach (var speach in dialogColumn.speaches)
                {
                    Debug.Log(speach.speakerId + ":" + speach.content);
                }
                break;
            case ChooseColumn chooseColumn:
                Debug.Log(chooseColumn.id);
                break;
            case TextureColumn textureColumn:
                Debug.Log(textureColumn.id + ":" + textureColumn.assetPath);
                break;
            case SpriteColumn spriteColumn:
                Debug.Log(spriteColumn.id + ":" + spriteColumn.textureId + "_" + spriteColumn.rect);
                break;
            case GameObjectColumn gameObjectColumn:
                Debug.Log(AssetManager.Instance.LoadObject<GameObject>(gameObjectColumn.id));
                Debug.Log(gameObjectColumn.id + ":" + gameObjectColumn.assetPath);
                break;
            case MaterialColumn materialColumn:
                Debug.Log(AssetManager.Instance.LoadObject<Material>(materialColumn.id));
                Debug.Log(materialColumn.id + ":" + materialColumn.assetPath);
                break;
            case MeshColumn meshColumn:
                Debug.Log(AssetManager.Instance.LoadObject<Mesh>(meshColumn.id));
                Debug.Log(meshColumn.id + ":" + meshColumn.assetPath);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
