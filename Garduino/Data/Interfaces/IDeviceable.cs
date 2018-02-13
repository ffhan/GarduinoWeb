using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data.Interfaces
{
    public interface IDeviceable<T>
    {
        Task<bool> AddAsync(T what, Device device);
    }
}
