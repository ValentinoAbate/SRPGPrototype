using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Collections.Graphs.AdjList
{
    public abstract class AdjListGraph<T> : IGraph<T>
    {
        public AdjListGraph(IEnumerable<T> vertices)
        {
            foreach (var v in vertices)
                AddVertex(v);
        }

        protected readonly List<List<Vertex<T>>> adjList = new List<List<Vertex<T>>>();

        public IEnumerable<Vertex<T>> Vertices => vertices;
        protected readonly List<Vertex<T>> vertices = new List<Vertex<T>>();

        public abstract void AddEdge(Vertex<T> from, Vertex<T> to);

        public abstract void RemoveEdge(Vertex<T> from, Vertex<T> to);

        public Vertex<T> AddVertex(T value)
        {
            vertices.Add(new Vertex<T>(adjList.Count, value));
            adjList.Add(new List<Vertex<T>>());
            return vertices[vertices.Count - 1];
        }

        public bool HasEdge(Vertex<T> from, Vertex<T> to)
        {
            if (!Contains(to))
                throw new System.Exception("Graph Exception: vertex is not in the graph");
            return Adjacent(from).Contains(to);
        }

        public bool Contains(Vertex<T> vertex)
        {
            return vertex.index < vertices.Count;
        }

        public IEnumerable<Vertex<T>> Adjacent(Vertex<T> v)
        {
            if (!Contains(v))
                throw new System.Exception("Graph Exception: vertex is not in the graph");
            return adjList[v.index];
        }
    }
}