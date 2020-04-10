using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs.Hierarchical;
using PathFinders.Graphs.Hierarchical.SimpleTypes;

namespace PathFinders.Algorithms.HpaStar
{
    public class HpaStarAlgorithm : ICellPathFinder
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        public HierarchicalGraph<double> HierarchicalGraph { get; set; }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            if (HierarchicalGraph == null)
            {
                var hierarchyGraph = HierarchicalGraphGenerator.GenerateGraph(map, NeighbourMode.SideOnly, 8);
            }


        }
    }
}
