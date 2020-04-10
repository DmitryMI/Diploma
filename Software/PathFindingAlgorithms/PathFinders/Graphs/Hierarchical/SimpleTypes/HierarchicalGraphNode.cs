using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class HierarchicalGraphNode<T> : WeightedGraphNode<T>, IHierarchicalGraphNode<T>
    {
        public HierarchicalGraphNode(T infinityWeight) : base(infinityWeight)
        {
        }
        public IHierarchicalGraph<T> ParentGraph { get; set; }

        public HierarchicalGraph<T> ParenHeHierarchicalGraph
        {
            get => (HierarchicalGraph<T>)ParentGraph;
            set => ParentGraph = value;
        }
        
    }
}
