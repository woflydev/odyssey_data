using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PathCreation.Examples {

    [ExecuteInEditMode]
    [RequireComponent(typeof(RoadMeshCreator))]
    public class PathPlacer : PathSceneTool
    {
        public Color32 normalColour = new Color32(171, 0, 255, 255);

        public GameObject prefab;
        public GameObject maskedPrefab;
        public GameObject objectHolder;
        public GameObject maskedObjectHolder;

        public GameObject[] randomObjects;
        public GameObject randomObjHolder;
        
        public int holderIndex;

        public bool spawnObjects;

        [Range(0.000f, 1.000f)]
        public float seed;
        
        [Range(2.0f, 50.0f)]
        public float maxSpacing = 3f;
        
        private float minSpacing = .1f;
        public float xOffset = 2f;
        public float zOffset = 1f;
        public float xRotOffset = 45f;
        public float yRotOffset = 45f;
        public float zRotOffset = 45f;

        private List<GameObject> spawnedObj;

        private void Generate() 
        {
            if (!spawnObjects)
            {
                DestroyObjects();
            }

            if (pathCreator != null && prefab != null && objectHolder != null && randomObjHolder != null && spawnObjects) {
                DestroyObjects();
                
                VertexPath path = pathCreator.path;

                maxSpacing = Mathf.Max(minSpacing, maxSpacing);
                float dst = 0;

                while (dst < path.length) {
                    Vector3 point = path.GetPointAtDistance(dst);
                    point.x += Random.Range(-xOffset, xOffset) * seed;
                    point.z += Random.Range(-zOffset, zOffset) * seed;
                    point.y += 0.8f;

                    Vector3 randomPoint = point;
                    randomPoint.y += Random.Range(0.5f, 2);
                    randomPoint.x += Random.Range(2f, 7f);
                    randomPoint.z += Random.Range(2f, 7f);
                    
                    Quaternion rot = path.GetRotationAtDistance(dst);
                    rot.x += Random.Range(-xRotOffset, xRotOffset);
                    rot.y += Random.Range(-yRotOffset, yRotOffset);
                    rot.z += Random.Range(-zRotOffset, zRotOffset);

                    GameObject normal = Instantiate(prefab, point, rot, objectHolder.transform);
                    GameObject masked = Instantiate(maskedPrefab, point, rot, maskedObjectHolder.transform);

                    if (Random.Range(1, 100) <= 100)
                    {
                        GameObject random = Instantiate(randomObjects[Random.Range(0, randomObjects.Length)], randomPoint,
                            rot, randomObjHolder.transform);
                    }
                    
                    MeshRenderer noShadows = masked.AddComponent<MeshRenderer>();
                    noShadows.receiveShadows = false;
                    
                    //spawnedObj.Add(spawned);

                    dst += Random.Range(minSpacing, maxSpacing);
                }
            }
        }

        private void DestroyObjects() 
        {
            int numChildren = objectHolder.transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--) {
                DestroyImmediate(objectHolder.transform.GetChild(i).gameObject, false);
                DestroyImmediate(maskedObjectHolder.transform.GetChild(i).gameObject, false);
            }

            int numRandmObj = randomObjHolder.transform.childCount;
            for (int i = numRandmObj - 1; i >= 0; i--)
            {
                DestroyImmediate(randomObjHolder.transform.GetChild(i).gameObject, false);
            }
        }

        protected override void PathUpdated()
        {
            if (pathCreator != null) {
                Generate();
            }
            
            if (objectHolder == null)
            {
                objectHolder = new GameObject("(" + holderIndex + ") Object Holder");
                objectHolder.transform.parent = transform.parent;
            }
            
            if (maskedObjectHolder == null)
            {
                maskedObjectHolder = new GameObject("(" + holderIndex + ") Masked Object Holder");
                maskedObjectHolder.transform.parent = transform.parent;
            }

            if (randomObjHolder == null)
            {
                randomObjHolder = new GameObject("(" + holderIndex + ") Random Object Holder");
            }
        }
        
        /*public void ChangeObjectColour(bool mask)
        {
            foreach (var obj in spawnedObj)
            {
                if (mask)
                {
                    obj.GetComponentInChildren<Renderer>().sharedMaterial.color = Color.green;
                }

                else
                {
                    obj.GetComponentInChildren<Renderer>().sharedMaterial.color = normalColour;
                }
            }
        }*/
    }
}