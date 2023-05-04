using System;

namespace We80s.Core
{
    public interface IExcelAssist
    {
        object[] GetRow(int row);
        object[] GetCollumn(int collumn);
        object Get(int row, int collumn);
        
        T[] GetRow<T>(int row);
        T[] GetCollumn<T>(int collumn);
        T Get<T>(int row, int collumn);
    }
}
