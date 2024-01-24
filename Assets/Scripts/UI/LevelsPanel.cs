using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    LevelButton[] buttons;

    void Start()
    {
        var levels = JsonUtility.FromJson<JSONSerializable.Levels>(Resources.Load<TextAsset>(JSONSerializable.levelsJSONFileName).text);
        buttons = GetComponentsInChildren<LevelButton>();
    }
}
