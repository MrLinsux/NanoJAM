using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    [SerializeField]
    TMP_Text JammedTask;
    [SerializeField]
    TMP_Text JamShieldedTask;
    [SerializeField]
    TMP_Text ButterSkipStepTask;
    [SerializeField]
    TMP_Text AllButterIsShieldedTask;
    [SerializeField]
    TMP_Text MaxJamPointTask;
    [SerializeField]
    string JammedTaskText;
    [SerializeField]
    string JamShieldedTaskText;
    [SerializeField]
    string ButterSkipStepTaskText;
    [SerializeField]
    string AllButterIsShieldedTaskText;
    [SerializeField]
    string MaxJamPointTaskText;

    public void Init(int needJammedTask, int needJamShieldedTask, int needButterSkipStepTask, bool needAllButterIsShieldedTask, int needMaxJamPointTask)
    {
        JammedTask.transform.parent.gameObject.SetActive(needJammedTask != -1);
        JamShieldedTask.transform.parent.gameObject.SetActive(needJamShieldedTask != -1);
        ButterSkipStepTask.transform.parent.gameObject.SetActive(needButterSkipStepTask != -1);
        AllButterIsShieldedTask.transform.parent.gameObject.SetActive(!needAllButterIsShieldedTask);
        MaxJamPointTask.transform.parent.gameObject.SetActive(needMaxJamPointTask != -1);

        if (needJammedTask != -1)
            SetJammedTaskProgress(0, needJammedTask);
        if (needJamShieldedTask != -1)
            SetJamShieldedTaskProgress(0, needJamShieldedTask);
        if (needButterSkipStepTask != -1)
            SetButterSkipStepTaskProgress(0, needButterSkipStepTask);
        if (!needAllButterIsShieldedTask)
            SetAllButterIsShieldedTaskProgress(needAllButterIsShieldedTask);
        if (needMaxJamPointTask != -1)
            SetMaxJamPointTaskProgress(0, needMaxJamPointTask);
    }

    public void SetJammedTaskProgress(int current, int need)
    {
        JammedTask.text = JammedTaskText + current.ToString() + "/" + need.ToString();
    }
    public void SetJamShieldedTaskProgress(int current, int need)
    {
        JamShieldedTask.text = JamShieldedTaskText + current.ToString() + "/" + need.ToString();
    }
    public void SetButterSkipStepTaskProgress(int current, int need)
    {
        ButterSkipStepTask.text = ButterSkipStepTaskText + current.ToString() + "/" + need.ToString();
    }
    public void SetAllButterIsShieldedTaskProgress(bool isAll)
    {
        AllButterIsShieldedTask.text = AllButterIsShieldedTaskText;
    }
    public void SetMaxJamPointTaskProgress(int current, int need)
    {
        MaxJamPointTask.text = MaxJamPointTaskText + current.ToString() + "/" + need.ToString();
    }
}
