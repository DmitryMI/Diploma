using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IWeightedGraphNode<T> : IGraphNode
    {
        ICollection<IWeightedGraphNode<T>> GetConnectedWeightedNodes();
        ICollection<T> GetConnectionWeights();

        T GetWeight(IWeightedGraphNode<T> connection);

        void SetWeight(IWeightedGraphNode<T> connection, T weight);

        T InfinityWeight { get; set; }
    }
}