using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public class LightingManager : MonoBehaviour
{
    public Light mainLight;

    [Range(0.00f, 5.00f)] 
    public float maxBrightness;
    
    [Range(0.00f, 5.00f)] 
    public float minBrightness;

    public float changeDelay;
    public bool changeLighting;

    private void Start()
    {
        StartCoroutine(ChangeLighting());
    }

    private IEnumerator ChangeLighting()
    {
        while (changeLighting)
        {
            mainLight.intensity = Random.Range(minBrightness, maxBrightness);
            yield return new WaitForSeconds(changeDelay);
        }
    }
}
