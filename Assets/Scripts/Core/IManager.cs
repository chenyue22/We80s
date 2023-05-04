using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace We80s.Core
{
    public interface IManager
    {
        bool Loaded { get; }
        void Init();
        void Start();
        void Release();
    }   
}
