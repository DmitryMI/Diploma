using System.Collections.Generic;

namespace PathFinders.Graphs.Hierarchical
{
    public interface IHierarchicalGraph<T> : IWeightedGraph<T>
    {
        int Level { get; }
        ICollection<IHierarchicalGraphNode<T>> GetHierarchicalGraphNodes();
    }
}