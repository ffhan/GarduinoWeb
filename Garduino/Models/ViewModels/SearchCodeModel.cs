using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models.ViewModels
{
    public class SearchCodeModel
    {
        public Guid DeviceId { get; set; }
        public string Search { get; set; }
    }
}
