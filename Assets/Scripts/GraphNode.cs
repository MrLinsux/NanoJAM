using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphNode : MonoBehaviour, IPointerDownHandler
{
    public enum NodeStates { None, Toaster, PeanutButter, JamSandwich, Jammed, Shielded, Bread }
    [SerializeField]
    Color[] stateColors;
    public int NodeIndex { get { return _graph.GetNodeIndex(this); } }

    [SerializeField]
    private NodeStates state;
    public NodeStates State {  get { return state; } }
    public bool IsToaster { get { return State == NodeStates.Toaster; } }
    public bool IsPeanutButter { get { return State == NodeStates.PeanutButter; } }
    public bool IsJamSandwich { get { return State == NodeStates.JamSandwich; } }
    public bool IsJammed { get { return State == NodeStates.Jammed; } }
    public bool IsShielded { get { return State == NodeStates.Shielded; } }
    public bool IsBread { get { return State == NodeStates.Bread; } }
    SpriteRenderer _sprite;
    SpriteRenderer _outline;
    NodesMap _graph;

    public void Init()
    {
        _graph = transform.parent.GetComponent<NodesMap>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = stateColors[(int)state];
        _outline = GameObject.Find("Outline").GetComponent<SpriteRenderer>();
    }

    public void SetAsJamSandwich()
    {
        _graph.FreeNode(_graph.GetNodeIndex(_graph.SelectedNode));
        state = NodeStates.JamSandwich;
        _sprite.color = stateColors[(int)NodeStates.JamSandwich];
        _graph.CurrentJamNumberDecrease();
    }
    public void SetAsJammed()
    {
        state = NodeStates.Jammed;
        _sprite.color = stateColors[(int)NodeStates.Jammed];
        _graph.CurrentJamNumberDecrease();
    }

    public void SetAsShield()
    {
        state = NodeStates.Shielded;
        _sprite.color = stateColors[(int)NodeStates.Shielded];
        _graph.CurrentJamNumberDecrease();
    }
    public void SetAsButter()
    {
        if(IsJamSandwich)
            _graph.MaxJamNumberDecrease();
        state = NodeStates.PeanutButter;
        _sprite.color = stateColors[(int)NodeStates.PeanutButter];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_graph.WaitSecondNode && !IsJamSandwich)
        {
            _graph.JoinNodeToSelect(this);
        }
        else if(_graph.MainGraph.BreadthFirstSearch(0, NodeIndex))  // 0 - is toster
        {
            var color = _outline.color;
            color.a = 0.6f;
            _outline.color = color;
            _outline.transform.position = transform.position;
            _graph.SelectedNode = this;
        }
    }
}
