namespace PathFinders.Graphs.Hierarchical
{
    public interface ICellCluster : ICellFragment
    {
        ICellMap Map { get; }
    }
}