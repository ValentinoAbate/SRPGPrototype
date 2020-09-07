using Collections.Graphs;
using Collections.Graphs.AdjList;
using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public const int maxWidth = 8;
    public const int minWidth = 5;
    public EncounterGenerator encounterGenerator;

    public Map Generate(MapData data)
    {
        var events = new List<Encounter>[data.Depth];
        // Generate Events
        foreach(int depth in Enumerable.Range(0, data.Depth))
        {
            events[depth] = new List<Encounter>();
            var encounterBank = data.GetEncounterData(depth);
            int width = RandomU.instance.RandomInt(minWidth, maxWidth);
            foreach(var i in Enumerable.Range(0, width))
            {
                var encounterData = RandomU.instance.Choice(encounterBank);
                string encounterName = "Encounter " + i.ToString();
                events[depth].Add(encounterGenerator.Generate(encounterData, encounterName));
            }
        }
        // Create the event graph and initial vertices to all depth 0 encounters
        Digraph<Encounter> eventGraph = new Digraph<Encounter>(events[0]);
        // Get the current vertices from those currently in the graph
        var currVertices = new List<Vertex<Encounter>>(eventGraph.Vertices);
        // Create the start vertex
        var startVertex = eventGraph.AddVertex(null);
        // Connect the starting vertex to all nodes at depth 0
        currVertices.ForEach((v) => eventGraph.AddEdge(startVertex, v));
        // Generate Graph
        foreach (int depth in Enumerable.Range(0, data.Depth - 1))
        {
            // Create vertices from next encounters
            var nextVertices = events[depth + 1].Select((e) => eventGraph.AddVertex(e)).ToList();
            foreach(var from in currVertices)
            {
                // Choose a random number of connections (or less if the nextVertices has few choices)
                int numConnections = Mathf.Min(RandomU.instance.RandomInt(2, 5), nextVertices.Count);
                var choices = new List<Vertex<Encounter>>(nextVertices);
                foreach(int connection in Enumerable.Range(0, numConnections))
                {
                    var to = RandomU.instance.Choice(choices);
                    eventGraph.AddEdge(from, to);
                    choices.Remove(to);
                }
            }
            currVertices = nextVertices;
        }
        return new Map(eventGraph, startVertex);
    }
}
