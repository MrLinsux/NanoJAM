using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JamCounterPanel : MonoBehaviour
{
    [SerializeField]
    NodesMap _nodesMap;

    TMP_Text _text;

    private void Awake()
    {
        _nodesMap = GameObject.Find("NodesMap").GetComponent<NodesMap>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        _text.text = _nodesMap.CurrentJamNumber + "/" + _nodesMap.MaxJamPoints;
    }
}
