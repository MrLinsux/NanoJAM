using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [SerializeField]
    NodesMap map;
    [SerializeField]
    bool isGameOver = false;
    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    GameObject levelCompletePanel;
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
        gameOverPanel.SetActive(true);
    }

    public void LevelIsComplete()
    {
        var levels = JsonUtility.FromJson<JSONSerializable.Levels>(Resources.Load<TextAsset>(JSONSerializable.levelsJSONFileName).text).levels;
        if (levels.First(l => l.name == LevelName)!=null)
            levels.First(l => l.name == LevelName).isComplete = true;
        else
            throw new MissingComponentException("There is no level.");

        var saveData = JsonUtility.ToJson(new JSONSerializable.Levels(levels));

        string path = null;
#if UNITY_STANDALONE
        // You cannot add a subfolder, at least it does not work for me
        path = $"NanoJAM_Data/Resources/{JSONSerializable.levelsJSONFileName}.json";
    #endif
#if UNITY_EDITOR
        path = $"Assets/Resources/{JSONSerializable.levelsJSONFileName}.json";
#endif

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(saveData);
            }
        }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        levelCompletePanel.SetActive(true);
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
