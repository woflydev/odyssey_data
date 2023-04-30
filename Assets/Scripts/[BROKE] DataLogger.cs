using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation.Examples;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    [Range(0.01f, 30f)]
    public float logFrequency;
    public bool logOnAwake = true;

    [Header("Road Mesh Creator Script")]
    public RoadMeshCreator meshCreator;
    
    private bool isRunning;
    private int currentFrame = 1;
    
    private void Start()
    {
        if (logOnAwake)
        {
            StartCoroutine(LogData());
            isRunning = true;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            if (isRunning)
            {
                StopCoroutine(LogData());
                isRunning = false;
                Debug.Log("Datalogger coroutine stopped.");
            }

            else
            {
                StartCoroutine(LogData());
                isRunning = true;
                Debug.Log("Datalogger coroutine started.");
            }
        }
    }

    private IEnumerator LogData()
    {
        currentFrame = 1;
        
        while (enabled)
        {
            yield return new WaitForSeconds(1);

            //ScreenCapture.CaptureScreenshot("Assets/Data/frame-" + currentFrame + "-" + System.DateTime.Now.ToString("HH-mm-ss") + ".png", 1);
            
            /*if (captureMaskMode)
            {
                meshCreator.maskMode = true;
                ScreenCapture.CaptureScreenshot("Assets/Data/frame-" + currentFrame + "-" + System.DateTime.Now.ToString("HH-mm-ss") + "-mask" + ".png", 1);
            }*/

            currentFrame++;
            
            yield return new WaitForSeconds(logFrequency);
        }
    }
}
