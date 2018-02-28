using System;

namespace Garduino.Models
{
    public interface IBaseModel<T>
    {
        Guid Id { get; set; }

        void Update(T code);
    }
}