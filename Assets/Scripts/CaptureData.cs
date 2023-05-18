using System;
using System.Collections;
using PathCreation.Examples;
using UnityEngine;
using UnityEngine.UI;

public class CaptureData : MonoBehaviour
{
    [Header("Configuration")]
    [Range(0.01f, 30f)]
    public float logFrequency;
    public KeyCode logKey = KeyCode.Backspace;
    public bool logOnAwake = true;
    public bool spawnRandomObjects = true;

    [Header("Dependencies")] 
    public RunNumber runNum;
    public RoadMeshCreator meshCreator;
    public PathPlacer pathPlacer;
    public EnvironmentManager envManager;
    public GameObject floor;
    public GameObject maskFloor;
    
    [Header("Debugging")]
    public bool isRunning;

    private Coroutine logDataRoutine = null;

    public int currentFrame;
    
    private void Start()
    {
        StartCoroutine(InitializeLogger());

        if (logOnAwake)
        {
            isRunning = true;
            logDataRoutine = StartCoroutine(LogData());
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(logKey))
        {
            if (isRunning)
            {
                isRunning = false;
                
                StopCoroutine(logDataRoutine);
                Debug.Log("Data logging stopped.");
                
                // resets to normal view
                ToggleMaskMode(false);
            }

            else
            {
                isRunning = true;
                
                logDataRoutine = StartCoroutine(LogData());
                Debug.Log("Data logging started!");
                
                // see above
                ToggleMaskMode(false);
            }
        }
    }
    
    private void ToggleMaskMode(bool isMask)
    {
        // the logic is really weird here, if we pass in 'false' (i.e. no mask mode), we have to reverse it in order to turn ON the meshHolder and turn OFF the maskHolder.
        meshCreator.meshHolder.SetActive(!isMask);
        meshCreator.maskHolder.SetActive(isMask);

        // because we can't access inspector scripts' variables at runtime, we must resort to toggling with parent objects.
        pathPlacer.objectHolder.SetActive(!isMask);
        pathPlacer.maskedObjectHolder.SetActive(isMask);
        
        pathPlacer.randomObjHolder.SetActive(!isMask);
        
        // this also applies for floor
        floor.SetActive(!isMask);
        maskFloor.SetActive(isMask);
    }
    
    private IEnumerator InitializeLogger()
    {
        ToggleMaskMode(true);
        Debug.Log("Initializing...");
        yield return new WaitForSeconds(2);
        ToggleMaskMode(false);
        yield return new WaitForSeconds(2);
        Debug.Log("Initialization complete!");
    }
    
    private IEnumerator LogData()
    {
        runNum.runNumber++;
        currentFrame = 0;
        
        // local var that restarts every time async job is reset
        //int currentFrame = 1;
        
        while (true)
        {
            ScreenCapture.CaptureScreenshot("Assets/Data/run" + runNum.runNumber + "_" + currentFrame + ".png", 1);
            
            // WHY.
            // JUST WHY.
            yield return this;
            
            meshCreator.meshHolder.SetActive(false);
            meshCreator.maskHolder.SetActive(true);
            pathPlacer.objectHolder.SetActive(false);
            pathPlacer.maskedObjectHolder.SetActive(false);
            pathPlacer.randomObjHolder.SetActive(false);
            floor.SetActive(false);
            maskFloor.SetActive(true);

            // can't use this anymore as mr. funny web man wants us to do separate for lane and obstacle
            //ToggleMaskMode(true);

            ScreenCapture.CaptureScreenshot("Assets/Data/run" + runNum.runNumber + "_" + currentFrame + "_lane" + ".png", 1);
            yield return this;

            meshCreator.meshHolder.SetActive(false);
            meshCreator.maskHolder.SetActive(false);
            pathPlacer.objectHolder.SetActive(false);
            pathPlacer.maskedObjectHolder.SetActive(true);
            pathPlacer.randomObjHolder.SetActive(false);
            floor.SetActive(false);
            maskFloor.SetActive(true);
            
            //ToggleMaskMode(false);
            
            ScreenCapture.CaptureScreenshot("Assets/Data/run" + runNum.runNumber + "_" + currentFrame + "_obstacle" + ".png", 1);
            yield return this;

            meshCreator.meshHolder.SetActive(true);
            meshCreator.maskHolder.SetActive(false);
            pathPlacer.objectHolder.SetActive(true);
            pathPlacer.maskedObjectHolder.SetActive(false);
            pathPlacer.randomObjHolder.SetActive(true);
            floor.SetActive(true);
            maskFloor.SetActive(false);
            
            yield return new WaitForSeconds(logFrequency);
            
            currentFrame++;
        }
    }

    /*private IEnumerator test()
    {
        int currentFrame = 1;
        
        while (true)
        {
            meshCreator.AssignMeshComponents();
            meshCreator.AssignMaterials();

            //ScreenCapture.CaptureScreenshot("Assets/Data/frame-" + currentFrame + "-" + System.DateTime.Now.ToString("HH-mm-ss") + ".png", 1);
            //Debug.Log("first shot taken");
            
            yield return new WaitForSeconds(2);
            
            meshCreator.AssignMeshComponents();
            meshCreator.AssignMaterials();

            //ScreenCapture.CaptureScreenshot("Assets/Data/frame-" + currentFrame + "-" + System.DateTime.Now.ToString("HH-mm-ss") + "-mask" + ".png", 1);
            //Debug.Log("second shot taken");

            currentFrame++;
        }
    }*/

}
