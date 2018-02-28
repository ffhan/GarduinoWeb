using System;

namespace Garduino.Models
{
    public interface IMeasureModel : IBaseModel<Entry>, ICriticalOrdering<Entry>, IDeviceableModel
    {


        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        DateTime DateTime { get; set; }

        int SoilMoisture { get; set; }
        string SoilDescription { get; set; }
        float AirHumidity { get; set; }
        float AirTemperature { get; set; }
        bool LightState { get; set; }


        
    }
}