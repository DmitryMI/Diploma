using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathFinders.Graphs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs.SimpleTypes;
using PathFindersTests;

namespace PathFinders.Graphs.Tests
{
    [TestClass()]
    public class GraphGeneratorTests
    {
        private static Random _rnd = new Random();

        private static bool GetRandomBoolean()
        {
            int value = _rnd.Next(0, 2);
            if (value == 0)
            {
                return false;
            }

            return true;
        }

        private static void FillRandom(IMutableCellMap mutableMap)
        {
            for (int i = 0; i < mutableMap.Width; i++)
            {
                for (int j = 0; j < mutableMap.Height; j++)
                {
                    mutableMap.SetPassable(i, j, GetRandomBoolean());
                }
            }
        }

        private GraphNode GetConnection(GraphNode node, Vector2Int step)
        {
            Vector2Int shiftPosition = node.Position + step;
            foreach (var connection in node.Connections)
            {
                Debug.Write($"Comparing position {shiftPosition} and {connection.Position}: ");
                if (shiftPosition == connection.Position)
                {
                    Debug.WriteLine($"Equal");
                    return connection;
                }
                else
                {
                    Debug.WriteLine($"Not equal");
                }
            }

            return null;
        }


        [TestMethod()]
        public void GetGraphTest()
        {
            int width = _rnd.Next(10, 20);
            int height = _rnd.Next(10, 20);
            NeighbourMode neighbourMode = NeighbourMode.SideOnly;
            MemoryCellFragment cellFragment = new MemoryCellFragment(width, height);
            FillRandom(cellFragment);
            GraphNode[,] nodeMatrix = GraphGenerator.GetGraph(cellFragment, neighbourMode);

            if (nodeMatrix.GetLength(0) != width)
                Assert.Fail("Graph width is incorrect");
            if (nodeMatrix.GetLength(1) != height)
                Assert.Fail("Graph height is incorrect");

            Vector2Int[] steps = Steps.GetSteps(neighbourMode);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GraphNode node = nodeMatrix[i, j];
                    bool cellPassable = cellFragment[i, j];
                    if (node == null)
                    {
                        if (cellPassable)
                        {
                            Assert.Fail($"Cell ({i}, {j}) is passable, but node does not exist");
                        }
                        continue;
                    }


                    int connectedCellsCount = 0;

                    foreach (var step in steps)
                    {
                        int xShift = i + step.X;
                        int yShift = j + step.Y;
                        bool stepInBounds = cellFragment.IsInBounds(xShift, yShift);
                        bool stepPassable = stepInBounds && cellFragment.IsPassable(xShift, yShift);
                        Debug.WriteLine($"Position: {node.Position}, Step: {step}, Neighbour: {node.Position + step}");
                        GraphNode connection = GetConnection(node, step);
                        if (stepPassable)
                        {
                            connectedCellsCount++;
                            if (connection == null)
                            {
                                Assert.Fail($"Node {node.Position} is not connected to position ({xShift}, {yShift}), that is passable");
                            }
                        }
                        else
                        {
                            if (connection != null)
                            {
                                Assert.Fail($"Node {node.Position} is connected to position ({xShift}, {yShift}), that is not passable");
                            }
                        }
                    }

                    if (node.Connections.Count != connectedCellsCount)
                    {
                        Assert.Fail("Number of connected nodes is incorrect");
                    }
                }
            }
        }

        [TestMethod()]
        public void GetWeightedGraphTest()
        {
            int width = _rnd.Next(10, 20);
            int height = _rnd.Next(10, 20);
            NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals;
            MemoryCellFragment cellFragment = new MemoryCellFragment(width, height);
            FillRandom(cellFragment);
            WeightedGraph<double> weightedGraph = GraphGenerator.GetWeightedGraph(cellFragment, neighbourMode);

            if (weightedGraph.Width != width)
                Assert.Fail("Graph width is incorrect");
            if (weightedGraph.Height != height)
                Assert.Fail("Graph height is incorrect");

            Vector2Int[] steps = Steps.GetSteps(neighbourMode);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WeightedGraphNode<double> node = (WeightedGraphNode<double>)weightedGraph[i, j];
                    bool cellPassable = cellFragment[i, j];
                    if (node == null)
                    {
                        if (cellPassable)
                        {
                            Assert.Fail($"Cell ({i}, {j}) is passable, but node does not exist");
                        }
                        continue;
                    }


                    int connectedCellsCount = 0;

                    foreach (var step in steps)
                    {
                        int xShift = i + step.X;
                        int yShift = j + step.Y;
                        bool stepInBounds = cellFragment.IsInBounds(xShift, yShift);
                        bool stepPassable = stepInBounds && cellFragment.IsPassable(xShift, yShift);
                        Debug.WriteLine($"Position: {node.Position}, Step: {step}, Neighbour: {node.Position + step}");
                        WeightedGraphNode<double> connection = (WeightedGraphNode<double>)GetConnection(node, step);
                        if (stepPassable)
                        {
                            connectedCellsCount++;
                            if (connection == null || double.IsInfinity(connection.GetWeight(node)))
                            {
                                Assert.Fail($"Node {node.Position} is not connected to position ({xShift}, {yShift}), that is passable");
                            }
                        }
                        else
                        {
                            if (connection != null && !double.IsInfinity(connection.GetWeight(node)))
                            {
                                Assert.Fail($"Node {node.Position} is connected to position ({xShift}, {yShift}), that is not passable");
                            }
                        }
                    }

                    if (node.Connections.Count != connectedCellsCount)
                    {
                        Assert.Fail("Number of connected nodes is incorrect");
                    }
                }
            }
        }
    }
}