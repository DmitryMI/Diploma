using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IWeightedGraphNode<T> : IGraphNode
    {
        ICollection<IWeightedGraphNode<T>> GetConnectedWeightedNodes();
        ICollection<T> GetConnectionWeights();

        T InfinityWeight { get; set; }
    }
}