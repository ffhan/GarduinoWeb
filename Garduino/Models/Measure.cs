﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GarduinoUniversal;

namespace Garduino.Models
{// TODO: Add device name
    public class Measure : IMeasureModel
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

        [HiddenInput]
        public string UserId { get; set; }

        [Required]
        [MinLength(4)]
        public string DeviceName { get; set; }

        public virtual Device Device { get; set; }

        public void SetUser(string id) => UserId = id;

        public bool EqualsEf(Measure measure)
        {
           return Equals(measure);
        }

        public int CompareTo(Measure other)
        {
            return DateTime.CompareTo(other.DateTime);
        }

        public override bool Equals(object obj)
        {
           return Equals(obj as Measure);
        }

        public bool IsUser(string userId) => StringOperations.IsFromUser(UserId, userId);

        public bool Equals(Measure other)
        {
           try
           {
               return other != null &&
                      DateTime.Equals(other.DateTime) && IsFromDevice(other.DeviceName);
           }
           catch (Exception e)
           {
               return false;
           }

        }

        public void Update(Measure measure) //TODO: Update only if field not null.
        {
            SoilMoisture = measure.SoilMoisture;
            SoilDescription = measure.SoilDescription;
            AirHumidity = measure.AirHumidity;
            AirTemperature = measure.AirTemperature;
            LightState = measure.LightState;
            DeviceName = measure.DeviceName;
        }

        

        public static bool operator ==(Measure measure1, Measure measure2)
        {
            try
            {
                if (measure1 is null) return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return measure1.Equals(measure2);
        }

        public static bool operator !=(Measure measure1, Measure measure2)
        {
            return !(measure1 == measure2);
        }

        public bool IsFromDevice(string device) => StringOperations.IsFromDevice(DeviceName, device);

        public override int GetHashCode()
        {
            var hashCode = -1004238049;
            hashCode = hashCode * -1521134295 + DateTime.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeviceName);
            return hashCode;
        }
    }
}
