using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Algorithms;
using PathFinders.Algorithms.HpaStar;

namespace PathFinders
{
    public class CellPathFinderFactory : ICellPathFinder
    {
        private ICellPathFinder _instance;

        public CellPathFinderFactory(CellPathFinderAlgorithms algorithm)
        {
            switch (algorithm)
            {
                case CellPathFinderAlgorithms.LeeAlgorithm:
                    _instance = new LeeAlgorithm();
                    break;
                case CellPathFinderAlgorithms.BestFirstSearchAlgorithm:
                    _instance = new BestFirstSearch();
                    break;
                case CellPathFinderAlgorithms.AStarAlgorithm:
                    _instance = new AStarAlgorithm();
                    break;
                case CellPathFinderAlgorithms.HpaStarAlgorithm:
                    _instance = new HpaStarAlgorithm();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
            }
        }

        public event Action<object, int, int, int> OnCellViewedEvent
        {
            add => _instance.OnCellViewedEvent += value;
            remove => _instance.OnCellViewedEvent -= value;
        }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            return _instance.GetPath(map, start, stop, neighbourMode);
        }

        public IList<Vector2Int> GetSmoothedPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            return _instance.GetSmoothedPath(map, start, stop, neighbourMode);
        }

        public void AddObstacle(ICellFragment cellCluster)
        {
            _instance.AddObstacle(cellCluster);
        }

        public void ClearObstacles()
        {
            _instance.ClearObstacles();
        }

        public void RecalculateObstacles(NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals)
        {
            _instance.RecalculateObstacles(neighbourMode);
        }

        public void Initialize(ICellMap mapBase)
        {
            _instance.Initialize(mapBase);
        }
    }
}
