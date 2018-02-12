using System;

namespace Garduino.Models.Interfaces
{
    public interface IBaseModel<T> : IComparable<T>
    {
        Guid Id { get; set; }

        ApplicationUser User { get; set; }

        void Update(T what);

        void SetUser(ApplicationUser user);

        bool IsUser(ApplicationUser user);
    }
}