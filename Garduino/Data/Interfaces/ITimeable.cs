using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Data
{
    public interface ITimeable<T>
    {
        T GetLatest(string userId);
    }
}
