using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GraphNode;

public class NodesMap : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField, Tooltip("level 2")]
    bool canMakeShield = true;
    public bool CanMakeShield { get { return canMakeShield; } }
    [SerializeField, Tooltip("level 3")]
    bool canMakeJamSandwitch = true;
    public bool CanMakeJamSandwitch { get { return canMakeJamSandwitch && !MaxJamNumberIsChanged; } }
    [SerializeField, Tooltip("level 4")]
    bool canMakeConnections = true;
    public bool CanMakeConnections { get { return canMakeConnections; } }
    [SerializeField, Tooltip("level 5")]
    bool butterCanChangeNodes = true;
    public bool ButterCanChangeNodes { get { return butterCanChangeNodes; } }

    [SerializeField]
    bool canMakeTotalShield = true;
    public bool CanMakeTotalShield { get { return canMakeTotalShield && !MaxJamNumberIsChanged && MaxJamPoints > 1; } }

    [SerializeField]
    bool butterCanShielded = true;
    public bool ButterCanShielded { get { return butterCanShielded; } }



    [Header("Level Tasks")]
    [SerializeField, Tooltip("task 1")]
    int NeedJammed = -1;
    [SerializeField, Tooltip("task 2")]
    int NeedJamShieled = -1;
    [SerializeField, Tooltip("task 3")]
    int NeedButterSkipSteps = -1;
    [SerializeField, Tooltip("task 4")]
    bool AllButterIsShielded = false;
    [SerializeField, Tooltip("task 5")]
    int NeedMaxJamPoint = -1;

    int JammedNumber { get { return nodes.Count(e => e.IsJammed); } }
    int livingSteps = 0;
    public void LivingStepsIncrease() => livingSteps++;
    int peanutButterSkipSteps = 0;
    public void PeanutButterSkipStepsIncrease()
    {
        peanutButterSkipSteps++;
        _taskPanel.SetButterSkipStepTaskProgress(peanutButterSkipSteps, NeedButterSkipSteps);
    }
    int peanutButterShieldSets = 0;
    public void PeanutButterShieldSetsIncrease() => peanutButterShieldSets++;

    public bool EnoughJammed { get { return NeedJammed <= JammedNumber || NeedJammed == -1; } }
    public bool EnoughJamShielded { get { return NeedJamShieled <= nodes.Count(n => n.IsJammed && n.IsShielded) || NeedJamShieled == -1; } }
    public bool EnoughButterSkipSteps { get { return NeedButterSkipSteps <= peanutButterSkipSteps || NeedButterSkipSteps == -1; } }
    public bool EnoughButterShielded { get { return !AllButterIsShielded || nodes.Count(n => n.IsPeanutButter) == nodes.Count(n => n.IsPeanutButter && n.IsShielded); } }
    public bool EnoughMaxJamPoint { get { return MaxJamPoints >= NeedMaxJamPoint || NeedMaxJamPoint == -1; } }

    public bool IsWin
    {
        get
        {
            return EnoughJammed && EnoughJamShielded && EnoughButterSkipSteps && EnoughButterShielded && EnoughMaxJamPoint;
        }
    }



    [Header("In game variables")]
    [SerializeField]
    int maxStepsToLose = 3;
    [SerializeField]
    int stepsToLose = 3;
    public int MaxStepsToLose { get { return maxStepsToLose; } }
    public int StepsToLose { get { return stepsToLose; } }
    public void StepsToLoseDecrease()
    {
        stepsToLose--;
        if (stepsToLose <= 0)
        {
            _controller.GameOver();
        }
    }

    public void ResetLoseProgression() => stepsToLose = maxStepsToLose;

    [SerializeField]
    int maxJamNumber = 0;
    public int MaxJamPoints { get { return maxJamNumber; } }
    public void MaxJamNumberIncrease()
    {
        maxJamNumber++;
        maxJamNumberIsChanged = true;
        _taskPanel.SetMaxJamPointTaskProgress(MaxJamPoints, NeedMaxJamPoint);
    }
    public void MaxJamNumberDecrease()
    {
        maxJamNumber--;
        maxJamNumberIsChanged = true;
        _taskPanel.SetMaxJamPointTaskProgress(MaxJamPoints, NeedMaxJamPoint);
    }
    bool maxJamNumberIsChanged = false;
    public bool MaxJamNumberIsChanged { get { return maxJamNumberIsChanged; } }

    [SerializeField]
    int currentJamNumber = 0;
    public int CurrentJamNumber { get { return currentJamNumber; } private set { currentJamNumber = value; } }
    public void CurrentJamNumberIncrease() => CurrentJamNumber++;
    public void CurrentJamPointDecrease()
    {
        CurrentJamNumber--;
        if (currentJamNumber <= 0 && !isNextTurn) NextTurn();
    }



    [Header("Outer Objects")]
    [SerializeField]
    Wave wave;

    Graph mainGraph;
    [SerializeField]
    AudioClip skipTurnSound;
    public Graph MainGraph { get { return mainGraph; } }
    [SerializeField]
    Controller _controller;
    [SerializeField]
    TaskPanel _taskPanel;
    public Controller Controller { get { return _controller; } }

    GraphNode[] nodes;
    public GraphNode[] Nodes
    {
        get { return nodes; }
    }
    [SerializeField]
    GameObject wayPref;
    [SerializeField]
    NodeChangeHint _nodeHintPanel;
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
    LineRenderer newWay;

    public LineRenderer NewWay { get { return newWay; } }
    public void StopDrawingNewWay()
    {
        Destroy(newWay.gameObject); newWay = null;
        if (secondSelectedNode != null && secondSelectedNode != selectedNode)
        {
            JoinNodeToSelect(secondSelectedNode);
            if (selectedNode != null)
                selectedNode.SetActiveOutline(false);
            if(secondSelectedNode != null)
                secondSelectedNode.OnPointerEnter(null);
            secondSelectedNode = null;
        }
        else
        {
            selectedNode.SetActiveOutline(false);
        }
    }
    public NodeChangeHint NodeHintPanel { get { return _nodeHintPanel; } }

    public GraphNode SelectedNode { get { return selectedNode; } set { selectedNode = value; } }

    void SetActiveTotalShield(bool isActive)
    {
        butterCanChangeNodes = !isActive;
        if (isActive)
        {
            StartCoroutine(wave.ZaWarudo());
            _controller.PlaySound();
            MaxJamNumberDecrease();
            CurrentJamPointDecrease();
        }
    }

    IEnumerator WaitZaWarudo()
    {
        while(wave.IsZaWarudo)
        {
            yield return new WaitForEndOfFrame();
        }
        NextTurn();
    }

    bool isNextTurn = false;

    public void NextTurn()
    {
        isNextTurn = true;
        if(wave.IsZaWarudo)
        {
            StartCoroutine(WaitZaWarudo());
            return;
        }

        if(IsWin)
        {
            _controller.LevelIsComplete();
            return;
        }

        if(butterCanChangeNodes)
            ButterTurn();
        else
            PeanutButterSkipStepsIncrease();

        CurrentJamNumber = MaxJamPoints;
        maxJamNumberIsChanged = false;
        canMakeJamSandwitch = true;
        if (!_controller.IsGameOver)
            LivingStepsIncrease();
        SetActiveTotalShield(false);
        Debug.Log("Next Turn");
        isNextTurn = false;
    }

    void ButterTurn()
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
        else
        {
            ResetLoseProgression();
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
        _taskPanel = GameObject.Find("TaskPanel").GetComponent<TaskPanel>();
        _taskPanel.Init(NeedJammed, NeedJamShieled, NeedButterSkipSteps, !AllButterIsShielded, NeedMaxJamPoint);
        _nodeHintPanel = GameObject.Find("MouseHintPanel").GetComponent<NodeChangeHint>();
    }

    private void Update()
    {
        if (selectedNode != null)
        {
            if(selectedNode.IsBread)
            {
                if(Input.GetKeyDown(KeyCode.Mouse0) && CanMakeJamSandwitch)
                {
                    canMakeJamSandwitch = false;
                    selectedNode.SetAsJamSandwich();
                }
                else if(Input.GetKeyDown(KeyCode.Mouse1))
                {
                    selectedNode.SetAsJammed();
                    _taskPanel.SetJammedTaskProgress(JammedNumber, NeedJammed);
                }
            }
            else if(selectedNode.IsJammed && !selectedNode.IsShielded)
            {
                if(Input.GetKeyUp(KeyCode.Mouse0) && canMakeShield)
                {
                    selectedNode.SetShieldStatus(true);
                    _taskPanel.SetJamShieldedTaskProgress(nodes.Count(n => n.IsJammed && n.IsShielded), NeedJamShieled);
                }
            }
            else if(selectedNode.IsPeanutButter && ButterCanShielded)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0) && canMakeShield)
                {
                    selectedNode.SetShieldStatus(true);
                    _taskPanel.SetAllButterIsShieldedTaskProgress(EnoughButterShielded);
                }
            }
            else if(selectedNode.IsToaster)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) && CanMakeTotalShield)
                {
                    SetActiveTotalShield(true);
                    _nodeHintPanel.HideLeftHint();
                }
                else if(Input.GetKeyDown(KeyCode.Mouse1) && !isNextTurn)
                {
                    NextTurn();
                    _controller.GetComponent<AudioSource>().PlayOneShot(skipTurnSound);
                }
            }
            if(Input.GetKeyDown(KeyCode.Mouse2))
            {
                if(!selectedNode.IsJamSandwich && canMakeConnections)
                {
                    newWay = Instantiate(wayPref, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>();
                    newWay.SetPosition(0, selectedNode.transform.position);
                    newWay.SetPosition(1, (Vector2)Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition));
                    _nodeHintPanel.HideAllHints();
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


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Number of jammed nodes: " + JammedNumber);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Number of living steps: " + livingSteps);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Butter skip " + peanutButterSkipSteps + " steps");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Has been " + peanutButterShieldSets + " shields on butter");
        }
#endif
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
            CurrentJamPointDecrease();
        }

        node.PlayConnectionSound();
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
