using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

        public override int GetHashCode()
        {
            var hashCode = 1957286377;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + DateTime.GetHashCode();
            hashCode = hashCode * -1521134295 + SoilMoisture.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SoilDescription);
            hashCode = hashCode * -1521134295 + AirHumidity.GetHashCode();
            hashCode = hashCode * -1521134295 + AirTemperature.GetHashCode();
            hashCode = hashCode * -1521134295 + LightState.GetHashCode();
            return hashCode;
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
