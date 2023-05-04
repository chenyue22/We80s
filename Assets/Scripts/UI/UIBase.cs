using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using We80s.Utils;

namespace We80s.UI
{
    public abstract class UIBase : MonoBehaviour, IPoolElement
    {
        public bool Used { get; set; }
        public abstract void Release();
    }   
}