using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private int _vertices;
    private List<int>[] _wayList;
    [SerializeField]
    GraphNode[] nodes;
    public GraphNode[] Nodes
    {
        get { return nodes; }
    }
    [SerializeField]
    GameObject wayPref;
    [SerializeField]
    List<int> firstWaysInspector;
    [SerializeField]
    List<int> secondWaysInspector;
    GraphNode selectedNode;
    public GraphNode SelectedNode { get { return selectedNode; } set { selectedNode = value; if (value == null) waitSecondNode = false; } }
    bool waitSecondNode = false;
    public bool WaitSecondNode {  get { return waitSecondNode; } }

    int jamNumber = 0;
    public int JamNumber { get { return jamNumber; } }
    public void JamNumberIncrease()
    {
        jamNumber++;
        Debug.Log("Number of JAM is " + jamNumber);
    }
    public void JamNumberDecrease()
    {
        jamNumber--;
        Debug.Log("Number of JAM is " + jamNumber);
    }

    public void Init()
    {
        nodes = transform.GetComponentsInChildren<GraphNode>();
        foreach (GraphNode node in nodes)
        {
            node.Init();
        }
        _vertices = nodes.Length;
        _wayList = new List<int>[_vertices];
        for(int i = 0; i < _vertices; i++)
        {
            _wayList[i] = new List<int>();
        }
        for(int i = 0; i < firstWaysInspector.Count; i++)
        {
            AddEdge(firstWaysInspector[i], secondWaysInspector[i]);
        }
        DrawGraph();
    }

    private void Update()
    {
        if (selectedNode != null)
        {
            if (Input.GetKeyDown(KeyCode.R) && selectedNode.State == GraphNode.NodeStates.PeanutButter)
            {
                // node is jam
                selectedNode.SetAsJam();
            }
            if (Input.GetKeyDown(KeyCode.S) && selectedNode.State == GraphNode.NodeStates.Jammed)
            {
                selectedNode.SetAsShield();
            }
            if(Input.GetKeyDown(KeyCode.W))
            {
                waitSecondNode = true;
            }
        }
    }

    public void JoinNodeToSelect(GraphNode node)
    {
        if(selectedNode != null)
        {
            if ((int)selectedNode.State > 1 && (int)node.State > 1)
            {
                int sIndex = GetNodeIndex(selectedNode);
                int nIndex = GetNodeIndex(node);
                if (!_wayList[sIndex].Contains(nIndex) && !_wayList[nIndex].Contains(sIndex))
                {
                    AddEdge(sIndex, nIndex);
                    DrawGraph();
                }
            }
        }
        waitSecondNode = false;
    }

    public GraphNode GetNode(int i)
    {
        return nodes[i];
    }

    public int GetNodeIndex(GraphNode node)
    {
        return Array.IndexOf(nodes, node);
    }

    public void FreeNode(int nodeIndex)
    {
        _wayList[nodeIndex] = new List<int>();
        for (int i = 0; i < _vertices; i++)
        {
            RemoveEdge(i, nodeIndex);
        }
        DrawGraph();
    }

    void AddEdge(int v, int w)
    {
        _wayList[v].Add(w);
    }

    void RemoveEdge(int v, int w)
    {
        _wayList[v].Remove(w);
    }

    void PrintGraph()
    {
        for (int i = 0; i < _vertices; i++)
        {
            Debug.Log("Vertex " + i + ": ");
            foreach (var vertex in _wayList[i])
                Debug.Log(vertex + " ");
        }
    }

    void DrawGraph(bool destroyOld = true)
    {
        if (destroyOld) 
        {
            var oldLines = transform.GetComponentsInChildren<LineRenderer>();
            for(int i = 0; i < oldLines.Length;  i++)
            {
                Destroy(oldLines[i].gameObject);
            }
        }

        for(int i = 0; i < _wayList.Length; i++)
        {
            for (int j = 0; j < _wayList[i].Count; j++)
            {
                LineRenderer line = Instantiate(wayPref, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
                line.SetPosition(0, GetNode(i).transform.position);
                line.SetPosition(1, GetNode(_wayList[i][j]).transform.position);
            }
        }
    }
}
