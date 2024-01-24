using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    LevelButton[] buttons;

    void Start()
    {
        if(LoadGame() == null)
        {
            NewGame();
        }
        var levels = LoadGame().levels;
        levels = levels.Where(l => l.isComplete).ToArray();
        buttons = GetComponentsInChildren<LevelButton>(true);

        for(int i = 0; i < levels.Length; i++)
        {
            buttons[i].Init(true);
        }

        if(levels.Length < buttons.Length)
        {
            buttons[levels.Length].Init(true);
            for (int i = levels.Length+1; i < buttons.Length; i++)
            {
                buttons[i].Init(false);
            }
        }
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
            Debug.LogWarning("There is no save data!");
            return null;
        }
    }

    void NewGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(JSONSerializable.saveFilePath);
        var levels = JsonUtility.FromJson<JSONSerializable.Levels>(Resources.Load<TextAsset>(JSONSerializable.levelsJSONFileName).text).levels;
        bf.Serialize(file, new JSONSerializable.Levels(levels));
        file.Close();
        Debug.Log("Game data saved!");
    }
}
