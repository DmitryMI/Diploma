namespace PathFindingAlgorithms
{
    public struct Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsValid { get; set; }

        public Vector2(int x, int y)
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
            if (obj is Vector2 vec)
            {
                return X == vec.X && Y == vec.Y;
            }

            return false;
        }

        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y && IsValid == other.IsValid;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
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