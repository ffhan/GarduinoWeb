using System;

namespace Garduino.Models
{
    public interface IBaseModel<T> : IComparable<T>
    {
        Guid Id { get; set; }
        string UserId { get; set; }

        void SetUser(string id);
        void Update(T code);

        bool IsUser(string userId);
    }
}