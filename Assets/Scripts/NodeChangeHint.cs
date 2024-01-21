using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeChangeHint : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] leftHints;
    [SerializeField]
    private GameObject[] rightHints;
    
    public void HideLeftHint()
    {
        leftHints[0].SetActive(false);
        leftHints[1].SetActive(false);
    }

    public void HideRightHint()
    {
        rightHints[0].SetActive(false);
        rightHints[1].SetActive(false);
    }

    public void HideAllHints()
    {
        HideLeftHint();
        HideRightHint();
    }

    public void SetLeftHint(Sprite leftNode)
    {
        leftHints[0].SetActive(true);
        leftHints[0].GetComponent<Image>().sprite = leftNode;
        leftHints[1].SetActive(true);
    }

    public void SetRightHint(Sprite rightNode)
    {
        rightHints[0].SetActive(true);
        rightHints[0].GetComponent<Image>().sprite = rightNode;
        rightHints[1].SetActive(true);
    }

    void Update()
    {
        transform.position = (Vector2)Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition);
    }
}
