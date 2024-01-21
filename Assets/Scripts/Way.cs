using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Way : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField]
    float timeBetweenAnimationFrame = 1f;
    float time = 0f;
    int frame = 0;
    [SerializeField]
    Texture2D[] sprites;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(time >= timeBetweenAnimationFrame)
        {
            time = 0;
            frame++;
        }
        time += Time.deltaTime;
        //lineRenderer.material.SetInt("_Frame", frame%8);
        lineRenderer.material.SetTexture("_MainTex", sprites[frame % 8]);
    }
}
