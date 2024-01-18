using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph
{
    private int _vertices;
    public int Vertices { get { return _vertices; } }
    private List<int>[] _edgesList;

    public Graph(int vertNum, List<int>[] edges)
    {
        _vertices = vertNum; _edgesList = edges;
    }

    public List<int> GetNeighbor(int i)
    {
        return _edgesList[i];
    }

    public void AddEdge(int v, int w)
    {
        if ((v < 0 || v > _vertices - 1) || (w < 0 || w > _vertices - 1))
            throw new ArgumentOutOfRangeException();

        _edgesList[v].Add(w);
    }

    public void RemoveEdge(int v, int w)
    {
        if ((v < 0 || v > _vertices - 1))
            throw new ArgumentOutOfRangeException();

        _edgesList[v].Remove(w);
    }

    public bool BreadthFirstSearch(int startVert, int endVert, List<bool> whoIsVisited = null)
    {
        List<bool> vertIsVisited = new List<bool>();
        for(int i = 0; i < Vertices; i++)
        {
            vertIsVisited.Add(false);
        }

        foreach(var neighbor in GetNeighbor(startVert))
        {
            if (!vertIsVisited[neighbor])
            {
                if (BreadthFirstSearch(neighbor, endVert, vertIsVisited))
                    return true;
            }
        }

        return false;
    }

    public void PrintGraph()
    {
        for (int i = 0; i < _vertices; i++)
        {
            Debug.Log("Vertex " + i + ": ");
            foreach (var vertex in _edgesList[i])
                Debug.Log(vertex + " ");
        }
    }
}
