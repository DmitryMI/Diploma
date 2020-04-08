using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAroundPathFinding.PathFinding.PolygonFillerUtil;
using PathFinders;

namespace FillerTesting
{
    public partial class MainForm : Form
    {
        private List<Point> _points = new List<Point>();
        private bool _isFinalized;
        private Graphics _graphics;
        private Pen _pen;
        private Bitmap _bitmap;

        public MainForm()
        {
            InitializeComponent();

            _graphics = pictureBox1.CreateGraphics();
            _pen = new Pen(Color.Black);
        }

        

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;

            OnClick(coordinates);
        }

        private List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>(_points.Count);

            Vector2Int prevPoint = default;
            for (int i = 0; i < _points.Count; i++)
            {
                Vector2Int vector2Int = new Vector2Int(_points[i].X, _points[i].Y);
                if (i > 0)
                {
                    edges.Add(new Edge(prevPoint, vector2Int));
                }

                prevPoint = vector2Int;
            }

            Vector2Int first = new Vector2Int(_points[0].X, _points[0].Y);
            Vector2Int last = new Vector2Int(_points.Last().X, _points.Last().Y);
            edges.Add(new Edge(first, last));
            return edges;
        }

        private void FinalizePolygon()
        {
            Debug.WriteLine("Starting filling polygon...");

            Point first = _points[0];
            Point last = _points.Last();

            _graphics.DrawLine(_pen, first, last);
            _isFinalized = true;

            PolygonFiller filler = new PolygonFiller(OnPointFilled);

            List<Edge> edges = GetEdges();

            _bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            filler.FillPolygon(edges);

            Debug.WriteLine("Filling finished!");

            pictureBox1.Image = _bitmap;
        }

        private void OnPointFilled(int x, int y, bool filled)
        {
            if (filled)
            {
                //Debug.WriteLine($"({x}, {y})");
                if(x < 0 || y < 0 || x >= _bitmap.Width || y >= _bitmap.Height)
                    return;
                _bitmap.SetPixel(x, y, Color.Blue);
            }
        }

        private void OnClick(Point point)
        {
            if (_isFinalized)
            {
                Debug.WriteLine("Clearing picture box");
                _graphics.Clear(Color.White);
                _points.Clear();
                _isFinalized = false;
            }

            if (_points.Count == 0)
            {
                _points.Add(point);
            }
            else
            {
                int firstDx = Math.Abs(_points[0].X - point.X);
                int firstDy = Math.Abs(_points[0].Y - point.Y);

                if (firstDx < 3 && firstDy < 3)
                {
                    FinalizePolygon();
                    return;
                }

                int prevX = _points.Last().X;
                int prevY = _points.Last().Y;
                _graphics.DrawLine(_pen, prevX, prevY, point.X, point.Y);
                _points.Add(point);
            }
        }
    }
}
