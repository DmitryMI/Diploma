using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAroundPathFinding.PathFinding;
using PathFinders;
using PathFinders.Algorithms;

namespace MapAroundPathFinding
{
    public partial class CellMapDrawerForm : Form
    {
        private int _stage;

        private Vector2Int _startPoint;
        private Vector2Int _stopPoint;

        private int DefaultWidth = 1000;
        private int DefaultHeight = 550;

        private MapAroundCellMap _cellMap;
        private Graphics _pictureBoxGraphics;
        private SolidBrush _reportBrush = new SolidBrush(Color.Green);
        private SolidBrush _pathBrush = new SolidBrush(Color.Blue);
        private int _stepsEstimate;

        private int _scale;

        private ICellPathFinder _currentPathFinder;

        public CellMapDrawerForm(MapAroundCellMap cellMap)
        {
            InitializeComponent();

            _cellMap = cellMap;
            _stage = 0;

            DrawCellMatrix();

            _pictureBoxGraphics = PictureBox.CreateGraphics();

            AddAlgorithms();
        }

        private class PathFinderSelectorItem
        {
            public Type PathFinderType { get; set; }
            public override string ToString()
            {
                return PathFinderType.Name;
            }

            public PathFinderSelectorItem(Type t)
            {
                PathFinderType = t;
            }
        }

        private ICellPathFinder InstantiatePathFinder()
        {
            if (InvokeRequired)
            {
                IAsyncResult result = BeginInvoke(new Func<ICellPathFinder>(InstantiatePathFinder));
                while (!result.IsCompleted)
                {
                    Thread.Sleep(0);
                }

                return (ICellPathFinder)EndInvoke(result);
            }
            else
            {
                PathFinderSelectorItem item = (PathFinderSelectorItem)AlgorithmSelectorBox.SelectedItem;
                Type type = item.PathFinderType;
                return (ICellPathFinder)Activator.CreateInstance(type);
            }
        }

        private void AddAlgorithms()
        {
            var cellPathFinderInterface = typeof(ICellPathFinder);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => cellPathFinderInterface.IsAssignableFrom(p) && p != cellPathFinderInterface);

            foreach (var type in types)
            {
                AlgorithmSelectorBox.Items.Add(new PathFinderSelectorItem(type));
            }

            AlgorithmSelectorBox.SelectedIndex = 0;
        }

        private void ResizeWindow(int pictureWidth, int pictureHeight)
        {
            PictureBox.Width = pictureWidth;
            PictureBox.Height = pictureHeight;
            Width = PictureBox.Width + PreferencesPanel.Width + 50;
            Height = PictureBox.Height + 70;

            int pictureBoxX = PictureBox.Location.X;
            int panelX = pictureBoxX + PictureBox.Width + 5;

            PreferencesPanel.Location = new Point(panelX, FindPathButton.Location.Y);
        }

        private void DrawCellMatrix()
        {
            int mapWidth = _cellMap.Width;
            int mapHeight = _cellMap.Height;
           
            int scale = (int)Math.Ceiling((double)DefaultWidth / mapWidth);

            if (scale < 1)
            {
                scale = 1;
            }

            ResizeWindow(mapWidth * scale, mapHeight * scale);
            

            Bitmap bitmap = _cellMap.ToBitmap(scale);
            _scale = scale;
            RenderBitmap(bitmap);
        }

        private void RenderBitmap(Bitmap bitmap)
        {
            PictureBox.Image = bitmap;
        }

        private Vector2Int SetPoint(Point mousePosition, Color color)
        {
            int x = (int)Math.Round((double)mousePosition.X / _scale);
            int y = (int)Math.Round((double)mousePosition.Y / _scale);
            Vector2Int position = new Vector2Int(x, y);
            if (_cellMap.Width <= position.X)
            {
                position.X = _cellMap.Width - 1;
            }

            if (_cellMap.Height <= position.Y)
            {
                position.Y = _cellMap.Height - 1;
            }
            
            Brush brush = new SolidBrush(color);
            int scaleShift = _scale;
            int pointWidth = _scale;
            int pointHeight = _scale;
            if (pointWidth < 3)
            {
                pointWidth = 3;
            }

            if (pointHeight < 3)
            {
                pointHeight = 3;
            }
            _pictureBoxGraphics.FillEllipse(brush, position.X * _scale - scaleShift + 1, position.Y * _scale - scaleShift + 1, pointWidth, pointHeight);

            return position;
        }

