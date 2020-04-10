using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IWeightedGraphPathFinder<T> : IGraphPathFinder
    {
        IList<IGraphNode> GetPath(IWeightedGraph<T> map, IWeightedGraphNode<T> start, IWeightedGraphNode<T> stop);
    }
}