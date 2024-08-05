//Made by Klausbdl

using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
#endif
public class TiledMesh : MonoBehaviour
{
    public Vector3 scale;
    public GameObject originalMesh;
    public bool hasCollider = true;
    public bool forceUpdate;
   
    MeshFilter filter;
    MeshCollider meshCollider;
    //Change these to avoid getting vertices that are close to the center. Change only to positive values!
    float xThreshold = 0;
    float yThreshold = 0;
    float zThreshold = 0;

    //Vertices
    Vector3[] deformedVertices;
    List<int>[] cornersIndexes;

#if UNITY_EDITOR
    //variables used during editor time
    Quaternion lastRotation = Quaternion.identity;
#endif
    string lastObject = ""; //used to track if changed the OriginalMesh variable

    private void OnValidate()
    {
        if (originalMesh == null) return;
        else
        {
            if(originalMesh.name != lastObject || forceUpdate)
            {
                lastObject = originalMesh.name;
                forceUpdate = false;

                filter.mesh = originalMesh.GetComponent<MeshFilter>().sharedMesh;
                deformedVertices = filter.mesh.vertices;
                FindCorners();
                UpdateMesh();
                return;
            }            
        }

        //additional checks
        if(cornersIndexes == null) FindCorners();

        UpdateMesh();
    }

    void FindCorners()
    {
        if (deformedVertices == null) deformedVertices = originalMesh.GetComponent<MeshFilter>().sharedMesh.vertices;

        cornersIndexes = new List<int>[8];
        for (int i = 0; i < cornersIndexes.Length; i++)
            cornersIndexes[i] = new List<int>();

        for (int i = 0; i < originalMesh.GetComponent<MeshFilter>().sharedMesh.vertices.Length; i++)
        {
            Vector3 v = originalMesh.GetComponent<MeshFilter>().sharedMesh.vertices[i];
            if(v.x < -xThreshold && v.y > yThreshold && v.z > zThreshold) //0
            {
                cornersIndexes[0].Add(i);
            }
            if (v.x > xThreshold && v.y > yThreshold && v.z > zThreshold) //1
            {
                cornersIndexes[1].Add(i);
            }
            if (v.x < -xThreshold && v.y > yThreshold && v.z < -zThreshold) //2
            {
                cornersIndexes[2].Add(i);
            }
            if (v.x > xThreshold && v.y > yThreshold && v.z < -zThreshold) //3
            {
                cornersIndexes[3].Add(i);
            }
            if (v.x < -xThreshold && v.y < -yThreshold && v.z > zThreshold) //4
            {
                cornersIndexes[4].Add(i);
            }
            if (v.x > xThreshold && v.y < -yThreshold && v.z > zThreshold) //5
            {
                cornersIndexes[5].Add(i);
            }
            if (v.x < -xThreshold && v.y < -yThreshold && v.z < -zThreshold) //6
            {
                cornersIndexes[6].Add(i);
            }
            if (v.x > xThreshold && v.y < -yThreshold && v.z < -zThreshold) //7
            {
                cornersIndexes[7].Add(i);
            }
        }
    }

    public void UpdateMesh()
    {
        for (int i = 0; i < cornersIndexes.Length; i++)
        {
            for (int j = 0; j < cornersIndexes[i].Count; j++)
            {
                int index = cornersIndexes[i][j];
                Vector3 scaledOffset = Vector3.zero;

                switch (i)
                {
                    case 0:
                        scaledOffset = new Vector3(-scale.x, scale.y, scale.z);
                        break;
                    case 1:
                        scaledOffset = new Vector3(scale.x, scale.y, scale.z);
                        break;
                    case 2:
                        scaledOffset = new Vector3(-scale.x, scale.y, -scale.z);
                        break;
                    case 3:
                        scaledOffset = new Vector3(scale.x, scale.y, -scale.z);
                        break;
                    case 4:
                        scaledOffset = new Vector3(-scale.x, -scale.y, scale.z);
                        break;
                    case 5:
                        scaledOffset = new Vector3(scale.x, -scale.y, scale.z);
                        break;
                    case 6:
                        scaledOffset = new Vector3(-scale.x, -scale.y, -scale.z);
                        break;
                    case 7:
                        scaledOffset = new Vector3(scale.x, -scale.y, -scale.z);
                        break;
                }

                Vector3 localVertexPosition = originalMesh.GetComponent<MeshFilter>().sharedMesh.vertices[index] + scaledOffset;

                deformedVertices[index] = localVertexPosition;
            }
        }

        //Update the mesh
        filter.mesh.vertices = deformedVertices;
        
        //This gives me an ugly look because of multiple vertices on top of one another in some spots
        //filter.mesh.RecalculateNormals();
        
        filter.sharedMesh.RecalculateBounds();
        if(meshCollider != null)
            meshCollider.sharedMesh = filter.sharedMesh;
    }

#if UNITY_EDITOR
    //anything inside this if UNITY_EDITOR will be discarded in build
    private void MyUpdate()
    {
        //update mesh when rotating the object in editor time
        if(transform.rotation.eulerAngles != lastRotation.eulerAngles)
        {
            lastRotation = transform.rotation;
            FindCorners();
            UpdateMesh();
        }
    }
#endif
    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update += MyUpdate;
#endif
        filter = GetComponent<MeshFilter>();
        if(!TryGetComponent(out meshCollider) && hasCollider)
            meshCollider = gameObject.AddComponent<MeshCollider>();
    }
#if UNITY_EDITOR
    private void OnDisable()
    {
        EditorApplication.update -= MyUpdate;
    }

    private void OnDrawGizmosSelected()
    {
        if (deformedVertices == null) return;

        Color[] colors = new Color[8] { Color.yellow, Color.black, Color.blue, Color.green, Color.gray, Color.red, Color.cyan, Color.magenta };
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            if (cornersIndexes[0].Contains(i)) Gizmos.color = colors[0];
            else if (cornersIndexes[1].Contains(i)) Gizmos.color = colors[1];
            else if (cornersIndexes[2].Contains(i)) Gizmos.color = colors[2];
            else if (cornersIndexes[3].Contains(i)) Gizmos.color = colors[3];
            else if (cornersIndexes[4].Contains(i)) Gizmos.color = colors[4];
            else if (cornersIndexes[5].Contains(i)) Gizmos.color = colors[5];
            else if (cornersIndexes[6].Contains(i)) Gizmos.color = colors[6];
            else if (cornersIndexes[7].Contains(i)) Gizmos.color = colors[7];

            Vector3 localVertexPosition = deformedVertices[i];

            // Transform to world position considering rotation and position
            localVertexPosition = transform.rotation * localVertexPosition + transform.position;

            Gizmos.DrawSphere(localVertexPosition, 0.05f);
        }
    }
#endif
}