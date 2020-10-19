using System;

namespace Pathfinding.Collections
{
    /// <summary>
    /// A c# priority queue implementation for pathfinding, based on a binary min heap
    /// </summary>
    public class PriorityQueue<T>
    {
        private readonly BinaryMinHeap<Item> items;

        public bool Empty { get => items.Empty; }

        public PriorityQueue()
        {
            items = new BinaryMinHeap<Item>();
        }
        public PriorityQueue(int capacity)
        {
            items = new BinaryMinHeap<Item>(capacity);
        }

        public T Peek()
        {
            if (Empty)
                return default;
            return items.PeekMin().item;
        }

        public T Dequeue()
        {
            return items.ExtractMin().item;
        }

        public void Enqueue(T item, float priority)
        {
            items.Insert(new Item(priority, item));
        }

        private readonly struct Item : IComparable<Item>
        {
            public readonly float priority;
            public readonly T item;

            public Item(float priority, T item)
            {
                this.priority = priority;
                this.item = item;
            }

            public int CompareTo(Item other)
            {
                return priority.CompareTo(other.priority);
            }
        }

    }
}