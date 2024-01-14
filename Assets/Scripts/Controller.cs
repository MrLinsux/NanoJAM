using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    Graph graph;

    private void Awake()
    {
        graph.Init();
    }
}
