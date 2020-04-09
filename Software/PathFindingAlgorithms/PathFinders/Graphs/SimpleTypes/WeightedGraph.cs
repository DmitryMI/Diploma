using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Graphs.SimpleTypes
{
    public class WeightedGraph<T> : Graph, IWeightedGraph<T>
    {

        public WeightedGraph(int width, int height, T infinityWeight) : base(new IGraphNode[width,height])
        {
            InfinityWeight = infinityWeight;
        }
        public WeightedGraph(IGraphNode[] graphNodes, T infinityWeight) : base(graphNodes)
        {
            InfinityWeight = infinityWeight;
        }

        public WeightedGraph(IGraphNode[,] graphNodes, T infinityWeight) : base(graphNodes)
        {
            InfinityWeight = infinityWeight;
        }

        public ICollection<IWeightedGraphNode<T>> GetWeightedGraphNodes()
        {
            IWeightedGraphNode<T>[] nodes = new IWeightedGraphNode<T>[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    nodes[y * Width + x] = (IWeightedGraphNode<T>) GraphNodes[x, y];
                }
            }

            return nodes;
        }

        private IWeightedGraphNode<T> GetByIndex(int nodeIndex)
        {
            int y = nodeIndex / Width;
            int x = nodeIndex - y * Width;
            return (IWeightedGraphNode<T>) GraphNodes[x, y];
        }

        public T GetWeight(int nodeAIndex, int nodeBIndex)
        {
            IWeightedGraphNode<T> nodeA = GetByIndex(nodeAIndex);
            IWeightedGraphNode<T> nodeB = GetByIndex(nodeBIndex);
            return nodeA.GetWeight(nodeB);
        }

        public T InfinityWeight { get; set; }
    }
}
