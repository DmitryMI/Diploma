namespace PathFinders.Graphs.Hierarchical
{
    public interface ICellCluster : ICellMap
    {
        Vector2Int LeftBottom { get; }
        ICellMap Map { get; }
    }
}