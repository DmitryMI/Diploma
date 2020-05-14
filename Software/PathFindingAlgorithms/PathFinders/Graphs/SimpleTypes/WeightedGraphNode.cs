using System;
using System.Collections.Generic;

namespace PathFinders.Graphs.SimpleTypes
{
    public class WeightedGraphNode<T> : GraphNode, IWeightedGraphNode<T>, ICollection<IWeightedGraphNode<T>>
    {
        private List<T> _weights = new List<T>();

        public List<T> Weights => _weights;

        public WeightedGraphNode(T infinityWeight)
        {
            InfinityWeight = infinityWeight;
        }

        public T GetWeight(int connectionIndex)
        {
            return _weights[connectionIndex];
        }

        public T GetWeight(IWeightedGraphNode<T> node)
        {
            int index = IndexOf((WeightedGraphNode<T>)node);
            if (index < 0)
            {
                return InfinityWeight;
                //throw new ArgumentException("Node is not connected");
            }

            return _weights[index];
        }

        public void SetWeight(IWeightedGraphNode<T> connection, T weight)
        {
            int connectionIndex = Connections.IndexOf((GraphNode)connection);
            if (connectionIndex == -1)
            {
                Connections.Add((GraphNode)connection);
                _weights.Add(weight);
            }
            else
            {
                _weights[connectionIndex] = weight;
            }
            
        }

        public int GetConnectionIndex(IWeightedGraphNode<T> connection)
        {
            return Connections.IndexOf((GraphNode)connection);
        }

        public void SetWeight(int connectionIndex, T weight)
        {
            _weights[connectionIndex] = weight;
        }

        public void SetWeightSymmetrical(int connectionIndex, T weight)
        {
            _weights[connectionIndex] = weight;
            WeightedGraphNode<T> node = (WeightedGraphNode<T>) Connections[connectionIndex];
            node.SetWeight(this, weight);
        }

        public void SetWeightSymmetrical(IWeightedGraphNode<T> node, T weight)
        {
            int index = IndexOf((WeightedGraphNode<T>)node);
            if (index < 0)
                throw new ArgumentException("Node is not connected");
            _weights[index] = weight;
            node.SetWeight(this, weight);
        }

        public override void Add(GraphNode item)
        {
            Connections.Add(item);
            Weights.Add(InfinityWeight);
        }

        public ICollection<IWeightedGraphNode<T>> GetConnectedWeightedNodes()
        {
            return this;
        }

        public ICollection<T> GetConnectionWeights()
        {
            return _weights;
        }

        public T InfinityWeight { get; set; }
        public bool IsInfinity(T weight)
        {
            if (weight is double d)
            {
                return Double.IsInfinity(d);
            }
            else
            {
                return weight.Equals(InfinityWeight);
            }
        }

        public new IEnumerator<IWeightedGraphNode<T>> GetEnumerator()
        {
            return new WeightedGraphNodeEnumerator<T>(this);
        }

        public void Add(IWeightedGraphNode<T> item)
        {
            base.Add(item);
        }

        public bool Contains(IWeightedGraphNode<T> item)
        {
            return base.Contains(item);
        }

        public void CopyTo(IWeightedGraphNode<T>[] array, int arrayIndex)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                array[arrayIndex + i] = (IWeightedGraphNode<T>) Connections[i];
            }
        }

        public bool Remove(IWeightedGraphNode<T> item)
        {
            return base.Remove(item);
        }
    }
}
