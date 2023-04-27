﻿using PathCreation;
using UnityEngine;

namespace PathCreation.Examples {

    [ExecuteInEditMode]
    public class PathPlacer : PathSceneTool {

        public GameObject prefab;
        public GameObject holder;
        
        [Range(0.000f, 1.000f)]
        public float seed;
        
        [Range(0.0f, 3.0f)]
        public float maxSpacing = 3f;
        
        private float minSpacing = .1f;
        public float xOffset = 2f;
        public float zOffset = 1f;
        public float xRotOffset = 45f;
        public float zRotOffset = 45f;

        void Generate () {
            if (pathCreator != null && prefab != null && holder != null) {
                DestroyObjects ();

                VertexPath path = pathCreator.path;

                maxSpacing = Mathf.Max(minSpacing, maxSpacing);
                float dst = 0;

                while (dst < path.length) {
                    Vector3 point = path.GetPointAtDistance(dst);

                    point.x += Random.Range(-xOffset, xOffset) * seed;
                    point.z += Random.Range(-zOffset, zOffset) * seed;
                    
                    Quaternion rot = path.GetRotationAtDistance(dst);
                    
                    rot.x += Random.Range(-xRotOffset, xRotOffset) * seed;
                    rot.z += Random.Range(-zRotOffset, zRotOffset) * seed;
                    
                    Instantiate (prefab, point, rot, holder.transform);
                    dst += Random.Range(minSpacing, maxSpacing);
                }
            }
        }

        void DestroyObjects () {
            int numChildren = holder.transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--) {
                DestroyImmediate (holder.transform.GetChild (i).gameObject, false);
            }
        }

        protected override void PathUpdated () {
            if (pathCreator != null) {
                Generate ();
            }
        }
    }
}