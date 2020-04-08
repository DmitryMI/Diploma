using System;
using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IGraphPathFinder
    {
        event Action<object, int, int, int> OnCellViewedEvent;

        IList<Vector2Int> GetPath(GraphNode[,] map, GraphNode start, GraphNode stop);
    }
}