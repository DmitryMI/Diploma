using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Graphs.SimpleTypes
{
    public class WeightedGraphNodeEnumerator<T> : IEnumerator<IWeightedGraphNode<T>>
    {
        private int _currentindex = -1;

        private WeightedGraphNode<T> _node;

        public WeightedGraphNodeEnumerator(WeightedGraphNode<T> node)
        {
            _node = node;
        }

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            if (_currentindex < _node.Count - 1)
            {
                _currentindex++;
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            _currentindex = -1;
        }

        public IWeightedGraphNode<T> Current => (IWeightedGraphNode<T>)_node[_currentindex];
        object IEnumerator.Current => _node[_currentindex];
    }
}
