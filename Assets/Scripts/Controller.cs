using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    NodesMap map;

    private void Awake()
    {
        map.Init();
    }

    public void GameOver()
    {
        Debug.Log("You lose!");
    }
}
