using System.Collections.Generic;
using UnityEngine;
using We80s.Managers;

namespace We80s.Core
{
    public class GameMain : MonoBehaviour
    {
        private List<IManager> managers = new List<IManager>();
        private List<IUpdate> updatableManagers = new List<IUpdate>();
        
        private bool allManagerLoaded;
        public bool AllManagerLoaded => allManagerLoaded;
        
        [SerializeField] private Camera camera;
        [SerializeField] private Canvas canvas;
        
        public static Camera mainCamera { get; private set; }

        private void AddManager(IManager manager)
        {
            managers.Add(manager);
            var update = manager as IUpdate;
            if (update != null)
            {
                updatableManagers.Add(update);
            }
        }
        
        
        private void Awake()
        {
            mainCamera = camera;
            managers.Add(AssetManager.Instance);
            managers.Add(SceneManager.Instance);
            managers.Add(UIManager.Instance);
            managers.Add(EventManager.Instance);
            managers.Add(ActorManager.Instance);

            UIManager.Instance.canvas = canvas;

            AssetManager.Instance.onLoaded += (assetManager) =>
            {
                foreach (var manager in managers)
                {
                    manager.Init();
                }
            };
            
            AssetManager.Instance.InitAssets();
        }

        private void Update()
        {
            if (allManagerLoaded)
            {
                foreach (var u in updatableManagers)
                {
                    u.OnUpdate();
                }
                return;
            }
            
            foreach (var m in managers)
            {
                if (!m.Loaded)
                {
                    return;
                }
            }

            allManagerLoaded = true;

            foreach (var m in managers)
            {
                m.Start();
            }
        }
    }
}
