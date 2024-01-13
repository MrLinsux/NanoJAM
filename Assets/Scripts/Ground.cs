using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ground : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    SpriteRenderer _outline;
    [SerializeField]
    Graph _graph;

    public void OnPointerDown(PointerEventData eventData)
    {
        var color = _outline.color;
        color.a = 0f;
        _outline.color = color;
        _graph.selectedNode = null;
    }
}
