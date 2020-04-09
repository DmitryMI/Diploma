using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IWeightedGraph<T> : IGraph
    {
        ICollection<IWeightedGraphNode<T>> GetWeightedGraphNodes();

        T GetWeight(int nodeA, int nodeB);

        T InfinityWeight { get; set; }
    }
}