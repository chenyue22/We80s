using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace We80s.GameData
{
    unsafe public struct ByteStream
    {
        private byte* ptr;
        private long size;
        
        public bool End => size == 0;

        public ByteStream(byte* bytes, long n)
        {
            ptr = bytes;
            size = n;
        }

        public byte* Get(int n)
        {
            var s = (long) Mathf.Min(size, n);
            var p = ptr;
            size -= s;
            ptr += s;
            return p;
        }
    }   
}
