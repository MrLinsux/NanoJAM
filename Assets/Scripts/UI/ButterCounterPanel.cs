using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButterCounterPanel : MonoBehaviour
{
    [SerializeField]
    NodesMap _nodesMap;
    Animator _anim;

    TMP_Text _text;

    private void Awake()
    {
        _nodesMap = GameObject.Find("NodesMap").GetComponent<NodesMap>();
        _text = GetComponentInChildren<TMP_Text>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_nodesMap.StepsToLose != _nodesMap.MaxStepsToLose)
        {
            _anim.SetBool("IsShriking", false);
            _text.text = _nodesMap.StepsToLose + "/" + _nodesMap.MaxStepsToLose;
        }
        else
        {
            _anim.SetBool("IsShriking", true);
        }
    }
}
