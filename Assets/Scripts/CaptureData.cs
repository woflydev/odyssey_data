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
    
    [Header("Dependencies")]
    public RoadMeshCreator meshCreator;
    public GameObject floor;
    public GameObject maskFloor;
    
    [Header("Debugging")]
    public bool isRunning;

    private Coroutine logDataRoutine = null;
    
    private void Start()
    {
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
        
        // this also applies for floor
        floor.SetActive(!isMask);
        maskFloor.SetActive(isMask);
    }

    private IEnumerator LogData()
    {
        // local var that restarts every time async job is reset
        int currentFrame = 1;
        
        while (true)
        {
            ScreenCapture.CaptureScreenshot("Assets/Data/frame-" + currentFrame + "-" + System.DateTime.Now.ToString("HH-mm-ss") + ".png", 1);
            
            // WHY.
            // JUST WHY.
            yield return this;
            
            ToggleMaskMode(true);
            
            ScreenCapture.CaptureScreenshot("Assets/Data/frame-" + currentFrame + "-" + System.DateTime.Now.ToString("HH-mm-ss") + "-mask" + ".png", 1);
            yield return this;

            currentFrame++;
            
            ToggleMaskMode(false);
            
            yield return new WaitForSeconds(logFrequency);
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
