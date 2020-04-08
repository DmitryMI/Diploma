namespace PathFindingAlgorithms
{
    public interface IMap
    {
        bool IsPassable(int x, int y);
        bool IsInBounds(int x, int y);

        int Width { get; }
        int Height { get; }
    }
}