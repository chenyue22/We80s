using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using We80s.GameData.Table;
using We80s.Managers;
using We80s.Render;
using SceneManager = We80s.Managers.SceneManager;

namespace We80s.Core
{
    public class GameMain : MonoBehaviour
    {
        private List<IManager> managers = new List<IManager>();
        private List<IUpdate> updatableManagers = new List<IUpdate>();
        
        private static bool allManagerLoaded;
        
        [SerializeField] private GameCamera camera;
        [SerializeField] private Canvas canvas;

        private static GameObject miniGame;
        public static GameCamera MainCamera { get; private set; }
        public static Camera MiniGameCamera { get; private set; }
        public static GameObject MiniGame => miniGame;
        
        private void AddManager(IManager manager)
        {
            managers.Add(manager);
            var update = manager as IUpdate;
            if (update != null)
            {
                updatableManagers.Add(update);
            }
        }

        private static Action onLoaded;
        public static Action OnLoaded
        {
            get => onLoaded;
            set
            {
                if (allManagerLoaded)
                {
                    value.Invoke();
                }
                else
                {
                    onLoaded += value;
                }
            }
        }
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            allManagerLoaded = false;
            MainCamera = camera;
            AddManager(AssetManager.Instance);
            AddManager(SceneManager.Instance);
            AddManager(UIManager.Instance);
            AddManager(EventManager.Instance);
            AddManager(ActorManager.Instance);
            AddManager(TimeManager.Instance);

            UIManager.Instance.canvas = canvas;

            AssetManager.Instance.OnLoaded += (assetManager) =>
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

            miniGame = AssetManager.Instance.LoadObject<GameObject>(100001);
            DontDestroyOnLoad(miniGame);
            Camera miniGameCamera;
            if (miniGame.transform.GetChild(0).TryGetComponent(out miniGameCamera))
            {
                MiniGameCamera = miniGameCamera;
            }
            onLoaded?.Invoke();

            SceneManager.Instance.LoadScene(100000, LoadSceneMode.Single, scene => scene.NewScene());
        }
    }
}
