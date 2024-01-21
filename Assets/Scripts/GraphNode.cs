using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
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
    [SerializeField]
    UnityEngine.Color outlineColor;
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
    NodesMap _map;

    public void Init()
    {
        _map = transform.parent.GetComponent<NodesMap>();
        _sprite = GetComponent<SpriteRenderer>();
        _sprite.sprite = stateSprites[(int)state];
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
        if (_map.CanMakeShield)
        {
            _map.NodeHintPanel.SetLeftHint(shieldSprite.GetComponent<SpriteRenderer>().sprite);
        }
        else
        {
            _map.NodeHintPanel.HideLeftHint();
        }
        if (_map.CanMakeConnections)
            _map.NodeHintPanel.SetUpHint();
        _map.NodeHintPanel.HideRightHint();
    }

    public void SetShieldStatus(bool isOn)
    {
        if(isOn)
        {
            isShielded = true;
            _map.CurrentJamNumberDecrease();
            if(IsPeanutButter)
                _map.PeanutButterShieldSetsIncrease();
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

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _sprite.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", outlineColor);
        _sprite.SetPropertyBlock(mpb);
    }

    public void SetActiveOutline(bool isActive)
    {
        UpdateOutline(isActive);
        _map.SelectedNode = isActive ? this : null;
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
                            if (_map.CanMakeConnections)
                                _map.NodeHintPanel.SetUpHint();
                            break;
                        case NodeStates.Bread:
                            if (_map.CanMakeJamSandwitch)
                            {
                                _map.NodeHintPanel.SetLeftHint(stateSprites[(int)NodeStates.JamSandwich]);
                            }
                            else
                            {
                                _map.NodeHintPanel.HideLeftHint();
                            }
                            if(_map.CanMakeConnections)
                                _map.NodeHintPanel.SetUpHint();
                            _map.NodeHintPanel.SetRightHint(stateSprites[(int)NodeStates.Jammed]);
                            break;
                        case NodeStates.PeanutButter:
                            if (_map.CanMakeShield)
                            {
                                _map.NodeHintPanel.SetLeftHint(shieldSprite.GetComponent<SpriteRenderer>().sprite);
                            }
                            else
                            {
                                _map.NodeHintPanel.HideLeftHint();
                            }
                            if (_map.CanMakeConnections)
                                _map.NodeHintPanel.SetUpHint();
                            break;
                        case NodeStates.Jammed:
                            if (_map.CanMakeShield)
                            {
                                _map.NodeHintPanel.SetLeftHint(shieldSprite.GetComponent<SpriteRenderer>().sprite);
                            }
                            else
                            {
                                _map.NodeHintPanel.HideLeftHint();
                            }
                            if (_map.CanMakeConnections)
                                _map.NodeHintPanel.SetUpHint();
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
