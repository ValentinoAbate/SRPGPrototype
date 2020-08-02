﻿using Collections.Graphs;
using Collections.Graphs.AdjList;
using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public EncounterGenerator encounterGenerator;

    public Map Generate(MapData data)
    {
        var events = new List<Encounter>[data.Depth];
        // Generate Events
        foreach(int depth in Enumerable.Range(0, data.Depth))
        {
            events[depth] = new List<Encounter>();
            var encounterBank = data.GetEncounterData(depth);
            int width = RandomU.instance.RandomInt(5, 10);
            foreach(var i in Enumerable.Range(0, width))
            {
                var encounterData = RandomU.instance.Choice(encounterBank);
                events[depth].Add(encounterGenerator.Generate(encounterData));
            }
        }
        // Create the event graph and initial vertices
        Digraph<Encounter> eventGraph = new Digraph<Encounter>(events[0]);
        var currVertices = new List<Vertex<Encounter>>(eventGraph.Vertices);
        var startVertex = RandomU.instance.Choice(currVertices);
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
