using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PathFindingAlgorithms.ConsolePathFinding
{
    public class ConsoleDrawer
    {
        private ConsoleColorLayer _backgroundLayer;
        private ConsoleTextLayer _textLayer;

        private Queue<Vector2> _updatedCells = new Queue<Vector2>();

        private int posX, posY;
        public int Width { get; }
        public int Height { get; }

        public Vector2 Scale { get; }

        public ConsoleDrawer(int x, int y, Vector2 scale, ConsoleColorLayer backgroundLayer, ConsoleTextLayer textLayer)
        {
            if (backgroundLayer.Width != textLayer.Width || backgroundLayer.Height != textLayer.Height)
            {
                throw new ArgumentException("Layer dimensions must match");
            }

            _backgroundLayer = backgroundLayer;
            _textLayer = textLayer;

            posX = x;
            posY = y;
            Scale = scale;

            Width = backgroundLayer.Width;
            Height = backgroundLayer.Height;
        }

        public ConsoleDrawer(int x, int y, int width, int height, Vector2 scale)
        {
            posX = x;
            posY = y;
            Scale = scale;
            _backgroundLayer = new ConsoleColorLayer(width, height);
            _textLayer = new ConsoleTextLayer(width, height);

            Width = width;
            Height = height;
        }

        private void DrawCell(int relativeX, int relativeY)
        {
            lock (Console.In)
            {
                int cellX = (relativeX) * Scale.X + posX;
                int cellY = (relativeY) * Scale.Y + posY;

                int consolePositionXTmp = Console.CursorLeft;
                int consolePositionYTmp = Console.CursorTop;

                ConsoleColor backgroundColorTmp = Console.BackgroundColor;
                ConsoleColor textColorTmp = Console.ForegroundColor;

                Console.SetCursorPosition(cellX, cellY);
                Console.BackgroundColor = _backgroundLayer[relativeX, relativeY];
                Console.ForegroundColor = _textLayer[relativeX, relativeY].Color;

                int linesCount = Scale.Y;
                int lineLength = Scale.X;
                string text = _textLayer[relativeX, relativeY].Text;

                int textPos = 0;
                for (int i = 0; i < linesCount; i++)
                {
                    char[] line = new char[lineLength];
                    for (int j = 0; j < lineLength; j++)
                    {
                        if (textPos < text.Length)
                        {
                            line[j] = text[textPos];
                            textPos++;
                        }
                        else
                        {
                            line[j] = ' ';
                        }
                    }
                    Console.CursorTop = cellY + i;
                    Console.CursorLeft = cellX;
                    Console.Write(line);
                }

                Console.SetCursorPosition(consolePositionXTmp, consolePositionYTmp);
                Console.BackgroundColor = backgroundColorTmp;
                Console.ForegroundColor = textColorTmp;
            }
        }

        public void SetTextCell(int x, int y, ColoredString coloredChar)
        {
            _textLayer.SetChar(x, y, coloredChar.Text);
            _textLayer.SetColor(x, y, coloredChar.Color);
            _updatedCells.Enqueue(new Vector2(x, y));
        }

        public void SetTextCell(int x, int y, string c)
        {
            _textLayer.SetChar(x, y, c);
            _updatedCells.Enqueue(new Vector2(x, y));
        }

        public void SetTextCell(int x, int y, ConsoleColor color)
        {
            _textLayer.SetColor(x, y, color);
            _updatedCells.Enqueue(new Vector2(x, y));
        }

        public void SetBackgroundCell(int x, int y, ConsoleColor color)
        {
            _backgroundLayer[x, y] = color;
            _updatedCells.Enqueue(new Vector2(x, y));
        }

        public void Update()
        {
            while (_updatedCells.Count > 0)
            {
                Vector2 cell = _updatedCells.Dequeue();
                DrawCell(cell.X, cell.Y);
            }
        }

        public void Draw()
        {
            _updatedCells.Clear();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    DrawCell(i, j);
                }
            }
        }
    }
}
