using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphNode : MonoBehaviour, IPointerDownHandler
{
    public enum NodeStates { Toaster, PeanutButter, Jammed, Shielded }
    [SerializeField]
    Color[] stateColors;

    [SerializeField]
    private NodeStates state;
    public NodeStates State {  get { return state; } }
    SpriteRenderer _sprite;
    SpriteRenderer _outline;
    Graph _graph;

    public void Init()
    {
        _graph = transform.parent.GetComponent<Graph>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = stateColors[(int)state];
        _outline = GameObject.Find("Outline").GetComponent<SpriteRenderer>();
    }

    public void SetAsJam()
    {
        _graph.FreeNode(_graph.GetNodeIndex(_graph.SelectedNode));
        state = NodeStates.Jammed;
        _sprite.color = stateColors[2];
        _graph.JamNumberIncrease();
    }

    public void SetAsShield()
    {
        state = NodeStates.Shielded;
        _sprite.color = stateColors[3];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_graph.WaitSecondNode)
        {
            _graph.JoinNodeToSelect(this);
        }
        else
        {
            var color = _outline.color;
            color.a = 0.6f;
            _outline.color = color;
            _outline.transform.position = transform.position;
            _graph.SelectedNode = this;
        }
    }
}
