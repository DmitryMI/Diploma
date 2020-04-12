using System.Collections;
using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IGraph
    {
        ICollection<IGraphNode> GetGraphNodes();

        IGraphNode this[Vector2Int position] { get; }
    }
}