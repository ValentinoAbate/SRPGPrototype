using System.Collections.Generic;
using System.Linq;

namespace Collections.Graphs.AdjMatrix
{
    public class WeightedGraph<T> : AdjMatrixGraph<T>, IWeightedGraph<T>
    {
        public WeightedGraph(IEnumerable<T> vertices) : base(vertices) { }
        public WeightedGraph(params T[] vertices) : this(vertices.AsEnumerable()) { }

        public void AddEdge(Vertex<T> from, Vertex<T> to, int weight)
        {
            if (!Contains(from))
                throw new System.Exception("Graph Exception: Attempted to add an edge from a vertex that isn't in this graph");
            if (!Contains(to))
                throw new System.Exception("Graph Exception: Attempted to add an edge to a vertex that isn't in this graph");
            if (HasEdge(from, to))
                return;
            adjMatrix[from.index][to.index] = weight;
            adjMatrix[to.index][from.index] = weight;
        }

        public override void AddEdge(Vertex<T> from, Vertex<T> to) => AddEdge(from, to, 1);

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

        public int Weight(Vertex<T> from, Vertex<T> to)
        {
            return adjMatrix[from.index][to.index];
        }
    }
}