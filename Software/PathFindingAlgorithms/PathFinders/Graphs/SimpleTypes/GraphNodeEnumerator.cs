using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Graphs.SimpleTypes
{
    public class GraphNodeEnumerator : IEnumerator<IGraphNode>
    {
        private int _currentindex = -1;

        private GraphNode _node;

        public GraphNodeEnumerator(GraphNode node)
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

        public IGraphNode Current => _node[_currentindex];
        object IEnumerator.Current => _node[_currentindex];
    }
}
