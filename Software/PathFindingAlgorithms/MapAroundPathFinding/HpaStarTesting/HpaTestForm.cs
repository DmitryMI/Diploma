using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAroundPathFinding.HpaStarTesting;
using MapAroundPathFinding.PathFinding;
using PathFinders;
using PathFinders.Algorithms.HpaStar;
using PathFinders.Graphs;
using PathFinders.Graphs.Hierarchical;
using PathFinders.Graphs.Hierarchical.SimpleTypes;

namespace MapAroundPathFinding
{
    public partial class HpaTestForm : Form
    {
        private SimpleCellMap _cellMap;
        private Graphics _graphics;
        private Brush _mapBrush = new SolidBrush(Color.Black);
        private int _scale;

        private HpaStarAlgorithm _hpaStar;
        private List<ICellFragment> _obstacles = new List<ICellFragment>();


        private void CreateMap()
        {
            string text = File.ReadAllText("complex.cmap");
            
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            
            _cellMap = StringMapBuilder.FromText(stream, 'S', 'E', 'X', '.');

            _hpaStar = new HpaStarAlgorithm();
            _hpaStar.Initialize(_cellMap, 8);
        }

        private void DrawMap()
        {
            //pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            int scale = (int)Math.Ceiling((double)pictureBox1.Width / _cellMap.Width);

            LayeredCellMap layeredCellMap = new LayeredCellMap(_cellMap);

            foreach (var obstacle in _obstacles)
            {
                layeredCellMap.AddFragment(obstacle);
            }
            
            for (int x = 0; x < layeredCellMap.Width; x++)
            {
                for (int y = 0; y < layeredCellMap.Height; y++)
                {
                    if (!layeredCellMap.IsPassable(x, y))
                    {
                        int xScaled = x * scale;
                        int yScaled = y * scale;
                        int xDraw = xScaled;
                        int yDraw = yScaled;
                        _graphics.FillRectangle(_mapBrush, xDraw, yDraw, scale, scale);
                    }
                }
            }

            _scale = scale;
        }

        public HpaTestForm(MapAroundCellMap map)
        {
            InitializeComponent();
            CreateMap();
        }

        private void HpaTestForm_Load(object sender, EventArgs e)
        {
            _graphics = pictureBox1.CreateGraphics();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            _graphics.Clear(Color.White);
            DrawMap();
            LaunchHpaStar();
        }

        private void DrawLevelOneNodes(List<HierarchicalGraphNode> nodes)
        {
            List<Vector2Int> positions = new List<Vector2Int>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                Color color;
                if (positions.Contains(nodes[i].Position))
                {
                    Debug.WriteLine($"Duplicate node {i} on {nodes[i].Position}");
                    color = Color.Red;
                }
                else
                {
                    Debug.WriteLine($"Node {i}: {nodes[i].Position}");
                    color = Color.Blue;
                    positions.Add(nodes[i].Position);

                }
                DrawLevelOneNode(nodes[i], color);
            }
        }

        private void DrawLevelOneNode(HierarchicalGraphNode node, Color color)
        {
            Vector2Int pos = node.Position;
            int xScaled = pos.X * _scale;
            int yScaled = pos.Y * _scale;
            _graphics.FillEllipse(new SolidBrush(color), xScaled, yScaled, _scale, _scale);
            
        }

        private void DrawLevelOneConnections(List<HierarchicalGraphNode> nodes, bool printWeights)
        {
            foreach (var node in nodes)
            {
                Vector2Int pos = node.Position;
                int xScaled = pos.X * _scale;
                int yScaled = pos.Y * _scale;
                foreach (var connection in node.GetConnectedNodes())
                {
                    int startX = xScaled + _scale / 2;
                    int startY = yScaled + _scale / 2;
                    int connectionXScaled = connection.Position.X * _scale + _scale / 2;
                    int connectionYScaled = connection.Position.Y * _scale + _scale / 2;
                    _graphics.DrawLine(new Pen(Color.Green, 3), startX, startY, connectionXScaled, connectionYScaled);

                    if (printWeights)
                    {
                        int middleX = (connectionXScaled + startX) / 2;
                        int middleY = (connectionYScaled + startY) / 2;
                        double weight = node.GetWeight((IWeightedGraphNode<double>) connection);
                        Font drawFont = new Font("Arial", 5);

                        _graphics.DrawString(weight.ToString("0.0"), drawFont, new SolidBrush(Color.Black), middleX,
                            middleY);
                    }
                }
            }
        }

