using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders
{
    public static class CellMapToBitmap
    {
        public static Bitmap GetBitmap(ICellMap map, int scale)
        {
            Bitmap bitmap = new Bitmap(map.Width * scale, map.Height * scale);

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (!map.IsPassable(x, y))
                    {
                        DrawScaledPixel(bitmap, Color.Black, x, y, scale);
                    }
                }
            }

            return bitmap;
        }

        private static void DrawScaledPixel(Bitmap bmp, Color color, int x, int y, int scale)
        {
            double scaleHalf = (double)scale / 2;
            int xScaled = x * scale - (int)Math.Round(scaleHalf);
            int yScaled = y * scale - (int)Math.Round(scaleHalf);

            int borderMin = -(int)Math.Round(scaleHalf);
            int borderMax = (int)Math.Round(scaleHalf);

            for (int i = borderMin; i <= borderMax; i++)
            {
                int drawX = xScaled + i;
                if (drawX < 0 || drawX >= bmp.Width)
                {
                    continue;
                }
                for (int j = -borderMax; j <= borderMax; j++)
                {
                    int drawY = yScaled + j;
                    if (drawY < 0 || drawY >= bmp.Height)
                        continue;
                    bmp.SetPixel(drawX, drawY, color);
                }
            }
        }
    }
}
