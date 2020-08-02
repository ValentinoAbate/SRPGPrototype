using System;

namespace Collections.Graphs
{
    public class Vertex<T> : IEquatable<Vertex<T>>
    {
        public int index;
        public T value;
        public Vertex(int index, T value)
        {
            this.index = index;
            this.value = value;
        }

        public bool Equals(Vertex<T> other)
        {
            return other.index == index;
        }
    }
}