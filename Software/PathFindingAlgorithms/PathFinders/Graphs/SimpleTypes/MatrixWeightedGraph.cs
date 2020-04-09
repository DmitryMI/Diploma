using System.Collections.Generic;

namespace PathFinders.Graphs.SimpleTypes
{
    public class MatrixWeightedGraph<T> : IWeightedGraph<T>
    {
        private readonly T[,] _weights;

        public int Count { get; }

        public int Width { get; }
        public int Height { get; }

        public T InfinityWeight { get; set; }
        public MatrixWeightedGraph(int width, int height, T infinityValue)
        {
            Width = width;
            Height = height;
            int count = width * height;
            _weights = new T[count, count];
            Count = count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    _weights[i, j] = infinityValue;
                }
            }
        }

        public MatrixWeightedGraph(T[,] weights)
        {
            _weights = weights;
            Count = weights.GetLength(0);
        }

        public void SetWeightSymmetrical(int from, int to, T weight)
        {
            SetWeight(from, to, weight);
            SetWeight(to, from, weight);
        }

        public void SetWeight(int from, int to, T weight)
        {
            _weights[from, to] = weight;
        }

        public T GetWeight(int from, int to)
        {
            return _weights[from, to];
        }

        public T this[int from, int to] => GetWeight(from, to);
        public ICollection<IWeightedGraphNode<T>> GetWeightedGraphNodes()
        {
            List<IWeightedGraphNode<T>> result = new List<IWeightedGraphNode<T>>(Count);

            for (int i = 0; i < Count; i++)
            {
                WeightedGraphNode<T> node = new WeightedGraphNode<T>(InfinityWeight);
                int y = i / Width;
                int x = i - y * Width;
                node.Position = new Vector2Int(x, y);
                result.Add(node);
            }

            for (int i = 0; i < Count; i++)
            {
                WeightedGraphNode<T> node = (WeightedGraphNode<T>)result[i];

                for (int j = 0; j < Count; j++)
                {
                    WeightedGraphNode<T> connection  = (WeightedGraphNode<T>)result[i];
                    node.Add(connection);
                    node.SetWeight(node.Count - 1, _weights[i, j]);
                }
            }

            return result;
        }

        public ICollection<IGraphNode> GetGraphNodes()
        {
            ICollection<IWeightedGraphNode<T>> nodes = GetWeightedGraphNodes();

            List<IGraphNode> result = new List<IGraphNode>(nodes.Count);
            foreach (var node in nodes)
            {
                result.Add(node);
            }

            return result;
        }
    }
}
