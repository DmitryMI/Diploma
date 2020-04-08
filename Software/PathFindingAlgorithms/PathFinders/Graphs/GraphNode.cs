using System.Collections;
using System.Collections.Generic;

namespace PathFinders.Graphs
{
    public class GraphNode : IList<GraphNode>
    {
        public object Data { get; set; }

        public Vector2Int Position { get; set; }

        public T GetData<T>()
        {
            return (T) Data;
        }

        private List<GraphNode> _connections = new List<GraphNode>();

        public List<GraphNode> Connections => _connections;

        public IEnumerator<GraphNode> GetEnumerator()
        {
            return _connections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _connections).GetEnumerator();
        }

        public virtual void Add(GraphNode item)
        {
            _connections.Add(item);
        }

        public virtual void Clear()
        {
            _connections.Clear();
        }

        public bool Contains(GraphNode item)
        {
            return _connections.Contains(item);
        }

        public void CopyTo(GraphNode[] array, int arrayIndex)
        {
            _connections.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(GraphNode item)
        {
            return _connections.Remove(item);
        }

        public int Count => _connections.Count;

        public bool IsReadOnly => false;

        public int IndexOf(GraphNode item)
        {
            return _connections.IndexOf(item);
        }

        public virtual void Insert(int index, GraphNode item)
        {
            _connections.Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            _connections.RemoveAt(index);
        }

        public GraphNode this[int index]
        {
            get => _connections[index];
            set => _connections[index] = value;
        }
    }
}
