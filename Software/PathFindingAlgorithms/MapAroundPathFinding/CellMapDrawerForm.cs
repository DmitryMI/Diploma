using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private MapAroundCellMap _cellMap;
        private Graphics _pictureBoxGraphics;
        private SolidBrush _reportBrush = new SolidBrush(Color.Green);
        private int _stepsEstimate;

        public CellMapDrawerForm(MapAroundCellMap cellMap)
        {
            InitializeComponent();

            _cellMap = cellMap;
            _stage = 0;

            DrawCellMatrix();

            _pictureBoxGraphics = PictureBox.CreateGraphics();
        }

        private void DrawCellMatrix()
        {
            Bitmap bitmap = _cellMap.ToBitmap();

            RenderBitmap(bitmap);
        }

        private void RenderBitmap(Bitmap bitmap)
        {
            PictureBox.Image = bitmap;
        }

        private void SetStartPoint(Point startMouse)
        {
            if (_cellMap.Width <= startMouse.X)
            {
                startMouse.X = _cellMap.Width - 1;
            }

            if (_cellMap.Height <= startMouse.Y)
            {
                startMouse.Y = _cellMap.Height - 1;
            }

            Graphics gr = PictureBox.CreateGraphics();
            Brush brush = new SolidBrush(Color.Red);
            gr.FillEllipse(brush, startMouse.X, startMouse.Y, 5, 5);

            _startPoint = new Vector2Int(startMouse.X, startMouse.Y);
        }

        private void SetStopPoint(Point stopMouse)
        {
            if (_cellMap.Width <= stopMouse.X)
            {
                stopMouse.X = _cellMap.Width - 1;
            }

            if (_cellMap.Height <= stopMouse.Y)
            {
                stopMouse.Y = _cellMap.Height - 1;
            }

            Graphics gr = PictureBox.CreateGraphics();
            Brush brush = new SolidBrush(Color.Green);
            gr.FillEllipse(brush, stopMouse.X, stopMouse.Y, 5, 5);

            _stopPoint = new Vector2Int(stopMouse.X, stopMouse.Y);
        }

        private void FindPath()
        {
            _stepsEstimate = Math.Abs(_stopPoint.Y - _startPoint.Y) + Math.Abs(_stopPoint.X - _startPoint.X);

            ICellPathFinder pathFinder = new AStarAlgorithm();
            pathFinder.OnCellViewedEvent += OnCellViewed;
            IList<Vector2Int> path = pathFinder.GetPath(_cellMap, _startPoint, _stopPoint, NeighbourMode.SidesAndDiagonals);

            Graphics graphics = PictureBox.CreateGraphics();
            Brush brush = new SolidBrush(Color.Blue);
            foreach (var cell in path)
            {
                graphics.FillEllipse(brush, cell.X, cell.Y, 3, 3);
            }
        }

        private void OnCellViewed(object sender, int x, int y, int d)
        {
            double factor = (double)d / _stepsEstimate;
            byte green = (byte)(255 * (factor));
            byte red = (byte)(255 * (1 - factor));
            Color color = Color.FromArgb(red, green, 0);
            _reportBrush.Color = color;
            _pictureBoxGraphics.FillRectangle(_reportBrush, x, y, 1, 1);
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            OnUserClick(coordinates);
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
    }
}
