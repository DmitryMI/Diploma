﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs.Hierarchical.SimpleTypes;

namespace PathFinders.Graphs.Hierarchical
{
    public static class HierarchicalMapGenerator
    {
        public static Action<object, int, int, int> CellViewedCallback { get; set; }

        public static CellCluster[,] GeneratedClusters { get; set; }

        private static CellCluster _currentCellCluster;

        public static HierarchicalMap GenerateMap(ICellMap cellMap, NeighbourMode neighbourMode, params int[] clusterSizes)
        {
            if (clusterSizes.Length == 1)
            {
                return GenerateTwoLevelMap(cellMap, neighbourMode, clusterSizes[0]);
            }

            throw new NotImplementedException();
        }

        private static CellCluster[,] CreateClusters(ICellMap map, int clusterSize)
        {
            int matrixWidth = (int)Math.Ceiling((double)map.Width / clusterSize);
            int matrixHeight = (int)Math.Ceiling((double)map.Height / clusterSize);

            CellCluster[,] clusterMatrix = new CellCluster[matrixWidth, matrixHeight];

            int currentX = 0;
            
            for (int cx = 0; cx < matrixWidth; cx++)
            {
                int xBorderDelta = map.Width - currentX;
                int actualWidth = xBorderDelta >= clusterSize ? clusterSize : xBorderDelta;
                int currentY = 0;
                for (int cy = 0; cy < matrixHeight; cy++)
                {
                    int yBorderDelta = map.Height - currentY;
                    int actualHeight = yBorderDelta >= clusterSize ? clusterSize : yBorderDelta;
                    
                    CellCluster cluster = new CellCluster()
                    {
                        Width =  actualWidth,
                        Height = actualHeight,
                        LeftBottom = new Vector2Int(currentX, currentY),
                        Map = map
                    };
                    clusterMatrix[cx, cy] = cluster;
                    currentY += actualHeight;
                }
                
                currentX += actualWidth;
            }

            return clusterMatrix;
        }

        private static void GetTransitionCells(CellCluster clusterA, CellCluster clusterB, Vector2Int start,
            Vector2Int delta, Vector2Int bOffset, List<Vector2Int> aPoints, List<Vector2Int> bPoints)
        {
            int xCurrent = start.X;
            int yCurrent = start.Y;

            ICellMap map = clusterA.Map;

            bool IsInsideCluster(int x, int y)
            {
                int clusterRelativeX = x - clusterA.LeftBottom.X;
                int clusterRelativeY = y - clusterA.LeftBottom.Y;
                if (clusterRelativeX < 0 || clusterRelativeY < 0 || clusterRelativeX >= clusterA.Width ||
                    clusterRelativeY >= clusterB.Height)
                    return false;
                return true;
            }

            while (IsInsideCluster(xCurrent, yCurrent))
            {
                while (!map.IsPassable(xCurrent, yCurrent) ||
                       !map.IsPassable(xCurrent + bOffset.X, yCurrent + bOffset.Y))
                {
                    xCurrent += delta.X;
                    yCurrent += delta.Y;
                    if (!IsInsideCluster(xCurrent, yCurrent))
                    {
                        return;
                    }
                }

                aPoints.Add(new Vector2Int(xCurrent, yCurrent));
                bPoints.Add(new Vector2Int(xCurrent + bOffset.X, yCurrent + bOffset.Y));

              
                xCurrent += delta.X;
                yCurrent += delta.Y;

                if (!IsInsideCluster(xCurrent, yCurrent))
                {
                    break;
                }

                while (map.IsPassable(xCurrent, yCurrent) && map.IsPassable(xCurrent + bOffset.X, yCurrent + bOffset.Y))
                {
                    xCurrent += delta.X;
                    yCurrent += delta.Y;
                    if (!IsInsideCluster(xCurrent, yCurrent))
                    {
                        break;
                    }
                }

                int prevPassableX = xCurrent - delta.X;
                int prevPassableY = yCurrent - delta.Y;
                //int prevPassableX = xCurrent;
                //int prevPassableY = yCurrent;
                Vector2Int aPoint = new Vector2Int(prevPassableX, prevPassableY);
                Vector2Int bPoint = new Vector2Int(prevPassableX + bOffset.X, prevPassableY + bOffset.Y);
                if (aPoints[aPoints.Count - 1] != aPoint)
                {
                    aPoints.Add(aPoint);
                    bPoints.Add(bPoint);
                }
            }
        }

