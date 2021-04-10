using System;
using MapAround.Geometry;

namespace MapAroundPathFinding.PathFinding
{
    public class RefCoordinate : ICoordinate
    {
        public const double ComparisonTolerance = 0.0000001f;
        private readonly bool _isReadOnly;
        public double X { get; set; }
        public double Y { get; set; }
        public bool Equals(ICoordinate p)
        {
            return Math.Abs(p.X - p.X) < ComparisonTolerance && Math.Abs(p.Y - Y) < ComparisonTolerance;
        }

        public bool ExactEquals(ICoordinate p)
        {
            return p.X == X && p.Y == p.Y;
        }

        public void Translate(double x, double y)
        {
            X += x;
            Y += y;
        }

        public double[] Values()
        {
            return new[] {X, Y};
        }

        public ICoordinate ReadOnlyCopy()
        {
            return (ICoordinate)Clone();
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
        }

        public RefCoordinate(double x, double y)
        {
            X = x;
            Y = y;
            _isReadOnly = false;
        }

        public object Clone()
        {
            return new RefCoordinate(X, Y);
        }
    }
}