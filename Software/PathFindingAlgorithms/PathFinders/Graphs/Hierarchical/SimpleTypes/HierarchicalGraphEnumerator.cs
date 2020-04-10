using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Graphs.Hierarchical.SimpleTypes
{
    public class HierarchicalGraphEnumerator<T> : IEnumerator<IHierarchicalGraphNode<T>>
    {
        private readonly IGraphNode[,] _nodeMatrix;
        private int _currentX, _currentY;

        public HierarchicalGraphEnumerator(IGraphNode[,] nodeMatrix)
        {
            _nodeMatrix = nodeMatrix;
        }

        public void Dispose()
        {

        }

        public bool MoveNext()
        {
            _currentX++;
            if (_currentX == _nodeMatrix.GetLength(0))
            {
                _currentX = 0;
                _currentY++;
                if (_currentY == _nodeMatrix.GetLength(1))
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            _currentX = 0;
            _currentY = 0;
        }

        public IHierarchicalGraphNode<T> Current => (IHierarchicalGraphNode<T>) _nodeMatrix[_currentX, _currentY];
        object IEnumerator.Current => _nodeMatrix[_currentX, _currentY];
    }
}