        private static void GetTransitionCells(CellCluster clusterA, CellCluster clusterB, List<Vector2Int> aPoints, List<Vector2Int> bPoints)
        {
            Vector2Int start = clusterA.LeftBottom;
            //Vector2Int start = new Vector2Int(0, 0);
            Vector2Int delta = default;
            Vector2Int bOffset = default;
            // A | B
            if (clusterB.LeftBottom.X > clusterA.LeftBottom.X && clusterB.LeftBottom.Y == clusterA.LeftBottom.Y)
            {
                start.X += clusterA.Width - 1;
                delta = new Vector2Int(0, 1);
                bOffset = new Vector2Int(1, 0);
            }

            // B | A
            if (clusterB.LeftBottom.X < clusterA.LeftBottom.X && clusterB.LeftBottom.Y == clusterA.LeftBottom.Y)
            {
                delta = new Vector2Int(0, 1);
                bOffset = new Vector2Int(-1, 0);
            }

            // A
            // -
            // B
            if (clusterB.LeftBottom.Y < clusterA.LeftBottom.Y && clusterB.LeftBottom.X == clusterA.LeftBottom.X)
            {
                //start.Y -= clusterA.Height - 1;
                delta = new Vector2Int(1, 0);
                bOffset = new Vector2Int(0, -1);
            }

            // B
            // -
            // A
            if (clusterB.LeftBottom.Y > clusterA.LeftBottom.Y && clusterB.LeftBottom.X == clusterA.LeftBottom.X)
            {
                start.Y += clusterA.Height - 1;
                delta = new Vector2Int(1, 0);
                bOffset = new Vector2Int(0, 1);
            }

            GetTransitionCells(clusterA, clusterB, start, delta, bOffset, aPoints, bPoints);
        }

        private static void ProceedNeighbourClusters(CellCluster currentCluster, CellCluster neighbourCluster, HierarchicalMap graph)
        {
            List<Vector2Int> currentPoints = new List<Vector2Int>(2);
            List<Vector2Int> neighbourPoints = new List<Vector2Int>(2);
            GetTransitionCells(currentCluster, neighbourCluster, currentPoints, neighbourPoints);
            for (var k = 0; k < currentPoints.Count; k++)
            {
                var currentPoint = currentPoints[k];
                var neighbourPoint = neighbourPoints[k];
                HierarchicalGraphNode transitionNodeCurrent, transitionNodeNeighbour;

                transitionNodeCurrent = (HierarchicalGraphNode)currentCluster.TransitionNodes.FirstOrDefault(node => node.Position == currentPoint);
                transitionNodeNeighbour = (HierarchicalGraphNode)neighbourCluster.TransitionNodes.FirstOrDefault(node => node.Position == neighbourPoint);
                if (transitionNodeCurrent == null)
                {
                    transitionNodeCurrent = new HierarchicalGraphNode();
                    currentCluster.TransitionNodes.Add(transitionNodeCurrent);
                    graph.Nodes.Add(transitionNodeCurrent);
                }

                if (transitionNodeNeighbour == null)
                {
                    transitionNodeNeighbour = new HierarchicalGraphNode();
                    neighbourCluster.TransitionNodes.Add(transitionNodeNeighbour);
                    graph.Nodes.Add(transitionNodeNeighbour);
                }
                
                transitionNodeCurrent.SetWeight(transitionNodeNeighbour, 1);
                transitionNodeNeighbour.SetWeight(transitionNodeCurrent, 1);

                transitionNodeCurrent.Position = currentPoint;
                transitionNodeNeighbour.Position = neighbourPoint;

                transitionNodeCurrent.ParentCluster = currentCluster;
                transitionNodeNeighbour.ParentCluster = neighbourCluster;
            }
        }

        

        private static HierarchicalMap GenerateTwoLevelMap(ICellMap cellMap, NeighbourMode neighbourMode, int levelZeroClusterSize)
        {
            CellCluster[,] clusterMatrix = CreateClusters(cellMap, levelZeroClusterSize);
            GeneratedClusters = clusterMatrix;
            
            HierarchicalMap levelOneMap = new HierarchicalMap();
            for (int i = 0; i < clusterMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < clusterMatrix.GetLength(1); j++)
                {
                    CellCluster currentCluster = clusterMatrix[i, j];
                    
                    int prevI = i - 1;
                    int prevJ = j - 1;
                    int nextJ = j + 1;
                    if (prevI >= 0)
                    {
                        CellCluster neighbourCluster = clusterMatrix[prevI, j];
                        ProceedNeighbourClusters(currentCluster, neighbourCluster, levelOneMap);
                    }
                    if (prevJ >= 0)
                    {
                        CellCluster neighbourCluster = clusterMatrix[i, prevJ];
                        ProceedNeighbourClusters(currentCluster, neighbourCluster, levelOneMap);
                    }
                    if (prevJ >= 0 && prevI >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                    {
                        CellCluster neighbourCluster = clusterMatrix[prevI, prevJ];
                        ProceedNeighbourClusters(currentCluster, neighbourCluster, levelOneMap);
                    }
                    if (nextJ < cellMap.Height && prevI >= 0 && neighbourMode == NeighbourMode.SidesAndDiagonals)
                    {
                        CellCluster neighbourCluster = clusterMatrix[prevI, nextJ];
                        ProceedNeighbourClusters(currentCluster, neighbourCluster, levelOneMap);
                    }
                }
            }

            for (int i = 0; i < clusterMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < clusterMatrix.GetLength(1); j++)
                {
                    _currentCellCluster = clusterMatrix[i, j];
                    clusterMatrix[i, j].CalculatePaths(neighbourMode, OnCellClusterCellViewed);
                }
            }

            levelOneMap.ZeroLevelClusters = clusterMatrix;

            return levelOneMap;
        }

        private static void OnCellClusterCellViewed(object sender, int x, int y, int d)
        {
            CellViewedCallback?.Invoke(sender, x + _currentCellCluster.LeftBottom.X, y + _currentCellCluster.LeftBottom.Y, d);
        }
    }
}
