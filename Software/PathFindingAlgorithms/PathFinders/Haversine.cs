using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders
{
    public static class Haversine
    {
        public const double r = 6378.137;

        public static double Hav(double theta)
        {
            double cosT = Math.Cos(theta);
            return (1 - cosT) / 2;
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double havP21 = Hav(lat2 - lat1);
            double havL21 = Hav(lng2 - lng1);

            double cosP1 = Math.Cos(lat1);
            double cosP2 = Math.Cos(lat2);
            double cosHav = cosP1 * cosP2 * havL21;

            double sum = havP21 + cosHav;
            double sqrt = Math.Sqrt(sum);
            double arcsin = Math.Asin(sqrt);

            double d = 2 * r * arcsin;
            return d;
        }
    }
}
