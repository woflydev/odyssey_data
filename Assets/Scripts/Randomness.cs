using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Randomness : MonoBehaviour
{
    [Header("Configuration")] 
    public float speed = 5f;
    public float delay = 3f;
    public Rigidbody rb;
    
    private Coroutine co;
    
    public void Start()
    {
        StartCoroutine(MoveRandomObjects());
        //rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        co = StartCoroutine(MoveRandomObjects());
    }

    private void OnDisable()
    {
        StopCoroutine(co);
    }

    private IEnumerator MoveRandomObjects()
    {
        while (true)
        {
            //rb.velocity = new Vector3(Random.Range(-xMagnitude, xMagnitude), Random.Range(-yMagnitude, yMagnitude), Random.Range(-zMagnitude, zMagnitude));

            var rnd = Random.insideUnitCircle;
            Vector3 currentDirection = new Vector3(rnd.x * 1000, 6f, rnd.y * 1000);
            yield return this;
            
            rb.AddForce(currentDirection * speed * Time.deltaTime, ForceMode.VelocityChange);
            
            yield return new WaitForSeconds(delay);
        }
    }
}
