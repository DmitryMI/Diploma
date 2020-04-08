using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders
{
    public static class Steps
    {
        public static Vector2Int[] GetSteps(NeighbourMode neighbourMode)
        {
            switch (neighbourMode)
            {
                case NeighbourMode.SideOnly:
                    return new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
                case NeighbourMode.SidesAndDiagonals:
                    Vector2Int[] result = new Vector2Int[]
                    {
                        new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1),
                        new Vector2Int(-1, -1), new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1),   
                    };
                    return result;
                default:
                    throw new ArgumentOutOfRangeException(nameof(neighbourMode), neighbourMode, null);
            }
        }
    }
}
