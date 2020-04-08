﻿namespace PathFinders
{
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsValid { get; set; }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
            IsValid = true;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }


        public override bool Equals(object obj)
        {
            if (obj is Vector2Int vec)
            {
                return X == vec.X && Y == vec.Y;
            }

            return false;
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y && IsValid == other.IsValid;
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ IsValid.GetHashCode();
                return hashCode;
            }
        }
    }
}