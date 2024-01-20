using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphNode : MonoBehaviour, IPointerDownHandler
{
    public enum NodeStates { None, Toaster, PeanutButter, JamSandwich, Jammed, Shielded, Bread }
    [SerializeField]
    Color[] stateColors;
    public int NodeIndex { get { return _map.GetNodeIndex(this); } }

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
    NodesMap _map;

    public void Init()
    {
        _map = transform.parent.GetComponent<NodesMap>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = stateColors[(int)state];
        _outline = GameObject.Find("Outline").GetComponent<SpriteRenderer>();
    }

    public void SetAsJamSandwich()
    {
        _map.FreeNode(_map.GetNodeIndex(_map.SelectedNode));
        state = NodeStates.JamSandwich;
        _sprite.color = stateColors[(int)NodeStates.JamSandwich];
        _map.MaxJamNumberIncrease();
        _map.CurrentJamNumberDecrease();
    }
    public void SetAsJammed()
    {
        state = NodeStates.Jammed;
        _sprite.color = stateColors[(int)NodeStates.Jammed];
        _map.CurrentJamNumberDecrease();
    }

    public void SetAsShield()
    {
        state = NodeStates.Shielded;
        _sprite.color = stateColors[(int)NodeStates.Shielded];
        _map.CurrentJamNumberDecrease();
    }
    public void SetAsButter()
    {
        if (IsToaster)
        {
            state = NodeStates.PeanutButter;
            _sprite.color = stateColors[(int)NodeStates.PeanutButter];
            _map.Controller.GameOver();
        }
        else
        {
            state = NodeStates.PeanutButter;
            _sprite.color = stateColors[(int)NodeStates.PeanutButter];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_map.WaitSecondNode && !IsJamSandwich)
        {
            _map.JoinNodeToSelect(this);
        }
        else if(IsToaster || IsJammed || _map.MainGraph.GetNeighbors(NodeIndex).Any(e=> _map.GetNode(e).IsJammed || _map.GetNode(e).IsToaster))
        {
            var color = _outline.color;
            color.a = 0.6f;
            _outline.color = color;
            _outline.transform.position = transform.position;
            _map.SelectedNode = this;
        }
    }
}