        private void DrawClusterBorders(CellCluster cluster)
        {
            int x = cluster.LeftBottom.X * _scale;
            int y = cluster.LeftBottom.Y * _scale;
            int width = cluster.Width * _scale;
            int height = cluster.Height * _scale;
            _graphics.DrawRectangle(new Pen(Color.Red, 2), x, y, width, height);
        }

        private void DrawClusters(CellCluster[,] clusterMatrix)
        {
            for (int x = 0; x < clusterMatrix.GetLength(0); x++)
            {
                for (int y = 0; y < clusterMatrix.GetLength(1); y++)
                {
                    DrawClusterBorders(clusterMatrix[x, y]);
                }
            }
        }

        private void DrawCellBorders()
        {
            for (int x = 0; x < _cellMap.Width; x++)
            {
                for (int y = 0; y < _cellMap.Height; y++)
                {
                    int xScaled = x * _scale;
                    int yScaled = y * _scale;
                    _graphics.DrawRectangle(new Pen(Color.Gray, 1), xScaled, yScaled, _scale, _scale);
                }
            }
        }

        private void LaunchHpaStar()
        {
            _hpaStar.ClearObstacles();
            foreach (var obstacle in _obstacles)
            {
                _hpaStar.AddObstacle(obstacle);
            }
            _hpaStar.RecalculateObstacles();

            DrawCellBorders();

            DrawClusters(_hpaStar.HierarchicalGraph.ZeroLevelClusters);
            
            DrawLevelOneNodes(_hpaStar.HierarchicalGraph.Nodes);

            DrawLevelOneConnections(_hpaStar.HierarchicalGraph.Nodes, false);
            
            _hpaStar.OnCellViewedEvent += OnCellViewed;

            IList<Vector2Int> path = _hpaStar.GetPath(_cellMap, _cellMap.DefaultStart, _cellMap.DefaultStop,
                NeighbourMode.SidesAndDiagonals);

            DrawPath(path);
        }

        private void DrawLine(Vector2Int from, Vector2Int to)
        {
            int fromX = from.X * _scale + _scale / 2;
            int fromY = from.Y * _scale + _scale / 2;
            int toX = to.X * _scale + _scale / 2;
            int toY = to.Y * _scale + _scale / 2;
            _graphics.DrawLine(new Pen(Color.DarkRed, 2), fromX, fromY, toX, toY);
        }

        private void DrawPath(IList<Vector2Int> path)
        {
            for (var i = 0; i < path.Count - 1; i++)
            {
                var cellA = path[i];
                var cellB = path[i + 1];
                DrawLine(cellA, cellB);
            }
        }

        private void OnCellViewed(object sender, int x, int y, int d)
        {
            int xScaled = x * _scale;
            int yScaled = y * _scale;
            _graphics.FillEllipse(new SolidBrush(Color.DarkGreen), xScaled, yScaled, _scale / 2, _scale / 2);
            //Thread.Sleep(100);
        }

        private void AddObstacle_Click(object sender, EventArgs e)
        {
            _obstacles.Clear();
            CellFragment fragment = new CellFragment(3, 3, new Vector2Int(15, 6));

            for (int i = 0; i < fragment.Width; i++)
            {
                for (int j = 0; j < fragment.Height; j++)
                {
                    fragment[i, j] = false;
                }
            }

            _obstacles.Add(fragment);

            DrawMap();
        }

        private void RemoveObstaclesButton_Click(object sender, EventArgs e)
        {
            _obstacles.Clear();
            
            _graphics.Clear(Color.White);
            DrawMap();
        }
    }
}
