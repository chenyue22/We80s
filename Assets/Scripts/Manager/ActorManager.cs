using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using We80s.Core;

namespace We80s.Managers
{
    public class ActorManager : LazyInstance<ActorManager>, IManager
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
