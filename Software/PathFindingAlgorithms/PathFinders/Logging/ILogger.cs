using System.Drawing;

namespace PathFinders.Logging
{
    public interface ILogger
    {
        void Log(string message, int errorLevel);
        void Log(Bitmap bmp);
    }
}