using System.Collections;
using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IGraph
    {
        ICollection<IGraphNode> GetGraphNodes();
    }
}