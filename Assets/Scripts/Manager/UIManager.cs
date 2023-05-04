using UnityEngine;
using We80s.Core;
using We80s.UI;

namespace We80s.Managers
{
    public enum SingletonUI
    {
        Dialog,
        Count
    }
    
    public class UIManager : LazyInstance<UIManager>, IManager
    {
        public bool Loaded { get; set; }
        public Canvas canvas;

        private UIBase[] singletonUIs;
        private int[] singletonUIIDs;

        public void Init()
        {
            Loaded = true;
        }

        public void Start()
        {
            singletonUIs = new UIBase[(int) SingletonUI.Count];
            singletonUIIDs = new int[(int) SingletonUI.Count];

            singletonUIIDs[(int) SingletonUI.Dialog] = 100000;
            
            for (int i = 0; i < singletonUIIDs.Length; ++i)
            {
                singletonUIs[i] = AssetManager.Instance.LoadObject<UIBase>(singletonUIIDs[i]);
                singletonUIs[i].gameObject.SetActive(false);
            }
        }

        public void Release()
        {

        }

        public UIBase Open(SingletonUI singletonUI)
        {
            var ui = singletonUIs[(int) singletonUI];
            ui.gameObject.SetActive(true);
            ui.transform.SetParent(canvas.transform);
            return ui;
        }

        public UIBase Open(int id)
        {
            var ui = AssetManager.Instance.LoadObject<UIBase>(id);
            ui.gameObject.SetActive(true);
            ui.transform.SetParent(canvas.transform);
            return ui;
        }

        public void Close(SingletonUI singletonUI)
        {
            singletonUIs[(int) singletonUI].gameObject.SetActive(false);
        }
    }   
}