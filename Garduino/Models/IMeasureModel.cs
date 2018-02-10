using System;

namespace Garduino.Models
{
    public interface IMeasureModel : IBaseModel<Measure>, ICriticalOrdering<Measure>
    {


        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        DateTime DateTime { get; set; }

        int SoilMoisture { get; set; }
        String SoilDescription { get; set; }
        float AirHumidity { get; set; }
        float AirTemperature { get; set; }
        bool LightState { get; set; }

        string DeviceName { get; set; }
        Device Device { get; set; }

        bool IsFromDevice(string device);
    }
}