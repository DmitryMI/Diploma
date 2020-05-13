using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapAround.Geometry;
using MapAround.Mapping;
using MapAroundPathFinding.PathFinding.PolygonFillerUtil;
using PathFinders;

namespace MapAroundPathFinding.PathFinding
{
    public class PolygonCellFragment : ICellFragment
    {
        private readonly int _width;
        private readonly int _height;
        private bool _leftBottomHasValue = false;
        private Vector2Int _leftBottom;
        private readonly double _cellWidth;
        private readonly double _cellHeight;
        private readonly BoundingRectangle _boundingRectangle;
        private readonly BoundingRectangle _mapRectangle;

        private readonly bool[,] _mapCells;

        public PolygonCellFragment(Feature polygonFeature, BoundingRectangle mapBoundingRectangle, double cellWidth, double cellHeight)
        {
            if (polygonFeature.FeatureType != FeatureType.Polygon)
            {
                throw new ArgumentException("Feature must be Polygon-type");
            }

            _mapRectangle = mapBoundingRectangle;

            _cellWidth = cellWidth;
            _cellHeight = cellHeight;

            var featureBoundingRectangle = polygonFeature.BoundingRectangle;

            int cellMapWidth = (int)Math.Round(featureBoundingRectangle.Width / cellWidth);
            int cellMapHeight = (int)Math.Round(featureBoundingRectangle.Height / cellHeight);
            _width = cellMapWidth;
            _height = cellMapHeight;
            _mapCells = new bool[cellMapWidth, cellMapHeight];

            _boundingRectangle = featureBoundingRectangle;

            double xCoordinate = _boundingRectangle.MinX - mapBoundingRectangle.MinX;
            double yCoordinate = _boundingRectangle.MinY - mapBoundingRectangle.MinY;

            int minX = (int) Math.Round(xCoordinate / cellWidth);
            int minY = (int) Math.Round(yCoordinate / cellHeight);

            _leftBottom = new Vector2Int(minX, minY);
            _leftBottomHasValue = true;

            for (int x = 0; x < cellMapWidth; x++)
            {
                for (int y = 0; y < cellMapHeight; y++)
                {
                    _mapCells[x, y] = true;
                }
            }

            RegisterObstacle(polygonFeature.Polygon);
        }

        private void FillPolygon(List<Edge> edges)
        {
            PolygonFiller filler = new PolygonFiller(OnCellFilled);
            filler.FillPolygon(edges);
        }

        private void OnCellFilled(int x, int y, bool filled)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;

            _mapCells[x, y] = !filled;
        }

        private void RegisterContour(Contour contour)
        {
            List<Edge> edges = new List<Edge>(contour.CoordinateCount);
            Vector2Int prevPoint = default;
            Vector2Int firstPoint = default;
            for (int i = 0; i < contour.CoordinateCount; i++)
            {
                double xCoordinate = _boundingRectangle.MinX;
                double yCoordinate = _boundingRectangle.MinY;

                double xShifted = contour.Vertices[i].X - xCoordinate;
                double yShifted = contour.Vertices[i].Y - yCoordinate;
                int xInt = (int)Math.Round(xShifted / _cellWidth);
                int yInt = (int)Math.Round(yShifted / _cellHeight);

                Vector2Int point = new Vector2Int(xInt, yInt);
                //SetLeftBottomIfLesser(point);

                if (i == 0)
                {
                    firstPoint = point;
                }
                else if (i > 0)
                {
                    Edge edge = new Edge(prevPoint, point);
                    edges.Add(edge);
                }

                prevPoint = point;
            }

            edges.Add(new Edge(firstPoint, prevPoint));

            FillPolygon(edges);
        }

        private void SetLeftBottomIfLesser(Vector2Int cell)
        {
            if (!_leftBottomHasValue)
            {
                _leftBottom = cell;
                _leftBottomHasValue = true;
                return;
            }
            if (_leftBottom.X < cell.X)
            {
                _leftBottom.X = cell.X;
            }

            if (_leftBottom.Y < cell.Y)
            {
                _leftBottom.Y = cell.Y;
            }
        }

        private void RegisterObstacle(Polygon polygon)
        {
            foreach (var contour in polygon.Contours)
            {
                RegisterContour(contour);
            }
        }

        public bool IsPassable(int x, int y)
        {
            return _mapCells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public int Width
        {
            get => _width;
        }

        public int Height
        {
            get => _height;
        }

        public Vector2Int LeftBottom
        {
            get => _leftBottom;
        }
    }
}
