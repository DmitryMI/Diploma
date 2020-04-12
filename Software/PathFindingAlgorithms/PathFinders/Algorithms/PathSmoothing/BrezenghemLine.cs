using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Algorithms.PathSmoothing
{
    public class BresenhamLinePlotter
    {

        private void CastLineXBased(Vector2Int from, Vector2Int to, Func<int, int, bool> onCellPlot)
        {
            if (from.X > to.X)
            {
                Vector2Int tmp = from;
                from = to;
                to = tmp;
            }

            int deltaX = Math.Abs(to.X - from.X);
            int deltaY = Math.Abs(to.Y - from.Y);
            int error = 0;
            int deltaError = deltaY + 1;
            //int deltaError = deltaY;
            int y = from.Y;
            int dirY = to.Y - from.Y;
            if (dirY > 0)
                dirY = 1;
            if (dirY < 0)
                dirY = -1;
            for (int x = from.X; x < to.X; x++)
            {
                
                bool plotResponse = onCellPlot(x, y);
                if (!plotResponse)
                {
                    //Debug.WriteLine($"Obstacle detected on {x}, {y}");
                    return;
                }

                error = error + deltaError;
                if (error >= deltaX + 1)
                //if (error >= deltaX)
                {
                    y = y + dirY;
                    error = error - (deltaX + 1);
                    //error = error - deltaX;
                }
            }
        }

        private void CastLineYBased(Vector2Int from, Vector2Int to, Func<int, int, bool> onCellPlot)
        {
            if (from.Y > to.Y)
            {
                Vector2Int tmp = from;
                from = to;
                to = tmp;
            }

            int deltaX = Math.Abs(to.X - from.X);
            int deltaY = Math.Abs(to.Y - from.Y);
            int error = 0;
            int deltaError = deltaX + 1;
            //int deltaError = deltaX;
            int x = from.X;
            int dirX = to.X - from.X;
            if (dirX > 0)
                dirX = 1;
            if (dirX < 0)
                dirX = -1;
            for (int y = from.Y; y < to.Y; y++)
            {
               
                bool plotResponse = onCellPlot(x, y);
                if (!plotResponse)
                {
                    return;
                }
                error = error + deltaError;
                if (error >= deltaY + 1)
                //if (error >= deltaY)
                {
                    x = x + dirX;
                    error = error - (deltaY + 1);
                    //error = error - (deltaY);
                }
            }
        }

        public void CastLine(Vector2Int from, Vector2Int to, Func<int, int, bool> onCellPlot)
        {
            int deltaX = Math.Abs(to.X - from.X);
            int deltaY = Math.Abs(to.Y - from.Y);
            if (deltaX >= deltaY)
            {
                CastLineXBased(from, to, onCellPlot);
            }
            else
            {
                CastLineYBased(from, to, onCellPlot);
            }
        }
    }
}
