using System;

namespace Garduino.Models.Interfaces
{
    public interface ICriticalOrdering<T> : IEquatable<T>, IComparable<T>
    {
        bool EqualsEf(T other);
        bool Equals(object obj);

        int GetHashCode();
    }
}