        private void SetStartPoint(Point startMouse)
        {
            _startPoint = SetPoint(startMouse, Color.Green);
        }

        private void SetStopPoint(Point stopMouse)
        {
            _stopPoint = SetPoint(stopMouse, Color.Red);
        }

        private void FindPath()
        {
            //StartPathFinder();
            //return;

            Task task = new Task(StartPathFinder, TaskCreationOptions.LongRunning);
            task.Start();

            SetUiEnabled(false);
        }

        private void DrawPathPoint(Vector2Int pathPoint, Color color)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Vector2Int, Color>(DrawPathPoint), pathPoint, color);
            }
            else
            {
                int x = pathPoint.X * _scale - _scale + 1;
                int y = pathPoint.Y * _scale - _scale + 1;
                int pointWidth = _scale;
                int pointHeight = _scale;
                /*int pointWidth = _scale;
                int pointHeight = _scale;
                if (pointWidth < 5)
                {
                    pointWidth = 5;
                }

                if (pointHeight < 5)
                {
                    pointHeight = 5;
                }*/
                Brush brush = new SolidBrush(color);
                _pictureBoxGraphics.FillEllipse(brush, x, y, pointWidth, pointHeight);
            }
        }

        private void OnSmootherObstacleDetected(int x, int y)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, int>(OnSmootherObstacleDetected), x, y);
            }
            else
            {
                Debug.WriteLine($"Obstacle detected on {x}, {y}");
                _pictureBoxGraphics.DrawEllipse(new Pen(Color.LawnGreen), x * _scale, y * _scale, 3, 3);
            }
        }

        private void DrawPath(IList<Vector2Int> path)
        {
            // Raw path drawing
            foreach (var cell in path)
            {
                DrawPathPoint(cell, Color.Blue);
            }

            PathSmoother smoother = new PathSmoother();
            smoother.OnObstacleDetectedEvent += OnSmootherObstacleDetected;
            IList<Vector2Int> smoothedPath = smoother.GetSmoothedPath(_cellMap, path);
            foreach (var cell in smoothedPath)
            {
                DrawPathPoint(cell, Color.DarkCyan);
            }
        }

        private void StartPathFinder()
        {
            try
            {
                _stepsEstimate = Math.Abs(_stopPoint.Y - _startPoint.Y) + Math.Abs(_stopPoint.X - _startPoint.X);

                ICellPathFinder pathFinder = _currentPathFinder;
                pathFinder.OnCellViewedEvent += OnCellViewed;
                IList<Vector2Int> path =
                    pathFinder.GetPath(_cellMap, _startPoint, _stopPoint, NeighbourMode.SidesAndDiagonals);
                if (path == null)
                {
                    MessageBox.Show("Path not found!", "Path finder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    DrawPath(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }

            SetUiEnabled(true);
        }

        private void OnCellViewed(object sender, int x, int y, int d)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, int, int, int>(OnCellViewed), sender, x, y, d);
            }
            else
            {
                double factor = (double)d / _stepsEstimate;
                if (factor > 1)
                    factor = 1;
                byte green = (byte)(255 * (factor));
                byte red = (byte)(255 * (1 - factor));
                Color color = Color.FromArgb(red, green, 0);
                _reportBrush.Color = color;
                int scaleHalf = _scale / 2;
                int drawX = x * _scale - _scale + 1;
                int drawY = y * _scale - _scale + 1;
                _pictureBoxGraphics.FillRectangle(_reportBrush, drawX, drawY, _scale, _scale);
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            OnUserClick(coordinates);
        }

        private void SetUiEnabled(bool uiEnabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(SetUiEnabled), uiEnabled);
            }
            else
            {
                FindPathButton.Enabled = uiEnabled;
                AlgorithmSelectorBox.Enabled = uiEnabled;
                if (!uiEnabled)
                {
                    PictureBox.Click -= PictureBox_Click;
                }
                else
                {
                    PictureBox.Click += PictureBox_Click;
                }
            }
        }

        private void OnUserClick(Point coordinates)
        {
            if (_stage == 0)
            {
                SetStartPoint(coordinates);
                _stage++;
            }
            else if (_stage == 1)
            {
                SetStopPoint(coordinates);
                _stage++;
            }
            else if (_stage == 2)
            {
                DrawCellMatrix();
                _stage = 0;
            }
        }

        private void FindPathButton_Click(object sender, EventArgs e)
        {
            FindPath();
        }

        private void AlgorithmSelectorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPathFinder = InstantiatePathFinder();
        }
    }
}
