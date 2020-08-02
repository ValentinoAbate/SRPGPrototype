using System.Collections.Generic;

namespace Collections.Graphs
{
    public interface IGraph<T>
    {
        IEnumerable<Vertex<T>> Vertices { get; }

        Vertex<T> AddVertex(T value);

        void AddEdge(Vertex<T> from, Vertex<T> to);

        void RemoveEdge(Vertex<T> from, Vertex<T> to);

        bool HasEdge(Vertex<T> from, Vertex<T> to);

        bool Contains(Vertex<T> vertex);

        IEnumerable<Vertex<T>> Adjacent(Vertex<T> v);
    }
}