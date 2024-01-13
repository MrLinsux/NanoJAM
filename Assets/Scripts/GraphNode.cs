using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphNode : MonoBehaviour, IPointerDownHandler
{
    public enum NodeStates { Toaster, PeanutButter, Jam }
    [SerializeField]
    Color[] stateColors;

    [SerializeField]
    private NodeStates state;
    public NodeStates State {  get { return state; } }
    SpriteRenderer _sprite;
    SpriteRenderer _outline;
    Graph _graph;

    private void Start()
    {
        _graph = transform.parent.GetComponent<Graph>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.color = stateColors[(int)state];
        _outline = GameObject.Find("Outline").GetComponent<SpriteRenderer>();
    }

    public void SetAsJam()
    {
        _graph.FreeNode(_graph.GetNodeIndex(_graph.selectedNode));
        _sprite.color = stateColors[2];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var color = _outline.color;
        color.a = 0.6f;
        _outline.color = color;
        _outline.transform.position = transform.position;
        _graph.selectedNode = this;
    }
}
