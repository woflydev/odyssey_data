using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    [Range(0.01f, 30f)]
    public float logFrequency;

    private bool isRunning;
    
    private void Start()
    {
        StartCoroutine(LogData());
        isRunning = true;
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
        while (enabled)
        {
            ScreenCapture.CaptureScreenshot("Assets/Data/data-" + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") + ".png", 1);
            yield return new WaitForSeconds(logFrequency);
        }
    }
}
