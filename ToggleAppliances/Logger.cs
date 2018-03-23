using System;

namespace ToggleAppliances
{
    internal class Logger
    {
        internal static void Log(string message)
        {
            Console.WriteLine("[ToggleAppliances] " + message);
        }
    }
}
