using System.Collections;
using System.Collections.Generic;

namespace PathFinders.Graphs.SimpleTypes
{
    public class GraphNode : IGraphNode, ICollection<IGraphNode>
    {
        private List<GraphNode> _connections = new List<GraphNode>();

        public object Data { get; set; }

        public ICollection<IGraphNode> GetConnectedNodes()
        {
            return this;
        }

        public Vector2Int Position { get; set; }

        public T GetData<T>()
        {
            return (T) Data;
        }

        

        public List<GraphNode> Connections => _connections;
        

        public virtual void Add(GraphNode item)
        {
            _connections.Add(item);
        }

        public void Add(IGraphNode item)
        {
            _connections.Add((GraphNode)item);
        }

        public void Clear()
        {
            _connections.Clear();
        }

        public bool Contains(IGraphNode item)
        {
            return _connections.Contains((GraphNode)item);
        }

        public void CopyTo(IGraphNode[] array, int arrayIndex)
        {
            for (int i = 0; i < _connections.Count; i++)
            {
                array[arrayIndex + i] = _connections[i];
            }
        }

        public bool Remove(IGraphNode item)
        {
            return _connections.Remove((GraphNode)item);
        }

        public int Count => _connections.Count;

        public bool IsReadOnly => false;

        public int IndexOf(GraphNode item)
        {
            return _connections.IndexOf(item);
        }
        
        public GraphNode this[int index]
        {
            get => _connections[index];
            set => _connections[index] = value;
        }

        public IEnumerator<IGraphNode> GetEnumerator()
        {
            return new GraphNodeEnumerator<GraphNode>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
