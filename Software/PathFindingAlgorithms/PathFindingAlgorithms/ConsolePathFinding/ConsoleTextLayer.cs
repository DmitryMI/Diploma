using System;
using System.Collections.Generic;
using System.Text;

namespace PathFindingAlgorithms.ConsolePathFinding
{
    public class ConsoleTextLayer
    {
        private readonly string[,] _layer;
        private readonly ConsoleColor[,] _symbolColor;

        public ColoredString this[int x, int y] => new ColoredString() {Color = _symbolColor[x, y], Text = _layer[x, y]};

        public void SetChar(int x, int y, string c) => _layer[x, y] = c;
        public void SetColor(int x, int y, ConsoleColor color) => _symbolColor[x, y] = color;

        public int Width { get; }
        public int Height { get; }

        public ConsoleTextLayer(int width, int height, string initialSymbol = " ", ConsoleColor initialColor = ConsoleColor.White)
        {
            Width = width;
            Height = height;
            _layer = new string[Width, Height];
            _symbolColor = new ConsoleColor[Width,Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _layer[x, y] = initialSymbol;
                    _symbolColor[x, y] = initialColor;
                }
            }
        }
    }
}
