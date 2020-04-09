using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using PathFinders;
using PathFinders.Algorithms;
using PathFindingAlgorithms.ConsolePathFinding;


namespace PathFindingAlgorithms
{
    class Program
    {
        private static ConsoleDrawer[] _consoleDrawers;
        private static ICellPathFinder[] _contestants;
        private static Task[] _tasks;
        private static ICellMap _map;

        private static Vector2Int _start, _stop;

        private static int _runningTasks;

        static void InitArrays(int count)
        {
            _consoleDrawers = new ConsoleDrawer[count];
            _contestants = new ICellPathFinder[count];
            _tasks = new Task[count];
        }

        static void InitConsoleDrawer(ConsoleMap map, Vector2Int scale, int spacing, Vector2Int start, Vector2Int stop, int index)
        {
            ConsoleColorLayer backgroundLayer = map.GetColorLayer();
            ConsoleTextLayer textLayer = new ConsoleTextLayer(backgroundLayer.Width, backgroundLayer.Height);

            int x = (map.Width * scale.X + spacing) * index;
            var drawer = new ConsoleDrawer(x, 0, scale, backgroundLayer, textLayer);

            drawer.SetTextCell(start.X, start.Y, "S");
            drawer.SetTextCell(stop.X, stop.Y, "E");

            Console.CursorTop = map.Height;
            drawer.Draw();

            _consoleDrawers[index] = drawer;
        }


        static void Main(string[] args)
        {
            Console.BufferWidth = 300;
            Console.BufferHeight = 240;
            Console.WindowWidth = 180;

            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            FileStream mapFileStream = File.Open("Maps/trap.cmap", FileMode.Open, FileAccess.Read);

            ConsoleMap map = ConsoleMapGenerator.FromText(mapFileStream, 'S', 'E', 'X', '.');
            _map = map;

            InitArrays(2);

            Vector2Int start = map.DefaultStart;
            Vector2Int stop = map.DefaultStop;
            _start = start;
            _stop = stop;
            Vector2Int scale = new Vector2Int(4, 2);
            int spacing = 4;

            

            InitConsoleDrawer(map, scale, spacing, start, stop, 0);
            InitConsoleDrawer(map, scale, spacing, start, stop, 1);

            Console.SetCursorPosition(0, map.Height * scale.Y);

            _contestants[0] = new AStarAlgorithm();
            _contestants[0].OnCellViewedEvent += OnCellUpdated;

            _contestants[1] = new BestFirstSearch();
            _contestants[1].OnCellViewedEvent += OnCellUpdated;

            StartPathFinder(0);
            StartPathFinder(1);

            while (_runningTasks > 0)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void StartPathFinder(int index)
        {
            Task task = new Task(GetPath, index, TaskCreationOptions.LongRunning);
            task.Start();
            _runningTasks++;
        }

        static void GetPath(object indexObj)
        {
            int index = (int) indexObj;
            IList<Vector2Int> path = _contestants[index].GetPath(_map, _start, _stop, NeighbourMode.SideOnly);

            DrawPath(path, index);
            _runningTasks--;
        }

        static void DrawPath(IList<Vector2Int> path, int index)
        {
            if (path == null)
            {
                Console.WriteLine($"Contestant {index}: Path not found");
                return;
            }
            foreach (var cell in path)
            {
                _consoleDrawers[index].SetBackgroundCell(cell.X, cell.Y, ConsoleColor.Red);
            }
            _consoleDrawers[index].Update();
        }

        static void OnCellUpdated(object sender, int x, int y, int d)
        {
            int index = 0;
            for (; index < _contestants.Length && _contestants[index] != sender; index++) ;
            
            string dStr = d.ToString();
            
            _consoleDrawers[index].SetTextCell(x, y, dStr);
            _consoleDrawers[index].Update();

            Thread.Sleep(10);
        }

    }
}
