using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GarduinoWebAPI.Models
{
    public class Measure
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int SoilMoisture { get; set; }
        [Required]
        public String SoilDescription { get; set; }

        [Required]
        public double AirHumidity { get; set; }
        [Required]
        public double AirTemperature { get; set; }

        [Required]
        public Guid SourceId { get; set; }
    }
}
