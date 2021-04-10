using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs;
using PathFinders.Logging;

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

        public static Bitmap GetBitmap(ICellMap map, int scale, Vector2Int start, Vector2Int end,
            IList<Vector2Int> path)
        {
            Bitmap bmp = GetBitmap(map, scale);

            if (path != null)
            {
                foreach (var cell in path)
                {
                    DrawScaledPixel(bmp, Color.Blue, cell.X, cell.Y, scale);
                }
            }

            DrawScaledPixel(bmp, Color.Green,  start.X, start.Y, scale);
            DrawScaledPixel(bmp, Color.Red,  end.X, end.Y, scale);

            return bmp;
        }

        public static Bitmap GetBitmap(IWeightedGraph<double> graph, Vector2Int start, int width, int height, int scale)
        {
            int bmpWidth = width * scale;
            int bmpHeight = width * scale;
            Bitmap bitmap = new Bitmap(bmpWidth, bmpHeight);

            var nodes = graph.GetWeightedGraphNodes();

            foreach (var node in nodes)
            {
                if (node != null)
                {
                    DrawScaledPixel(bitmap, Color.Blue, node.Position.X, node.Position.Y, scale);
                }
            }

            return bitmap;
        }

        private static void DrawGraphNode(Bitmap bitmap, IGraphNode node, int scale, List<IGraphNode> visitedList)
        {
            DrawScaledPixel(bitmap, Color.Blue, node.Position.X, node.Position.Y, scale);
            visitedList.Add(node);
            LogManager.Log($"Visited list size: {visitedList.Count}");
            foreach (var connection in node.GetConnectedNodes())
            {
                if (!visitedList.Contains(connection))
                {
                    DrawGraphNode(bitmap, connection, scale, visitedList);
                }
            }
        }

        private static void DrawScaledPixel(Bitmap bmp, Color color, int x, int y, int scale)
        {
            double scaleHalf = (double)scale / 2;
            int xScaled = x * scale - (int)Math.Round(scaleHalf);
            int yScaled = y * scale - (int)Math.Round(scaleHalf);

            int borderMin = -(int)Math.Floor(scaleHalf);
            int borderMax = (int)Math.Ceiling(scaleHalf);

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
