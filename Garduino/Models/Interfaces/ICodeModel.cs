using System;

namespace Garduino.Models.Interfaces
{
    public interface ICodeModel : IBaseModel<Code>, IContainedInDevice
    {
        void Complete(DateTime executed);
        bool IsFromDevice(string device);

        int Action { get; set; }

        string ActionName { get; set; }

        DateTime DateArrived { get; set; }

        DateTime DateCompleted { get; set; }

        DateTime DateExecuted { get; set; }

        bool IsCompleted { get; set; }
    }
}