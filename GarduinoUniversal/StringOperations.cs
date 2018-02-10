using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarduinoUniversal
{
    public static class StringOperations
    {
        public static string PrepareForSearch(string input)
        {
            return (input ?? "").Trim();
        }

        private static string PrepareForDevice(string device) => PrepareForSearch(device).ToUpper();

        private delegate string StringPreparer(string externalString);

        private static bool IsFrom(string internalString, string externalString, StringPreparer preparer)
        {
            if (string.IsNullOrWhiteSpace(externalString)) return false;
            return preparer.Invoke(internalString).Equals(preparer.Invoke(externalString));
        }

        public static bool IsFromUser(string internalUser, string externalUser) =>
            IsFrom(internalUser, externalUser, PrepareForDevice);

        public static bool IsFromDevice(string internalDevice, string externalDevice) =>
            IsFrom(internalDevice, externalDevice, PrepareForDevice);
    }
}
