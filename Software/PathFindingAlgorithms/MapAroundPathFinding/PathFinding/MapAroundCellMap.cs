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
    public class MapAroundCellMap : ICellMap
    {
        private Map _mapAroundMap;

        private List<Polygon> _obstacles;

        private bool[,] _mapCells;

        private BoundingRectangle _boundingRectangle;

        private double _cellWidth;
        private double _cellHeight;

        

        

        public MapAroundCellMap(Map mapAroundMap, BoundingRectangle mapBoundaries, float cellWidth, float cellHeight)
        {
            _mapAroundMap = mapAroundMap;
            _obstacles = new List<Polygon>();
            _boundingRectangle = mapBoundaries;

            _cellWidth = cellWidth;
            _cellHeight = cellHeight;

            int cellMapWidth = (int)Math.Round(mapBoundaries.Width / cellWidth);
            int cellMapHeight = (int)Math.Round(mapBoundaries.Height / cellHeight);
            _mapCells = new bool[cellMapWidth, cellMapHeight];
        }

        private void FillPolygon(List<Edge> edges)
        {
            PolygonFiller filler = new PolygonFiller(OnCellUpdate);
            filler.FillPolygon(edges);
        }

        private void OnCellUpdate(int x, int y, bool filled)
        {
            _mapCells[x, y] = filled;
        }

        private void RegisterContour(Contour contour)
        {
            List<Edge> edges = new List<Edge>(contour.CoordinateCount / 2);
            List<Vector2Int> vertices = new List<Vector2Int>(contour.CoordinateCount);
            Vector2Int prevPoint = default;
            for (int i = 0; i < contour.CoordinateCount; i++)
            {
                double xShifted = contour.Vertices[i].X - _boundingRectangle.MinX;
                double yShifted = contour.Vertices[i].Y - _boundingRectangle.MinY;
                int xInt = (int)Math.Round(xShifted / _cellWidth);
                int yInt = (int)Math.Round(yShifted / _cellHeight);

                Vector2Int point = new Vector2Int(xInt, yInt);
                vertices.Add(point);

                if (i > 0)
                {
                    Edge edge = new Edge(prevPoint, point);
                    edges.Add(edge);
                }

                prevPoint = point;
            }

            FillPolygon(edges);
        }

        private void RegisterObstacle(Polygon polygon)
        {
            foreach (var contour in polygon.Contours)
            {
                RegisterContour(contour);
            }
        }

        public void AddPolygonObstaclesLayer(FeatureLayer layer)
        {
            foreach (var polygon in layer.Polygons)
            {
                _obstacles.Add(polygon.Polygon);
            }
        }

        public bool IsPassable(int x, int y)
        {
            return _mapCells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            return _mapCells[x, y];
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
