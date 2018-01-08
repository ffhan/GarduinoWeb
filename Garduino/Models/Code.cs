using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    public class Code
    {
        
        public Guid Id { get; set; }
        public int Action { get; set; }
        public DateTime dateArrived { get; set; }
        public DateTime dateCompleted { get; set; }
        public DateTime dateExecuted { get; set; }
        public bool isCompleted { get; set; }

    }
}
