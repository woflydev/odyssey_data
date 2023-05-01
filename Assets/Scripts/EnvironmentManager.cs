using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Light), typeof(Renderer))]
public class EnvironmentManager : MonoBehaviour
{
    [Header("Lighting Configuration")]
    [Range(0.00f, 5.00f)] 
    public float maxBrightness;
    
    [Range(0.00f, 5.00f)] 
    public float minBrightness;

    public float changeDelay;
    public bool changeLighting;

    [Header("Texture Configuration")] 
    [Range(-40.00f, 40.00f)]
    public float scrollSpeed;

    [Header("Dependencies")]
    public Light mainLight;
    public Renderer floorRenderer;


    private void Start()
    {
        StartCoroutine(ChangeLighting());
    }

    private void Update()
    {
        floorRenderer.material.mainTextureOffset = new Vector2(floorRenderer.material.mainTextureOffset.x - scrollSpeed * Time.deltaTime, floorRenderer.material.mainTextureOffset.y);
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
