using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    string levelName;
    public Button ButtonComponent { get { return GetComponent<Button>(); } }

    public void Init(bool levelIsLoadable)
    {
        ButtonComponent.interactable = levelIsLoadable;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
