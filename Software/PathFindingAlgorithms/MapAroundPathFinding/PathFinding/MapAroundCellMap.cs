using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        public MapAroundCellMap(Map mapAroundMap, BoundingRectangle mapBoundaries, double cellWidth, double cellHeight)
        {
            _mapAroundMap = mapAroundMap;
            _obstacles = new List<Polygon>();
            _boundingRectangle = mapBoundaries;

            _cellWidth = cellWidth;
            _cellHeight = cellHeight;

            int cellMapWidth = (int)Math.Round(mapBoundaries.Width / cellWidth);
            int cellMapHeight = (int)Math.Round(mapBoundaries.Height / cellHeight);
            _mapCells = new bool[cellMapWidth, cellMapHeight];

            for (int x = 0; x < cellMapWidth; x++)
            {
                for (int y = 0; y < cellMapHeight; y++)
                {
                    _mapCells[x, y] = true;
                }
            }
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
                double xShifted = contour.Vertices[i].X - _boundingRectangle.MinX;
                double yShifted = contour.Vertices[i].Y - _boundingRectangle.MinY;
                int xInt = (int) Math.Round(xShifted / _cellWidth);
                int yInt = (int) Math.Round(yShifted / _cellHeight);

                Vector2Int point = new Vector2Int(xInt, yInt);

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

        private void RegisterObstacle(Polygon polygon)
        {
            foreach (var contour in polygon.Contours)
            {
                RegisterContour(contour);
            }
        }

        public void AddPolygonObstaclesLayer(FeatureLayer layer)
        {
            foreach (var polygonFeature in layer.Polygons)
            {
                var bounds = polygonFeature.BoundingRectangle;
                if (_boundingRectangle.ContainsRectangle(bounds))
                {
                    _obstacles.Add(polygonFeature.Polygon);
                    RegisterObstacle(polygonFeature.Polygon);
                }
            }
        }

        public bool IsPassable(int x, int y)
        {
            return _mapCells[x, y];
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
            //return _mapCells[x, y];
        }

        public Bitmap ToBitmap()
        {
            return ToBitmap(1);
        }

        private void DrawScaledPixel(Bitmap bmp, Color color, int x, int y, int scale)
        {
            double scaleHalf = (double)scale / 2;
            int xScaled = x * scale - (int)Math.Round(scaleHalf);
            int yScaled = y * scale - (int)Math.Round(scaleHalf);

            int borderMin = -(int) Math.Round(scaleHalf);
            int borderMax = (int)Math.Round(scaleHalf);

            for (int i = borderMin; i <= borderMax; i++)
            {
                int drawX = xScaled + i;
                if (drawX < 0 || drawX >= bmp.Width)
                {
                    continue;
                }
                for (int j = -borderMax; j <= borderMax; j++)
                {
                    int drawY = yScaled + j;
                    if (drawY < 0 || drawY >= bmp.Height)
                        continue;
                    bmp.SetPixel(drawX, drawY, color);
                }
            }
        }

        public Bitmap ToBitmap(int pixelScale)
        {
            Bitmap bitmap = new Bitmap(Width * pixelScale, Height * pixelScale);

            for (int x = 0; x < Width; x++)
            {
                for (int y= 0; y < Height; y++)
                {
                    if (!_mapCells[x, y])
                    {
                        DrawScaledPixel(bitmap, Color.Black, x, y, pixelScale);
                    }
                }
            }

            return bitmap;
        }

        public int Width => _mapCells.GetLength(0);
        public int Height => _mapCells.GetLength(1);
    }
}
