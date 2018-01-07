using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    public class Measure : IEquatable<Measure>
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Time stamp")]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTime { get; set; }
        [Required]
        [DisplayName("Soil moisture")]
        public int SoilMoisture { get; set; }

        [DisplayName("Soil description")]
        public String SoilDescription { get; set; }

        [Required]
        [DisplayName("Air humidity")]
        public float AirHumidity { get; set; }
        [Required]
        [DisplayName("Air temperature")]
        public float AirTemperature { get; set; }
        [Required]
        [DisplayName("Light on")]
        public bool LightState { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Measure);
        }

        public bool Equals(Measure other)
        {
            return other != null &&
                   DateTime == other.DateTime;
        }

        public static bool operator ==(Measure measure1, Measure measure2)
        {
            return EqualityComparer<Measure>.Default.Equals(measure1, measure2);
        }

        public static bool operator !=(Measure measure1, Measure measure2)
        {
            return !(measure1 == measure2);
        }
    }
}
