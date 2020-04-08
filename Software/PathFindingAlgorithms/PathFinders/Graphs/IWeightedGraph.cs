using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IWeightedGraph<T> : IGraph
    {
        ICollection<IWeightedGraphNode<T>> GetWeightedGraphNodes();

        T InfinityWeight { get; set; }
    }
}