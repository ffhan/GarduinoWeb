using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Data.Interfaces
{
    public interface ICallable<T>
    {
        T Call(string device);
    }
}
