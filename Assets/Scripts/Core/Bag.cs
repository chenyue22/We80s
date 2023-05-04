namespace We80s.Core
{
    public interface IBagElement
    {
        bool Empty { get; set; }
    }
    
    public class Bag<T> where T : IBagElement
    {
        private T[] array;
        private int num;

        public Bag(int size)
        {
            array = new T[size];
        }
        
        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public bool Empty => num == 0;

        public bool Add(T t)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i] == null)
                {
                    array[i] = t;
                    t.Empty = false;
                    return true;
                }
            }

            return false;
        }

        public void Set(int idx, T t)
        {
            array[idx] = t;
            t.Empty = false;
        }

        public void Remove(int idx)
        {
            array[idx] = default;
        }

        public T Get(int idx)
        {
            if (idx < array.Length)
            {
                var t = array[idx];
                if (!t.Empty)
                {
                    t.Empty = true;
                    return t;
                }
            }

            return default;
        }
    }
}