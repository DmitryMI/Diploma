﻿using System;
using System.Collections.Generic;

namespace PathFinders
{
    public interface ICellPathFinder
    {
        event Action<object, int, int, int> OnCellViewedEvent;

        IList<Vector2Int> GetPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode);
        IList<Vector2Int> GetSmoothedPath(ICellMap map, Vector2Int start, Vector2Int stop, NeighbourMode neighbourMode);

        void AddObstacle(ICellFragment cellCluster);

        void ClearObstacles();
        void RecalculateObstacles(NeighbourMode neighbourMode = NeighbourMode.SidesAndDiagonals);

        void Initialize(ICellMap mapBase);
    }
}