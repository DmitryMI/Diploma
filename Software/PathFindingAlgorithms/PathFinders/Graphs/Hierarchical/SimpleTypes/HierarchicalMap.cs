using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs.SimpleTypes;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class HierarchicalMap
    {
        private List<HierarchicalGraphNode> _nodes = new List<HierarchicalGraphNode>();

        public CellCluster[,] ZeroLevelClusters { get; set; }

        public HierarchicalMap(HierarchicalGraphNode[] nodes)
        {
            _nodes.AddRange(nodes);
        }

        public HierarchicalMap()
        {
           
        }


        public int Level { get; set; }
        public ICollection<HierarchicalGraphNode> GetHierarchicalGraphNodes()
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
       

        public List<HierarchicalGraphNode> Nodes => _nodes;
    }
}
