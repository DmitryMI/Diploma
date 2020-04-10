namespace PathFinders.Graphs.Hierarchical
{
    public interface ICellCluster : ICellMap
    {
        int Level { get; }
        Vector2Int LeftBottom { get; }
        ICellMap Map { get; }
    }
}