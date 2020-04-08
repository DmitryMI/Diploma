using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PathFinders;

namespace PathFindingAlgorithms.ConsolePathFinding
{
    public static class ConsoleMapGenerator
    {
        public static ConsoleMap FromText(Stream stream, char start, char stop, char wall, char empty)
        {
            List<string> lines = ReadTextStream(stream, new List<char>(){wall, empty, start, stop}, empty);

            Console.WriteLine("map file:");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();

            if (lines.Count == 0)
            {
                throw new ArgumentException("Nothing was read from stream");
            }
            int width = lines[0].Length;
            int height = lines.Count;

            ConsoleMap map = new ConsoleMap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (lines[y][x] == wall)
                    {
                        map.SetCell(x, y, false);
                    }
                    else if (lines[y][x] == empty)
                    {
                        map.SetCell(x, y, true);
                    }
                    else if (lines[y][x] == start)
                    {
                        map.SetCell(x, y, true);
                        map.DefaultStart = new Vector2Int(x, y);
                    }
                    else if (lines[y][x] == stop)
                    {
                        map.SetCell(x, y, true);
                        map.DefaultStop = new Vector2Int(x, y);
                    }
                    else
                    {
                        throw new ArgumentException("Stream contains unknown chars");
                    }
                }
            }

            return map;
        }

        private static List<string> ReadTextStream(Stream stream, List<char> whiteList, char replacer)
        {
            List<string> lines = new List<string>();

            while (stream.Position < stream.Length)
            {
                StringBuilder builder = new StringBuilder();
                while (stream.Position < stream.Length)
                {
                    int symbol = stream.ReadByte();
                    if (symbol == -1)
                    {
                        break;
                    }
                    if (symbol == '\n' || symbol == '\r')
                    {
                        if (builder.Length > 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (whiteList.Contains((char)symbol))
                        {
                            builder.Append((char)symbol);
                        }
                        else
                        {
                            //builder.Append(replacer);
                        }
                    }
                }
                //Console.WriteLine("Line length: " + builder.Length);
                lines.Add(builder.ToString());
            }

            return lines;
        }
    }
}
