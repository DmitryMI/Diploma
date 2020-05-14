using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinders.Algorithms.HpaStar
{
    public class LayeredCellMap : ICellMap
    {
        private ICellMap _layerBase;
        private List<ICellMap> _layers = new List<ICellMap>();
        private List<Vector2Int> _layerOffsets = new List<Vector2Int>();

        public bool IsPassable(int x, int y)
        {
            if (!IsInBounds(x, y))
                return false;

            if (!_layerBase.IsPassable(x, y))
                return false;

            for (var i = 0; i < _layers.Count; i++)
            {
                if(!IsLayerCellPassable(i, x, y))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsLayerCellPassable(int index, int x, int y)
        {
            ICellMap layer = _layers[index];
            Vector2Int offset = _layerOffsets[index];

            int shiftedX = x - offset.X;
            int shiftedY = y - offset.Y;

            if (!layer.IsInBounds(shiftedX, shiftedY))
            {
                return true;
            }
            else
            {
                bool layerIsPassable = layer.IsPassable(shiftedX, shiftedY);
                return layerIsPassable;
            }
        }

        public bool IsInBounds(int x, int y)
        {
            // Use only layerBase
            return _layerBase.IsInBounds(x, y);
        }

        public int Width => _layerBase.Width;

        public int Height => _layerBase.Height;

        public LayeredCellMap(ICellMap layerBase)
        {
            _layerBase = layerBase;
        }

        public void AddLayer(ICellMap layer)
        {
            _layers.Add(layer);
            _layerOffsets.Add(new Vector2Int(0, 0));
        }

        public void AddFragment(ICellFragment fragment)
        {
            _layers.Add(fragment);
            _layerOffsets.Add(fragment.LeftBottom);
        }

        public void RemoveLayer(int i)
        {
            _layers.RemoveAt(i);
            _layerOffsets.RemoveAt(i);
        }

        public void RemoveLayer(ICellMap layer)
        {
            int index = _layers.IndexOf(layer);
            RemoveLayer(index);
        }

        public void ClearLayers()
        {
            _layers.Clear();
            _layerOffsets.Clear();
        }
    }
}
