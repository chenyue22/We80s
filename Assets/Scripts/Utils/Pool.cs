using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Object = UnityEngine.Object;

namespace We80s.Utils
{
    public delegate IPoolElement InstantiateDelegate(Type type);

    public delegate T InstantiateDelegate<T>(Object source) where T : Object, IPoolElement;

    public delegate void ReleaseFunc<T>(T t) where T : Object, IPoolElement;

    public interface IPoolElement
    {
        bool Used { get; set; }
        void Release();
    }

    public abstract class Pool
    {
        public abstract IPoolElement Get();
        public abstract void Release(IPoolElement poolElement);
    }

    public sealed class CommonPool : Pool
    {
        public InstantiateDelegate instantiateFunc;

        private Type elementType;
        private List<IPoolElement> elements = new List<IPoolElement>();

        public CommonPool(Type type)
        {
            elementType = type;
        }

        public override IPoolElement Get()
        {
            IPoolElement element;
            if (elements.Count == 0 || elements[0].Used)
            {
                element = instantiateFunc != null
                    ? instantiateFunc.Invoke(elementType)
                    : (IPoolElement) FormatterServices.GetSafeUninitializedObject(elementType);
                element.Used = true;
                elements.Add(element);
            }
            else
            {
                element = elements[0];
                element.Used = true;
                elements.RemoveAt(0);
                elements.Add(element);
            }

            return element;
        }

        public override void Release(IPoolElement poolElement)
        {
            if (elements.Remove(poolElement))
            {
                poolElement.Release();
            }
        }
    }

    public abstract class Pool<T> : Pool where T : IPoolElement
    {
        public InstantiateDelegate instantiateFunc;

        private List<T> elements = new List<T>();
        private Type elementType;

        public Pool()
        {
            elementType = typeof(T);
        }

        public override IPoolElement Get()
        {
            T element;
            if (elements.Count == 0 || elements[0].Used)
            {
                element = instantiateFunc != null
                    ? (T) instantiateFunc.Invoke(elementType)
                    : (T) FormatterServices.GetSafeUninitializedObject(elementType);
                element.Used = true;
                elements.Add(element);
            }
            else
            {
                element = elements[0];
                element.Used = true;
                elements.RemoveAt(0);
                elements.Add(element);
            }

            return element;
        }

        public T GetT()
        {
            T element;
            if (elements.Count == 0 || elements[0].Used)
            {
                element = instantiateFunc != null
                    ? (T) instantiateFunc.Invoke(elementType)
                    : (T) FormatterServices.GetSafeUninitializedObject(elementType);
                element.Used = true;
                elements.Add(element);
            }
            else
            {
                element = elements[0];
                element.Used = true;
                elements.RemoveAt(0);
                elements.Add(element);
            }

            return element;
        }

        public override void Release(IPoolElement poolElement)
        {
            if (elements.Remove((T) poolElement))
            {
                poolElement.Release();
            }
        }

        public void ReleaseT(T poolElement)
        {
            if (elements.Remove(poolElement))
            {
                poolElement.Release();
            }
        }
    }

    public class ObjectPool<T> : Pool where T : Object, IPoolElement
    {
        public InstantiateDelegate<T> instantiateFunc;
        public ReleaseFunc<T> releaseFunc;

        private List<T> elements = new List<T>();
        private Object source;

        public ObjectPool(Object source)
        {
            this.source = source;
        }

        public override IPoolElement Get()
        {
            T element;
            if (elements.Count == 0 || elements[0].Used)
            {
                element = instantiateFunc != null ? instantiateFunc.Invoke(source) : (T) Object.Instantiate(source);
                element.Used = true;
                elements.Add(element);
            }
            else
            {
                element = elements[0];
                element.Used = true;
                elements.RemoveAt(0);
                elements.Add(element);
            }

            return element;
        }

        public T GetT()
        {
            T element;
            if (elements.Count == 0 || elements[0].Used)
            {
                element = instantiateFunc != null ? instantiateFunc.Invoke(source) : (T) Object.Instantiate(source);
                element.Used = true;
                elements.Add(element);
            }
            else
            {
                element = elements[0];
                element.Used = true;
                elements.RemoveAt(0);
                elements.Add(element);
            }

            return element;
        }

        public void ReleaseT(T poolElement)
        {
            if (elements.Remove(poolElement))
            {
                poolElement.Release();
                if (releaseFunc != null)
                    releaseFunc.Invoke(poolElement);
                else
                    poolElement.SafeRelease();
            }
        }

        public override void Release(IPoolElement poolElement)
        {
            T t = (T) poolElement;
            if (elements.Remove(t))
            {
                poolElement.Release();
                if (releaseFunc != null)
                    releaseFunc.Invoke(t);
                else
                    t.SafeRelease();
            }
        }
    }
}