namespace PathFinders
{
    public interface ICellMap
    {
        bool IsPassable(int x, int y);
        bool IsInBounds(int x, int y);

        int Width { get; }
        int Height { get; }
    }
}