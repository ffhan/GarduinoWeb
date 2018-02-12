using System;

namespace Garduino.Models.Interfaces
{
    public interface IMeasureModel : IBaseModel<Measure>, ICriticalOrdering<Measure>, IContainedInDevice
    {


        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        DateTime DateTime { get; set; }

        int SoilMoisture { get; set; }
        string SoilDescription { get; set; }
        float AirHumidity { get; set; }
        float AirTemperature { get; set; }
        bool LightState { get; set; }

        bool IsFromDevice(string device);
    }
}