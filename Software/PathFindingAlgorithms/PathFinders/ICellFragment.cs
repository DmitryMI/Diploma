namespace PathFinders
{
    public interface ICellFragment : ICellMap
    {
        Vector2Int LeftBottom { get; }
    }
}