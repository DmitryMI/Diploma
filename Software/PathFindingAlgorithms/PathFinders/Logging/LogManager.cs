using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Logging
{
    public static class LogManager
    {
        public static ILogger Logger { get; set; }

        public static void Log(string message, int errorLevel = 0)
        {
            Logger?.Log(message, errorLevel);
        }

        public static void Log(Bitmap bitmap)
        {
            Logger?.Log(bitmap);
        }
    }
}
