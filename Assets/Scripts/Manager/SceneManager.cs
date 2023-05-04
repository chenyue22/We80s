using We80s.Core;

namespace We80s.Managers
{
    public class SceneManager : LazyInstance<SceneManager>, IManager
    {
        public bool Loaded { get; set; }

        public void Init()
        {
            Loaded = true;
        }

        public void Start()
        {

        }

        public void Release()
        {

        }
    }
}