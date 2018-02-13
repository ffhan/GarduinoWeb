using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data.Interfaces
{
    public interface ICodeRepository : IBaseRepository<Code, Device>, IDeviceable<Code>, ITimeable<Code, Device>
    {
        IEnumerable<Code> GetActive(Device device);
        Task CompleteAsync(Code code, DateTime dateExecuted);
        Task CompleteAsync(Guid id, DateTime dateExecuted);
    }
}
