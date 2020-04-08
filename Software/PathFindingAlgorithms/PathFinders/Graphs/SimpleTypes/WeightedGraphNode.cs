using System;
using System.Collections.Generic;

namespace PathFinders.Graphs.SimpleTypes
{
    public class WeightedGraphNode<T> : GraphNode, IWeightedGraphNode<T>
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

        public T GetWeight(WeightedGraphNode<T> node)
        {
            int index = IndexOf(node);
            if (index < 0)
                throw new ArgumentException("Node is not connected");
            return _weights[index];
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

        public void SetWeightSymmetrical(WeightedGraphNode<T> node, T weight)
        {
            int index = IndexOf(node);
            if (index < 0)
                throw new ArgumentException("Node is not connected");
            _weights[index] = weight;
            node.SetWeight(this, weight);
        }

        public void SetWeight(WeightedGraphNode<T> node, T weight)
        {
            int index = IndexOf(node);
            if(index < 0)
                throw new ArgumentException("Node is not connected");
            _weights[index] = weight;
        }

        public override void Add(GraphNode item)
        {
            Connections.Add(item);
            Weights.Add(InfinityWeight);
        }

        public override void Clear()
        {
            Weights.Clear();
            Connections.Clear();
        }

        public override bool Remove(GraphNode item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;
            Connections.RemoveAt(index);
            _weights.RemoveAt(index);
            return true;
        }
    

        public override void Insert(int index, GraphNode item)
        {
            Connections.Insert(index, item);
            Weights.Insert(index, InfinityWeight);
        }

        public override void RemoveAt(int index)
        {
            Weights.RemoveAt(index);
            Connections.RemoveAt(index);
        }

        public ICollection<IWeightedGraphNode<T>> GetConnectedWeightedNodes()
        {
            List<IWeightedGraphNode<T>> result = new List<IWeightedGraphNode<T>>(Connections.Count);
            foreach (var connection in Connections)
            {
                result.Add((IWeightedGraphNode<T>)connection);
            }

            return result;
        }

        public ICollection<T> GetConnectionWeights()
        {
            return _weights;
        }

        public T InfinityWeight { get; set; }
    }
}
