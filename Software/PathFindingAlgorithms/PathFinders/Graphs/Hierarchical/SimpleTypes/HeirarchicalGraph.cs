using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class HierarchicalGraph<T> : IHierarchicalGraph<T>
    {
        private List<IHierarchicalGraphNode<T>> _nodes = new List<IHierarchicalGraphNode<T>>();
        public HierarchicalGraph(HierarchicalGraphNode<T>[] nodes, T infinityWeight)
        {
            _nodes.AddRange(nodes);
        }

        public HierarchicalGraph(T infinityWeight)
        {
            InfinityWeight = InfinityWeight;
        }


        public int Level { get; set; }
        public ICollection<IHierarchicalGraphNode<T>> GetHierarchicalGraphNodes()
        {
            return _nodes;
        }

        public ICollection<IGraphNode> GetGraphNodes()
        {
            IGraphNode[] array = new IGraphNode[_nodes.Count];
            for (int i = 0; i < _nodes.Count; i++)
            {
                array[i] = _nodes[i];
            }

            return array;
        }

        public IGraphNode this[Vector2Int position]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ICollection<IWeightedGraphNode<T>> GetWeightedGraphNodes()
        {
            throw new NotImplementedException();
        }

        public T GetWeight(int nodeA, int nodeB)
        {
            throw new NotImplementedException();
        }

        public T InfinityWeight { get; set; }

        public List<IHierarchicalGraphNode<T>> Nodes => _nodes;
    }
}
