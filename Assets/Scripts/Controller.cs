using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        var levels = LoadGame().levels;
        if (levels.First(l => l.name == LevelName)!=null)
            levels.First(l => l.name == LevelName).isComplete = true;
        else
            throw new MissingComponentException("There is no level.");

        SaveGame(new JSONSerializable.Levels(levels));

        levelCompletePanel.SetActive(true);
    }

    void SaveGame(JSONSerializable.Levels data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(JSONSerializable.saveFilePath);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    JSONSerializable.Levels LoadGame()
    {
        if (File.Exists(JSONSerializable.saveFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
              File.Open(JSONSerializable.saveFilePath, FileMode.Open);
            JSONSerializable.Levels data = (JSONSerializable.Levels)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data loaded!");
            return data;
        }
        else
        {
            Debug.LogError("There is no save data!");
            return null;
        }
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
