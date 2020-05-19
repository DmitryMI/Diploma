namespace PathFinders
{
    public interface IMutableCellMap : ICellMap
    {
        void SetPassable(int x, int y, bool isPassable);
    }
}