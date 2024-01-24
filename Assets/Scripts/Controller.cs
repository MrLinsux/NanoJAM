using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [SerializeField]
    NodesMap map;
    [SerializeField]
    bool isGameOver = false;
    string LevelName { get { return SceneManager.GetActiveScene().name; } }
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

    public void LevelIsComlete()
    {
        JSONSerializable.Levels levels = JsonUtility.FromJson<JSONSerializable.Levels>(JSONSerializable.levelsJSONFileName);
        if(levels.levels.First(l => l.name == LevelName)!=null)
            levels.levels.First(l => l.name == LevelName).isComplete = true;
        else
            throw new MissingComponentException("There is no level.");
        JsonUtility.ToJson(levels);
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
