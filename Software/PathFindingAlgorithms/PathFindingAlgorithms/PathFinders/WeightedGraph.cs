using System;
using System.Collections.Generic;
using System.Text;

namespace PathFindingAlgorithms.PathFinders
{
    public class WeightedGraph<T>
    {
        private readonly T[,] _weights;

        public int Count { get; }
        public WeightedGraph(int count, T infinityValue)
        {
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

        public WeightedGraph(T[,] weights)
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
    }
}
