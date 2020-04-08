using System;
using System.Collections.Generic;
using System.Text;

namespace PathFindingAlgorithms.ConsolePathFinding
{
    public class ConsoleColorLayer
    {
        private readonly ConsoleColor[,] _layer;

        public ConsoleColor this[int x, int y]
        {
            get => _layer[x, y];
            set => _layer[x, y] = value;
        }

        public int Width { get; }
        public int Height { get; }

        public ConsoleColorLayer(int width, int height, ConsoleColor initialColor = ConsoleColor.Black)
        {
            Width = width;
            Height = height;
            _layer = new ConsoleColor[Width,Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _layer[x, y] = initialColor;
                }
            }
        }

        public ConsoleColorLayer(ConsoleColor[,] layer)
        {
            _layer = layer;
            Width = layer.GetLength(0);
            Height = layer.GetLength(1);
        }
    }
}
