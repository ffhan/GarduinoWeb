using System;

namespace Garduino.Models.Interfaces
{
    public interface IBaseModel<T> : IComparable<T>
    {
        Guid Id { get; set; }

        void Update(T what);
    }
}