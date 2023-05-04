using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace We80s.Utils
{
    public static class LayerUtils
    {
        public static readonly LayerMask ground = LayerMask.GetMask("Ground");
        public static readonly LayerMask actor = LayerMask.GetMask("Actor");
        public static readonly LayerMask eventBound = LayerMask.GetMask("EventBound");
    }   
}