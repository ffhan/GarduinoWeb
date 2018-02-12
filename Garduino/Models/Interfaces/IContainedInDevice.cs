namespace Garduino.Models.Interfaces
{
    public interface IContainedInDevice
    {
        string DeviceName { get; set; }

        Device Device { get; set; }
    }
}
