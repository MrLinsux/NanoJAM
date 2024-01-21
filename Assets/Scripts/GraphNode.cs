using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum NodeStates { None, Toaster, PeanutToaster, PeanutButter, JamSandwich, Jammed, Bread }
    [SerializeField]
    Sprite[] stateSprites;
    public int NodeIndex { get { return _map.GetNodeIndex(this); } }

    [SerializeField]
    private NodeStates state;
    public NodeStates State {  get { return state; } }
    public bool IsToaster { get { return State == NodeStates.Toaster; } }
    public bool IsPeanutToaster { get { return State == NodeStates.PeanutToaster; } }
    public bool IsPeanutButter { get { return State == NodeStates.PeanutButter; } }
    public bool IsJamSandwich { get { return State == NodeStates.JamSandwich; } }
    public bool IsJammed { get { return State == NodeStates.Jammed; } }
    public bool IsShielded { get { return isShielded; } }
    bool isShielded = false;
    [SerializeField]
    GameObject shieldSprite;
    public bool IsBread { get { return State == NodeStates.Bread; } }
    SpriteRenderer _sprite;
    SpriteRenderer _outline;
    NodesMap _map;

    public void Init()
    {
        _map = transform.parent.GetComponent<NodesMap>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.sprite = stateSprites[(int)state];
        _outline = GameObject.Find("Outline").GetComponent<SpriteRenderer>();
    }

    public void SetAsJamSandwich()
    {
        _map.FreeNode(_map.GetNodeIndex(_map.SelectedNode));
        state = NodeStates.JamSandwich;
        _sprite.sprite = stateSprites[(int)state];
        SetActiveOutline(false);
        _map.NodeHintPanel.HideAllHints();
        _map.MaxJamNumberIncrease();
        _map.CurrentJamNumberDecrease();
    }
    public void SetAsJammed()
    {
        state = NodeStates.Jammed;
        _sprite.sprite = stateSprites[(int)state];
        _map.CurrentJamNumberDecrease();
        _map.NodeHintPanel.SetLeftHint(shieldSprite.GetComponent<SpriteRenderer>().sprite);
        _map.NodeHintPanel.HideRightHint();
    }

    public void SetShieldStatus(bool isOn)
    {
        if(isOn)
        {
            isShielded = true;
            _map.CurrentJamNumberDecrease();
        }
        else
        {
            isShielded = false;
            _sprite.sprite = stateSprites[(int)state];
        }
        shieldSprite.SetActive(isOn);
    }

    public void SetAsButter()
    {
        if (IsToaster)
        {
            state = NodeStates.PeanutToaster;
            _sprite.sprite = stateSprites[(int)state];
            _map.Controller.GameOver();
        }
        else if(!IsPeanutToaster)
        {
            state = NodeStates.PeanutButter;
            _sprite.sprite = stateSprites[(int)state];
        }
        SetActiveOutline(false);
        _map.NodeHintPanel.HideAllHints();
    }

    public void SetActiveOutline(bool isActive)
    {
        var color = _outline.color;
        if (isActive)
        {
            color.a = 0.6f;
            _outline.color = color;
            _outline.transform.position = transform.position;
            _map.SelectedNode = this;
        }
        else
        {
            color.a = 0f;
            _outline.color = color;
            _map.SelectedNode = null;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_map.NewWay == null)
        {
            if (IsToaster || IsJammed || _map.MainGraph.GetNeighbors(NodeIndex).Any(e => _map.GetNode(e).IsJammed || _map.GetNode(e).IsToaster))
            {
                SetActiveOutline(true);
                if (!IsShielded)
                    switch (State)
                    {
                        case NodeStates.Toaster:
                            break;
                        case NodeStates.Bread:
                            _map.NodeHintPanel.SetLeftHint(stateSprites[(int)NodeStates.JamSandwich]);
                            _map.NodeHintPanel.SetRightHint(stateSprites[(int)NodeStates.Jammed]);
                            break;
                        case NodeStates.PeanutButter:
                            _map.NodeHintPanel.SetLeftHint(shieldSprite.GetComponent<SpriteRenderer>().sprite);
                            break;
                        case NodeStates.Jammed:
                            _map.NodeHintPanel.SetLeftHint(shieldSprite.GetComponent<SpriteRenderer>().sprite);
                            break;
                        case NodeStates.JamSandwich:
                            break;
                    }
            }
        }
        else
        {
            if (!IsJamSandwich)
            {
                _map.SecondSelectNode = this;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_map.NewWay == null)
        {
            if (IsToaster || IsJammed || _map.MainGraph.GetNeighbors(NodeIndex).Any(e => _map.GetNode(e).IsJammed || _map.GetNode(e).IsToaster))
            {
                _map.NodeHintPanel.HideAllHints();
                SetActiveOutline(false);
            }
        }
        else
        {
            _map.SecondSelectNode = null;
        }
    }
}
