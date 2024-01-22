using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    NodesMap map;
    [SerializeField]
    bool isGameOver = false;
    public bool IsGameOver { get { return isGameOver; } }

    private void Awake()
    {
        map.Init();
    }

    public void GameOver()
    {
        Debug.Log("You lose!");
        isGameOver = true;
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
