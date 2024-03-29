﻿using System;
using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public interface IGraphPathFinder
    {
        event Action<object, int, int, int> OnCellViewedEvent;

        IList<IGraphNode> GetPath(IGraph map, IGraphNode start, IGraphNode stop);
    }
}