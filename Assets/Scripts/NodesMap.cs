using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodesMap : MonoBehaviour
{
    Graph mainGraph;
    public Graph MainGraph { get { return mainGraph; } }
    [SerializeField]
    Controller controller;
    public Controller Controller { get { return controller; } }

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

    [SerializeField]
    int maxJamNumber = 0;
    public int MaxJamNumber { get { return maxJamNumber; } }
    public void MaxJamNumberIncrease() => maxJamNumber++;
    public void MaxJamNumberDecrease() => maxJamNumber--;

    [SerializeField]
    int currentJamNumber = 0;
    public int CurrentJamNumber { get { return currentJamNumber; } private set { currentJamNumber = value; if (currentJamNumber <= 0) NextTurn(); } }
    public void CurrentJamNumberIncrease() => CurrentJamNumber++;
    public void CurrentJamNumberDecrease() => CurrentJamNumber--;

    public void NextTurn()
    {
        ButterTurn();
        CurrentJamNumber = MaxJamNumber;
        Debug.Log("Next Turn");
    }

    public void ButterTurn()
    {
        var butterNodes = new List<GraphNode>();
        for(int i = 0; i < nodes.Length; i++)
        {
            if(GetNode(i).IsPeanutButter)
                butterNodes.Add(GetNode(i));
        }

        for(int i = 0; i < butterNodes.Count; i++)
        {
            var neighbors = mainGraph.GetNeighbors(butterNodes[i].NodeIndex).Where(e => !GetNode(e).IsPeanutButter).ToList();
            if(neighbors != null && neighbors.Count() > 0)
            {
                GetNode(neighbors[0]).SetAsButter();
            }
        }
    }

    public void Init()
    {
        nodes = transform.GetComponentsInChildren<GraphNode>();
        foreach (GraphNode node in nodes)
        {
            node.Init();
        }
        var _wayList = new List<int>[nodes.Length];
        for(int i = 0; i < nodes.Length; i++)
        {
            _wayList[i] = new List<int>();
        }
        for(int i = 0; i < firstWaysInspector.Count; i++)
        {
            _wayList[firstWaysInspector[i]].Add(secondWaysInspector[i]);
            _wayList[secondWaysInspector[i]].Add(firstWaysInspector[i]);
        }
        mainGraph = new Graph(nodes.Length, _wayList);
        DrawGraph();
    }

    private void Update()
    {
        if (selectedNode != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (selectedNode.IsBread)
                {
                    selectedNode.SetAsJamSandwich();
                    MaxJamNumberIncrease();
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (selectedNode.IsBread)
                {
                    selectedNode.SetAsJamSandwich();
                    MaxJamNumberIncrease();
                }
                else if (selectedNode.IsJammed || selectedNode.IsPeanutButter)
                {
                    selectedNode.SetAsShield();
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (!selectedNode.IsJamSandwich)
                    waitSecondNode = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextTurn();
        }
    }

    public void JoinNodeToSelect(GraphNode node)
    {
        if (selectedNode == null)
            throw new ArgumentNullException();
        if(node == null)
            throw new ArgumentNullException();
        if (node == selectedNode)
            throw new ArgumentException();

        if ((int)selectedNode.State > 1 && (int)node.State > 1)
        {
            int sIndex = GetNodeIndex(selectedNode);
            int nIndex = GetNodeIndex(node);
            if (!mainGraph.GetNeighbors(sIndex).Contains(nIndex) && !mainGraph.GetNeighbors(nIndex).Contains(sIndex))
            {
                mainGraph.AddEdge(sIndex, nIndex);
                DrawGraph();
                CurrentJamNumberDecrease();
            }
        }
        waitSecondNode = false;
    }

    public GraphNode GetNode(int i)
    {
        if (i < 0 || i > mainGraph.Vertices - 1)
            throw new ArgumentOutOfRangeException();

        return nodes[i];
    }

    public int GetNodeIndex(GraphNode node)
    {
        if (node == null)
            throw new ArgumentNullException();

        return Array.IndexOf(nodes, node);
    }

    public void FreeNode(int nodeIndex)
    {
        if (nodeIndex < 0 || nodeIndex > mainGraph.Vertices - 1)
            throw new ArgumentOutOfRangeException();

        mainGraph.GetNeighbors(nodeIndex).Clear();
        for (int i = 0; i < mainGraph.Vertices; i++)
        {
            mainGraph.RemoveEdge(i, nodeIndex);
        }
        DrawGraph();
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

        for(int i = 0; i < mainGraph.Vertices; i++)
        {
            for (int j = 0; j < mainGraph.GetNeighbors(i).Count; j++)
            {
                LineRenderer line = Instantiate(wayPref, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
                line.SetPosition(0, GetNode(i).transform.position);
                line.SetPosition(1, GetNode(mainGraph.GetNeighbors(i)[j]).transform.position);
            }
        }
    }
}
