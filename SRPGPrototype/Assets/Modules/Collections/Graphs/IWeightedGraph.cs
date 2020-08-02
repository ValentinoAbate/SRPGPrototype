namespace Collections.Graphs
{
    public interface IWeightedGraph<T> : IGraph<T>
    {
        void AddEdge(Vertex<T> v1, Vertex<T> v2, int weight);

        int Weight(Vertex<T> v1, Vertex<T> v2);
    }
}