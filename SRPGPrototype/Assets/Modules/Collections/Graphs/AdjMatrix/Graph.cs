using System.Collections.Generic;
using System.Linq;

namespace Collections.Graphs.AdjMatrix
{
    public class Graph<T> : AdjMatrixGraph<T>
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
            adjMatrix[from.index][to.index] = 1;
            adjMatrix[to.index][from.index] = 1;
        }

        public override void RemoveEdge(Vertex<T> from, Vertex<T> to)
        {
            if (!Contains(from))
                throw new System.Exception("Graph Exception: Attempted to remove an edge from a vertex that isn't in this graph");
            if (!Contains(to))
                throw new System.Exception("Graph Exception: Attempted to remove an edge to a vertex that isn't in this graph");
            if (!HasEdge(from, to))
                return;
            adjMatrix[from.index][to.index] = noEdge;
            adjMatrix[to.index][from.index] = noEdge;
        }
    }
}