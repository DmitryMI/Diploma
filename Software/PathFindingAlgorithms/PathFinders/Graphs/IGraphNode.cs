using System.Collections;
using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IGraphNode
    {
        Vector2Int Position { get; set; }
        object Data { get; set; }
        ICollection<IGraphNode> GetConnectedNodes();
    }
}