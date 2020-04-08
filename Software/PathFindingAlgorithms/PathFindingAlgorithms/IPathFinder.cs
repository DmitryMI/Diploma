using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PathFindingAlgorithms
{
    public interface IPathFinder
    {
        event Action<object, int, int, int> OnCellViewedEvent;

        IList<Vector2> GetPath(IMap map, Vector2 start, Vector2 stop);
    }
}