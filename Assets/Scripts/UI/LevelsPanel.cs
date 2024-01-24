using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelsPanel : MonoBehaviour
{
    LevelButton[] buttons;

    void Start()
    {
        var levels = JsonUtility.FromJson<JSONSerializable.Levels>(Resources.Load<TextAsset>(JSONSerializable.levelsJSONFileName).text).levels.Where(l =>l.isComplete);
        buttons = GetComponentsInChildren<LevelButton>(true);

        for(int i = 0; i < levels.Count(); i++)
        {
            buttons[i].Init(true);
        }

        if(levels.Count() < buttons.Length)
        {
            buttons[levels.Count()].Init(true);
            for (int i = levels.Count()+1; i < buttons.Length; i++)
            {
                buttons[i].Init(false);
            }
        }
    }
}
