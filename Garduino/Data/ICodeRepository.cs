using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface ICodeRepository : IRepository<Code>
    {
        IEnumerable<Code> GetActive(string userId);
    }
}
