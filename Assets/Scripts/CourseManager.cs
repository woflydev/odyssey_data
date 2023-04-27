using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class CourseManager : MonoBehaviour
{
    public bool gizmo;
    public Parameters p;

    public Transform[] coursePoints;
    
    public MeshFilter leftFilter;
    public MeshFilter rightFilter;

    public MeshCollider leftCollider;
    public MeshCollider rightCollider;

    private Parameters _previousParams;

    // Start is called before the first frame update
    void Start()
    {
        p.coursePositions = coursePoints.Select(a => a.localPosition).ToArray();
        
        Mesh[] meshes = ReturnMeshes();
        leftFilter.mesh = meshes[0];
        rightFilter.mesh = meshes[2];
        
        leftCollider.sharedMesh = leftFilter.mesh;
        rightCollider.sharedMesh = rightFilter.mesh;
        
        _previousParams = p.CloneViaSerialization();
    }

    // Update is called once per frame
    void Update()
    {
        // If params have changed
        if (p.IsDifferent(_previousParams))
        {
            Debug.Log("Updated!");
            Mesh[] meshes = ReturnMeshes();
            leftFilter.mesh = meshes[0];
            rightFilter.mesh = meshes[2];

            leftCollider.sharedMesh = leftFilter.mesh;
            rightCollider.sharedMesh = rightFilter.mesh;
            
            _previousParams = p.CloneViaSerialization();
        }
    }

    void OnDrawGizmos()
    {
        // Converts from Transform to Vector3 because otherwise it points to the same location in memory
        // so it won't know when handles are moved if otherwise.
        p.coursePositions = coursePoints.Select(a => a.localPosition).ToArray();
        // Shows gizmos
        ReturnMeshes(gizmo);
        
        /*Debug.Log(p);
        Debug.Log(_previousParams);
        if (p == null || _previousParams == null)
        {
            p = new Parameters();
            _previousParams = new Parameters();
            // Shows gizmos
            ReturnMeshes(true);
        } else if (p.IsDifferent(_previousParams))
        {
            // Shows gizmos
            ReturnMeshes(true);
            _previousParams = p.CloneViaSerialization();
        }*/
        
    }

    Mesh[] ReturnMeshes(bool showGizmo = false)
    {
        Mesh leftMesh = new Mesh();
        Mesh centreMesh = new Mesh();
        Mesh rightMesh = new Mesh();


        int splines = Mathf.FloorToInt(p.coursePositions.Length / 2) - (p.closedLoop ? 0 : 1);
        int loops = Mathf.FloorToInt(1f / p.resolution);
        // Only the central curve in the middle of the thick curve will be counted in the mesh
        int meshVerticesNum = splines * (loops + (p.closedLoop ? 0 : 1)) * 2;

        Vector3[] leftMeshVertices = new Vector3[meshVerticesNum];
        Vector3[] rightMeshVertices = new Vector3[meshVerticesNum];

        Vector3[] centreMeshVertices = new Vector3[meshVerticesNum];

        for (int spline = 0; spline < splines; spline++)
        {
            Vector3[] currentSpline = Spline(p.coursePositions, spline);

            if (showGizmo)
            {
                Gizmos.DrawLine(currentSpline[1], Vector3.LerpUnclamped(currentSpline[1], currentSpline[0], 2));
                Gizmos.DrawLine(currentSpline[2], Vector3.LerpUnclamped(currentSpline[2], currentSpline[3], 2));
            }

            // Draws loops line segments along the Bezier curve
            for (int start = 0; start < loops; start++)
            {
                int currentIndex = 2 * spline * loops + 2 * start;
                Vector3 startPoint = DeCasteljau(start * p.resolution, currentSpline);
                Vector3 endPoint = DeCasteljau((start + 1) * p.resolution, currentSpline);
                /*meshVertices[(spline + 1) * (2 * start + 1) - 1] = startPoint;
                meshVertices[(spline + 1) * (2 * start + 1)] = endPoint;*/

                Vector3 difference = endPoint - startPoint;
                Vector3 normal = Vector3.Cross(difference, Vector3.up).normalized;

                leftMeshVertices[currentIndex] = startPoint + p.gap / 2 * normal;
                leftMeshVertices[currentIndex + 1] = startPoint + (p.thickness + p.gap) / 2 * normal;

                centreMeshVertices[currentIndex] = startPoint - p.thickness / 2 * normal;
                centreMeshVertices[currentIndex + 1] = startPoint + p.thickness / 2 * normal;

                rightMeshVertices[currentIndex] = startPoint - (p.thickness + p.gap) / 2 * normal;
                rightMeshVertices[currentIndex + 1] = startPoint - p.gap / 2 * normal;

                if (spline == splines - 1 && !p.closedLoop)
                {
                    leftMeshVertices[currentIndex + 2] = endPoint + p.gap / 2 * normal;
                    leftMeshVertices[currentIndex + 3] = endPoint + (p.thickness + p.gap) / 2 * normal;

                    centreMeshVertices[currentIndex + 2] = endPoint - p.thickness / 2 * normal;
                    centreMeshVertices[currentIndex + 3] = endPoint + p.thickness / 2 * normal;

                    rightMeshVertices[currentIndex + 2] = endPoint - (p.thickness + p.gap) / 2 * normal;
                    rightMeshVertices[currentIndex + 3] = endPoint - p.gap / 2 * normal;
                }

                if (showGizmo)
                {
                    // Add p.thickness to line
                    for (float i = -p.thickness / 2; i <= p.thickness / 2; i += p.thicknessRes)
                    {
                        Gizmos.DrawLine(startPoint + normal * i, endPoint + normal * i);
                    }
                }

                //Gizmos.DrawLine(startPoint, endPoint);
            }
        }

        leftMesh.vertices = leftMeshVertices;
        centreMesh.vertices = centreMeshVertices;
        rightMesh.vertices = rightMeshVertices;

        leftMesh.SetIndices(TriangularIndices(meshVerticesNum, p.closedLoop), MeshTopology.Triangles, 0);
        centreMesh.SetIndices(TriangularIndices(meshVerticesNum, p.closedLoop), MeshTopology.Triangles, 0);
        rightMesh.SetIndices(TriangularIndices(meshVerticesNum, p.closedLoop), MeshTopology.Triangles, 0);

        Mesh[] meshArr = new Mesh[3];
        meshArr[0] = leftMesh;
        meshArr[1] = centreMesh;
        meshArr[2] = rightMesh;

        return meshArr;
    }

    int[] TriangularIndices(int n, bool loop)
    {
        int[] list = new int[3 * n];
        // For each quad, create two triangles
        for (int i = 0; i < Mathf.FloorToInt(n / 2) - (loop ? 0 : 1); i++)
        {
            // First triangle
            list[6 * i] = (2 * i) % n;
            list[6 * i + 1] = (2 * i + 1) % n;
            list[6 * i + 2] = (2 * i + 2) % n;

            // Second triangle
            list[6 * i + 3] = (2 * i + 3) % n;
            list[6 * i + 4] = (2 * i + 2) % n;
            list[6 * i + 5] = (2 * i + 1) % n;
        }

        return list;
    }

    Vector3[] Spline(Vector3[] transforms, int n)
    {
        // Firstly point-handle, then handle-point, then handle-point (format of the transforms)
        // The if statements are to make the first 4 points more intuitive to use
        Vector3[] positions = new Vector3[4];
        if (n == 0)
        {
            return transforms.Skip(0).Take(4).ToArray();
        }

        // The below if statement completes a loop
        if (n == transforms.Length / 2 - 1)
        {
            positions[2] = Vector3.LerpUnclamped(transforms[1], transforms[0], 2);
            positions[3] = transforms[0];
        }
        else
        {
            positions[2] = transforms[2 * n + 2];
            positions[3] = transforms[2 * n + 3];
        }

        positions[0] = transforms[2 * n + 1];
        positions[1] =
            Vector3.LerpUnclamped(transforms[2 * n], transforms[2 * n + 1], 2);


        return positions;
    }

    Vector3 DeCasteljau(float t, in Vector3[] points)
    {
        Vector3[] oldPoints = points;
        Vector3[] newPoints = new Vector3[points.Length - 1];
        for (int j = 0; j < points.Length - 1; j++)
        {
            for (int i = 0; i < oldPoints.Length - 1; i++)
            {
                newPoints[i] = Vector3.Lerp(oldPoints[i], oldPoints[i + 1], t);
            }

            // Resets the process
            oldPoints = newPoints;
            if (oldPoints.Length > 0)
            {
                newPoints = new Vector3[oldPoints.Length - 1];
            }
        }

        return oldPoints[0];
    }
}

[Serializable]
public class Parameters
{
    public Vector3[] coursePositions;
    public float resolution = 0.02f;
    public float thicknessRes = 0.005f;
    public float thickness = 0.1f;
    public float gap = 0.5f;
    public bool closedLoop;

    public bool IsDifferent(Parameters a)
    {
        if (coursePositions.Length != a.coursePositions.Length)
        {
            return true;
        }
        
        for (int i = 0; i < coursePositions.Length; i++)
        {
            if (coursePositions[i] != a.coursePositions[i])
            {
                return true;
            }
        }

        return (resolution != a.resolution ||
                thicknessRes != a.thicknessRes ||
                thickness != a.thickness ||
                gap != a.gap ||
                (closedLoop ^ a.closedLoop));
    }
}