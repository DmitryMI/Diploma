using System.Collections;
using System.Collections.Generic;

namespace PathFinders.Graphs.SimpleTypes
{
    public class GraphNodeEnumerator<T> : IEnumerator<T> where T: GraphNode
    {
        private int _currentindex = -1;

        private T _node;

        public GraphNodeEnumerator(T node)
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

        public T Current => (T)_node[_currentindex];
        object IEnumerator.Current => _node[_currentindex];
    }
}
