using System;
using System.Collections.Generic;
using System.Linq;
using PathFinders;

namespace MapAroundPathFinding.PathFinding.PolygonFillerUtil
{
    public class PolygonFiller : IComparer<Edge>
    {
        private Action<int, int, bool> _onCellStateUpdate;
        public PolygonFiller(Action<int, int, bool> onCellStateUpdate)
        {
            _onCellStateUpdate = onCellStateUpdate;
        }

        private void ProcessCurrentLine(List<Edge> edges, int y, List<Edge> activeEdges, out int yNext)
        {
            yNext = Int32.MaxValue;
            foreach (var edge in edges)
            {
                int y1 = edge.Start.Y;
                int y2 = edge.End.Y;

                if (y1 == y && y2 != y)
                {
                    activeEdges.Add(edge);
                }

                if (y1 > y && y1 < yNext)
                {
                    yNext = y1;
                }

                if (y2 > y && y2 < yNext)
                {
                    yNext = y2;
                }
            }
        }

        private List<int> GetIntersectionX(List<Edge> activeEdges, int y)
        {
            List<int> result = new List<int>(activeEdges.Count);

            foreach (var edge in activeEdges)
            {
                double dx = (double)(edge.End.X - edge.Start.X) / (edge.End.Y - edge.Start.Y);
                double x = dx * (y - edge.Start.Y) + edge.Start.X;
                result.Add((int)Math.Round(x));
            }

            result.Sort();
            return result;
        }

        private void FillLine(int xStart, int xEnd, int y)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                _onCellStateUpdate?.Invoke(x, y, true);
            }
        }

        private void FillScanLine(List<int> xIntersections, int y)
        {
            for (int i = 0; i < xIntersections.Count; i += 2)
            {
                int xStart = xIntersections[i];
                int xEnd = xIntersections[i + 1];
                FillLine(xStart, xEnd, y);
            }
        }

        private void ClearActiveEdges(List<Edge> activeEdges, int y)
        {
            for (int i = 0; i < activeEdges.Count; i++)
            {
                if (activeEdges[i].End.Y == y)
                {
                    activeEdges.RemoveAt(i);
                    i--;
                }
            }
        }

        public void FillPolygon(List<Edge> edges)
        {
            edges.Sort(this);
            int yMin = edges[0].Start.Y;
            int yMax = edges.Last().End.Y;

            int yCurrent = yMin;
            List<Edge> activeEdges = new List<Edge>();
            while (yCurrent < yMax)
            {
                ProcessCurrentLine(edges, yCurrent, activeEdges, out int yNext);

                for (int yStep = yCurrent; yStep < yNext; yStep++)
                {
                    List<int> xIntersection = GetIntersectionX(activeEdges, yStep);
                    FillScanLine(xIntersection, yStep);
                }

                yCurrent = yNext;

                ClearActiveEdges(activeEdges, yCurrent);
            }
        }

        public int Compare(Edge a, Edge b)
        {
            if (a == null || b == null)
                return 0;
            return a.Start.Y - b.Start.Y;
        }
    }
}
