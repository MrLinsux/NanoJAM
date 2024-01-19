using System;
using System.Collections.Generic;
using System.Linq;
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

    public List<int> GetNeighbors(int i)
    {
        if (i < 0 || i > _vertices - 1)
            throw new ArgumentOutOfRangeException();

        //var neighbors = new List<int>(_edgesList[i]);
        //for(int j = 0; j < i; j++)
        //{
        //    if (_edgesList[j].Contains(i))
        //    {
        //        neighbors.Add(j);
        //    }
        //}
        //for (int j = i+1; j < _vertices; j++)
        //{
        //    if (_edgesList[j].Contains(i))
        //    {
        //        neighbors.Add(j);
        //    }
        //}

        //return neighbors;
        return _edgesList[i];
    }

    public void AddEdge(int v, int w)
    {
        if ((v < 0 || v > _vertices - 1) || (w < 0 || w > _vertices - 1))
            throw new ArgumentOutOfRangeException();

        _edgesList[w].Add(v);
        _edgesList[v].Add(w);
    }

    public void RemoveEdge(int v, int w)
    {
        if ((v < 0 || v > _vertices - 1))
            throw new ArgumentOutOfRangeException();

        _edgesList[w].Remove(v);
        _edgesList[v].Remove(w);
    }

    public bool BreadthFirstSearch(int startVert, int endVert, List<bool> vertIsVisited = null)
    {
        if (startVert == endVert)
            return true;

        if (vertIsVisited == null)
        {
            vertIsVisited = new List<bool>();
            for (int i = 0; i < Vertices; i++)
            {
                vertIsVisited.Add(false);
            }
        }

        if (vertIsVisited[startVert])
            return false;
        vertIsVisited[startVert] = true;

        foreach(var neighbor in GetNeighbors(startVert))
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
