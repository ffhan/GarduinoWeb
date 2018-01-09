using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    public class Code
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int Action { get; set; }

        [Required]
        [DisplayName("Date arrived")]
        public DateTime DateArrived { get; set; }

        [Required]
        [DisplayName("Date completed")]
        public DateTime DateCompleted { get; set; }

        [DisplayName("Date executed")]
        public DateTime DateExecuted { get; set; }

        [Required]
        [DisplayName("Is it completed?")]
        public bool IsCompleted { get; set; }

        public void Complete()
        {
            IsCompleted = true;
            DateCompleted = DateTime.Now;
        }
    }
}
