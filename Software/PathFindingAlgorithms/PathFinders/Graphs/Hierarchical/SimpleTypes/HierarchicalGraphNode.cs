using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class HierarchicalGraphNode : WeightedGraphNode<double>
    {
        public CellCluster ParentCluster { get; set; }
        

        public HierarchicalGraphNode() : base(double.PositiveInfinity)
        {
           
        }
    }
}
