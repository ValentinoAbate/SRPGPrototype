using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;

namespace Collections.Graphs.AdjMatrix
{
    public abstract class AdjMatrixGraph<T> : IGraph<T>
    {
        public const int noEdge = 0;
        public IEnumerable<Vertex<T>> Vertices => vertices;
        protected readonly List<Vertex<T>> vertices;

        protected List<List<int>> adjMatrix;

        public AdjMatrixGraph(IEnumerable<T> vertices)
        {
            int numVertices = vertices.Count();
            adjMatrix = new List<List<int>>();
            foreach(int i in Enumerable.Range(0, numVertices))
                adjMatrix.Add(Enumerable.Repeat(noEdge, numVertices).ToList());
            this.vertices = new List<Vertex<T>>();
            var vertArr = vertices.ToArray();
            foreach (int i in Enumerable.Range(0, vertices.Count()))
                this.vertices.Add(new Vertex<T>(i, vertArr[i]));
        }

        public abstract void AddEdge(Vertex<T> from, Vertex<T> to);

        public abstract void RemoveEdge(Vertex<T> from, Vertex<T> to);

        public Vertex<T> AddVertex(T value)
        {
            adjMatrix.ForEach((l) => l.Add(noEdge));
            adjMatrix.Add(new List<int>(Enumerable.Repeat(noEdge, vertices.Count + 1)));
            var newVertex = new Vertex<T>(vertices.Count, value);
            vertices.Add(newVertex);
            return newVertex;
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
            var adjList = adjMatrix[v.index];
            return vertices.Where((v2) => adjList[v2.index] > 0);
        }
    }
}