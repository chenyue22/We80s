using System;

namespace We80s.Core
{
    public class LazyInstance<T>
    {
        private static Lazy<T> lazyInstance = new Lazy<T>(() => Activator.CreateInstance<T>());
        public static T Instance => lazyInstance.Value;

    }   
}