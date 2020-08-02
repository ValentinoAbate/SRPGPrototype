using System.Collections.Generic;
using System.Linq;

namespace Collections.Graphs.AdjList
{
    public class Graph<T> : AdjListGraph<T>
    {
        public Graph(IEnumerable<T> vertices) : base(vertices) { }
        public Graph(params T[] vertices) : this(vertices.AsEnumerable()) { }

        public override void AddEdge(Vertex<T> from, Vertex<T> to)
        {
            if (!Contains(from))
                throw new System.Exception("Graph Exception: Attempted to add an edge from a vertex that isn't in this graph");
            if (!Contains(to))
                throw new System.Exception("Graph Exception: Attempted to add an edge to a vertex that isn't in this graph");
            if (HasEdge(from, to))
                return;
            adjList[from.index].Add(to);
            adjList[to.index].Add(from);
        }

        public override void RemoveEdge(Vertex<T> from, Vertex<T> to)
        {
            if (!Contains(from))
                throw new System.Exception("Graph Exception: Attempted to remove an edge from a vertex that isn't in this graph");
            if (!Contains(to))
                throw new System.Exception("Graph Exception: Attempted to remove an edge to a vertex that isn't in this graph");
            if (!HasEdge(from, to))
                return;
            adjList[from.index].Remove(to);
            adjList[to.index].Remove(from);
        }
    }
}