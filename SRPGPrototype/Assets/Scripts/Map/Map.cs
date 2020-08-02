using Collections.Graphs.AdjList;
using Collections.Graphs;
using System.Collections;
using System.Collections.Generic;

public class Map
{
    private Digraph<Encounter> events;
    public Vertex<Encounter> Current { get; set; }

    public Map(Digraph<Encounter> events, Vertex<Encounter> start)
    {
        this.events = events;
        Current = start;
    }

    public IEnumerable<Vertex<Encounter>> NextEncounters => events.Adjacent(Current);
}
