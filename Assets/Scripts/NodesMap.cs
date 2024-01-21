using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static GraphNode;

public class NodesMap : MonoBehaviour
{
    Graph mainGraph;
    public Graph MainGraph { get { return mainGraph; } }
    [SerializeField]
    Controller _controller;
    public Controller Controller { get { return _controller; } }

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
    GraphNode secondSelectedNode;
    public GraphNode SecondSelectNode
    { 
        get { return secondSelectedNode; } 
        set { secondSelectedNode = value; }
    }
    [SerializeField]
    NodeChangeHint nodeHintPanel;
    LineRenderer newWay;
    [SerializeField]
    int maxStepsToLose = 3;
    [SerializeField]
    int stepsToLose = 3;
    public void StepsToLoseDecrease()
    {
        stepsToLose--;
        if(stepsToLose <= 0)
        {
            _controller.GameOver();
        }
    }

    public LineRenderer NewWay { get { return newWay; } }
    public void StopDrawingNewWay()
    {
        Destroy(newWay.gameObject); newWay = null;
        if (secondSelectedNode != null && secondSelectedNode != selectedNode)
        {
            JoinNodeToSelect(secondSelectedNode);
            selectedNode.SetActiveOutline(false);
            secondSelectedNode.OnPointerEnter(null);
            secondSelectedNode = null;
        }
        else
        {
            selectedNode.SetActiveOutline(false);
        }
    }
    public NodeChangeHint NodeHintPanel { get { return nodeHintPanel; } }

    public GraphNode SelectedNode { get { return selectedNode; } set { selectedNode = value; } }

    bool canMakeJamSandwitch = true;

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
        bool canChangeAnyNode = false;
        var butterNodes = new List<GraphNode>();
        for(int i = 0; i < nodes.Length; i++)
        {
            if(GetNode(i).IsPeanutButter && !GetNode(i).IsShielded)
                butterNodes.Add(GetNode(i));
        }

        for(int i = 0; i < butterNodes.Count; i++)
        {
            var suitableNeighbors = mainGraph.GetNeighbors(butterNodes[i].NodeIndex).Where(e => !GetNode(e).IsPeanutButter && !GetNode(e).IsShielded && !GetNode(e).IsPeanutToaster).ToList();
            if(suitableNeighbors != null && suitableNeighbors.Count() > 0)
            {
                GetNode(suitableNeighbors[0]).SetAsButter();
                canChangeAnyNode = true;
            }
        }
        if(!canChangeAnyNode)
        {
            StepsToLoseDecrease();
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
        stepsToLose = maxStepsToLose;
    }

    private void Update()
    {
        if (selectedNode != null)
        {
            if(selectedNode.IsBread)
            {
                if(Input.GetKeyDown(KeyCode.Mouse0) && canMakeJamSandwitch)
                {
                    selectedNode.SetAsJamSandwich();
                    canMakeJamSandwitch = false;
                }
                else if(Input.GetKeyDown(KeyCode.Mouse1))
                {
                    selectedNode.SetAsJammed();
                }
            }
            else if((selectedNode.IsJammed || selectedNode.IsPeanutButter) && !selectedNode.IsShielded)
            {
                if(Input.GetKeyUp(KeyCode.Mouse0))
                {
                    selectedNode.SetShieldStatus(true);
                }
            }
            if(Input.GetKeyDown(KeyCode.Mouse2))
            {
                if(!selectedNode.IsJamSandwich)
                {
                    newWay = Instantiate(wayPref, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
                    newWay.SetPosition(0, selectedNode.transform.position);
                    newWay.SetPosition(1, (Vector2)Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition));
                    nodeHintPanel.HideAllHints();
                }
            }
        }

        if(newWay != null)
        {
            newWay.SetPosition(1, (Vector2)Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition));
            if(!Input.GetKey(KeyCode.Mouse2))
            {
                StopDrawingNewWay();
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
        if (node == null)
            throw new ArgumentNullException();
        if (node == selectedNode)
            throw new ArgumentException();

        int sIndex = selectedNode.NodeIndex;
        int nIndex = node.NodeIndex;
        if (!mainGraph.GetNeighbors(sIndex).Contains(nIndex) && !mainGraph.GetNeighbors(nIndex).Contains(sIndex))
        {
            mainGraph.AddEdge(sIndex, nIndex);
            DrawGraph();
            CurrentJamNumberDecrease();
        }
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

        Graph existsWays = new Graph(mainGraph.Vertices);

        for(int i = 0; i < mainGraph.Vertices; i++)
        {
            for (int j = 0; j < mainGraph.GetNeighbors(i).Count; j++)
            {
                if (!existsWays.GetNeighbors(mainGraph.GetNeighbors(i)[j]).Contains(i))
                {
                    LineRenderer line = Instantiate(wayPref, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
                    line.SetPosition(0, GetNode(i).transform.position);
                    line.SetPosition(1, GetNode(mainGraph.GetNeighbors(i)[j]).transform.position);
                    existsWays.AddEdge(i, mainGraph.GetNeighbors(i)[j]);
                }
            }
        }
    }
}
