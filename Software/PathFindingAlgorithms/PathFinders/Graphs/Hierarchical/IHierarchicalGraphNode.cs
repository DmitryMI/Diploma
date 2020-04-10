namespace PathFinders.Graphs.Hierarchical
{
    public interface IHierarchicalGraphNode<T> : IWeightedGraphNode<T>
    {
        IHierarchicalGraph<T> ParentGraph { get; }
    }
}