using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    public interface ICriticalOrdering<T> : IEquatable<T>, IComparable<T>
    {
        bool EqualsEf(T other);
        bool Equals(object obj);

        int GetHashCode();
    }
}
