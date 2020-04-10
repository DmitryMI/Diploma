﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapAroundPathFinding.PathFinding;
using PathFinders;
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

        private void CreateMap()
        {
            string text = File.ReadAllText("complex.cmap");

            
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            
            _cellMap = StringMapBuilder.FromText(stream, 'S', 'E', 'X', '.');
        }

        private void DrawMap()
        {
            //pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            int scale = (int)Math.Ceiling((double)pictureBox1.Width / _cellMap.Width);
            
            for (int x = 0; x < _cellMap.Width; x++)
            {
                for (int y = 0; y < _cellMap.Height; y++)
                {
                    if (_cellMap[x, y] == false)
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
            DrawMap();

            LaunchHpaStar();
        }

        private void DrawLevelOneNodes(List<IHierarchicalGraphNode<double>> nodes)
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

        private void DrawLevelOneNode(IHierarchicalGraphNode<double> node, Color color)
        {
            Vector2Int pos = node.Position;
            int xScaled = pos.X * _scale;
            int yScaled = pos.Y * _scale;
            _graphics.FillEllipse(new SolidBrush(color), xScaled, yScaled, _scale, _scale);
            
        }

        private void DrawLevelOneConnections(List<IHierarchicalGraphNode<double>> nodes)
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

        private void LaunchHpaStar()
        {
            var hierarchyGraph = HierarchicalGraphGenerator.GenerateGraph(_cellMap, NeighbourMode.SideOnly, 8);

            DrawLevelOneNodes(hierarchyGraph.Nodes);

            DrawLevelOneConnections(hierarchyGraph.Nodes);

            CellCluster[,] clusters = HierarchicalGraphGenerator.GeneratedClusters;

            for (int x = 0; x < clusters.GetLength(0); x++)
            {
                for (int y = 0; y < clusters.GetLength(1); y++)
                {
                    DrawClusterBorders(clusters[x, y]);
                }
            }
        }
    }
}
