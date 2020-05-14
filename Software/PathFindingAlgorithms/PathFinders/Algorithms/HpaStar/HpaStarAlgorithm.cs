using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFinders.Graphs;
using PathFinders.Graphs.Hierarchical;
using PathFinders.Graphs.Hierarchical.SimpleTypes;

namespace PathFinders.Algorithms.HpaStar
{
    public class HpaStarAlgorithm : ICellPathFinder
    {
        public event Action<object, int, int, int> OnCellViewedEvent;

        public HierarchicalMap HierarchicalGraph { get; set; }

        public int ClusterSizeZero { get; private set; } = 16;

        private CellCluster _currentCellCluster;
        private bool _isInitialized = false;
        private ICellMap _mapBase;
        private LayeredCellMap _layeredCellMap;

        private List<ICellFragment> _userObstacles = new List<ICellFragment>();
        private List<ICellFragment> _removedObstacles = new List<ICellFragment>();

        public ICellMap LayeredCellMap => _layeredCellMap;

        

        private void PreBuildGraph(ICellMap map)
        {
            Stopwatch sw = new Stopwatch();
            Debug.WriteLine("Hierarchical graph construction started");
            sw.Start();
            HierarchicalMapGenerator generator = new HierarchicalMapGenerator();
            HierarchicalGraph = generator.GenerateMap(map, NeighbourMode.SideOnly, ClusterSizeZero);
            sw.Stop();
            Debug.WriteLine($"Hierarchical graph construction finished in {sw.Elapsed}");
        }

        public void Initialize(ICellMap mapBase, int clusterSize = 16)
        {
            _mapBase = mapBase;
            _layeredCellMap = new LayeredCellMap(_mapBase);
            ClusterSizeZero = clusterSize;
            PreBuildGraph(_layeredCellMap);
            _isInitialized = true;
        }

        public IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode)
        {
            if (!_isInitialized)
            {
                Initialize(map);
            }

            CellCluster startContainer = HierarchicalMapGenerator.GetContainingCluster(HierarchicalGraph.ZeroLevelClusters, ClusterSizeZero, ClusterSizeZero, start);
            CellCluster stopContainer = HierarchicalMapGenerator.GetContainingCluster(HierarchicalGraph.ZeroLevelClusters, ClusterSizeZero, ClusterSizeZero, stop);

            HierarchicalGraphNode startNode = new HierarchicalGraphNode();
            startNode.Position = start;
            startContainer.ConnectNode(startNode, neighbourMode);
            startNode.ParentCluster = startContainer;

            HierarchicalGraphNode stopNode = new HierarchicalGraphNode();
            stopNode.Position = stop;
            stopContainer.ConnectNode(stopNode, neighbourMode);
            stopNode.ParentCluster = stopContainer;

            AStarAlgorithm aStarAlgorithm = new AStarAlgorithm();
            _currentCellCluster = null;
            aStarAlgorithm.OnCellViewedEvent += OnAStarCellViewed;
            IList<IGraphNode> abstractPath = aStarAlgorithm.GetPath(null, startNode, stopNode);
            List<Vector2Int> path = null;
            if (abstractPath != null)
            {
                path = new List<Vector2Int>();
                for (var i = 0; i < abstractPath.Count - 1; i++)
                {
                    HierarchicalGraphNode nodeA = (HierarchicalGraphNode)abstractPath[i];
                    HierarchicalGraphNode nodeB = (HierarchicalGraphNode)abstractPath[i + 1];
                    if (nodeA.ParentCluster == nodeB.ParentCluster)
                    {
                        
                        CoordinateTransformer transformer =
                            new CoordinateTransformer(nodeA.ParentCluster, nodeA.ParentCluster.LeftBottom);
                        _currentCellCluster = nodeA.ParentCluster;
                        IList<Vector2Int> realPath = aStarAlgorithm.GetPath(transformer, nodeA.Position - transformer.Transform,
                            nodeB.Position - transformer.Transform, neighbourMode);

                        TransformPath(realPath, transformer.Transform);
                        realPath = Utils.GetInvertedList(realPath);
                        path.AddRange(realPath);
                    }
                }
                
            }

            DestroyConnections(startNode);
            DestroyConnections(stopNode);
            DestroyData(HierarchicalGraph.ZeroLevelClusters);

            return path;
        }

        private void DestroyConnections(HierarchicalGraphNode node)
        {
            foreach (var connection in node.Connections)
            {
                connection.Remove(node);
            }
        }

        private void DestroyData(CellCluster[,] cellClusters)
        {
            for (int i = 0; i < cellClusters.GetLength(0); i++)
            {
                for (int j = 0; j < cellClusters.GetLength(1); j++)
                {
                    DestroyData(cellClusters[i, j]);
                }
            }
        }

        private void DestroyData(CellCluster cluster)
        {
            foreach (var node in cluster.TransitionNodes)
            {
                node.Data = null;
            }
        }

        private void TransformPath(IList<Vector2Int> path, Vector2Int transform)
        {
            for (int i = 0; i < path.Count; i++)
            {
                path[i] += transform;
            }
        }

        private void OnAStarCellViewed(object sender, int x, int y, int d)
        {
            if (_currentCellCluster != null)
            {
                OnCellViewedEvent?.Invoke(this, x + _currentCellCluster.LeftBottom.X,
                    y + _currentCellCluster.LeftBottom.Y, d);
            }
            else
            {
                OnCellViewedEvent?.Invoke(this, x, y, d);
            }
        }

        public void AddObstacle(ICellFragment cellCluster)
        {
            _userObstacles.Add(cellCluster);
        }

        public void ClearObstacles()
        {
            _removedObstacles.AddRange(_userObstacles);
            _userObstacles.Clear();
        }

        public void RecalculateHierarchicalGraph(NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals)
        {
            HierarchicalMapGenerator generator = new HierarchicalMapGenerator();

            foreach (var removedObstacle in _removedObstacles)
            {
                _layeredCellMap.RemoveLayer(removedObstacle);
            }

            generator.UpdateGraph(_layeredCellMap, _removedObstacles, HierarchicalGraph, neighbourMode);

            _removedObstacles.Clear();
           
            foreach (var obstacle in _userObstacles)
            {
                _layeredCellMap.AddFragment(obstacle);
            }

            generator.UpdateGraph(_layeredCellMap, _userObstacles, HierarchicalGraph, neighbourMode);
        }
    }
}
