using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GarduinoUniversal;
using Newtonsoft.Json;

namespace Garduino.Models
{// TODO: Add device name
    public class Entry : IMeasureModel
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("UTC Time stamp")]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTime { get; set; }

        [DisplayName("Local time stamp")]
        public DateTime LocalDateTime => Device.User.ConvertTime(DateTime);

        [Required]
        [DisplayName("Soil moisture")]
        public int SoilMoisture { get; set; }

        [DisplayName("Soil description")]
        public string SoilDescription { get; set; }

        [Required]
        [DisplayName("Air humidity")]
        public float AirHumidity { get; set; }
        [Required]
        [DisplayName("Air temperature")]
        public float AirTemperature { get; set; }
        [Required]
        [DisplayName("Light on")]
        public bool LightState { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }


        public bool EqualsEf(Entry entry)
        {
           return Equals(entry);
        }

        public int CompareTo(Entry other)
        {
            return DateTime.CompareTo(other.DateTime);
        }

        public override bool Equals(object obj)
        {
           return Equals(obj as Entry);
        }
        

        public bool Equals(Entry other)
        {
           try
           {
               return other != null &&
                      DateTime.Equals(other.DateTime) && IsFromDevice(other.Device?.Name);
           }
           catch (Exception e)
           {
               return false;
           }

        }

        public void Update(Entry entry) //TODO: Update only if field not null.
        {
            SoilMoisture = entry.SoilMoisture;
            SoilDescription = entry.SoilDescription;
            AirHumidity = entry.AirHumidity;
            AirTemperature = entry.AirTemperature;
            LightState = entry.LightState;
        }

        public bool IsFromDevice(string device) => StringOperations.IsFromDevice(Device.Name, device);

        public override int GetHashCode()
        {
            var hashCode = -1004238049;
            hashCode = hashCode * -1521134295 + DateTime.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Device>.Default.GetHashCode(Device);
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(SoilMoisture);
            return hashCode;
        }

        public void SetDevice(Device device)
        {
            Device = device;
        }

        public bool IsFromDevice(Device device)
        {
            return StringOperations.IsFromDevice(Device.Name, device.Name);
        }
    }
}
