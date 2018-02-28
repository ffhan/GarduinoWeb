using System;

namespace Garduino.Models
{
    public interface ICodeModel : IBaseModel<Code>, IDeviceableModel, IComparable<Code>
    {
        int Action { get; set; }
        string ActionName { get; set; }
        DateTime DateArrived { get; set; }
        DateTime DateCompleted { get; set; }
        DateTime DateExecuted { get; set; }
        bool IsCompleted { get; set; }

        void Complete(DateTime dateExecuted);
    }
}