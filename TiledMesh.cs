//Made by Klausbdl
//Best used in a prefab, so don't add this component to an object at runtime.

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
#endif
public class TiledMesh : MonoBehaviour
{
    public Vector3 scale;
    //Change these to avoid getting vertices that are close to the center. Change only to positive values!
    public float xThreshold = 0;
    public float yThreshold = 0;
    public float zThreshold = 0;
    [Space(8)]
    public GameObject originalMesh;
    public bool hasCollider = true;
    [Tooltip("Check this to force update the corners, in case it looks wrong.")]
    public bool forceUpdate;

    MeshFilter filter;
    MeshCollider meshCollider;

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
            if (originalMesh.name != lastObject || forceUpdate)
            {
                lastObject = originalMesh.name;
                forceUpdate = false;

                if (filter == null) filter = GetComponent<MeshFilter>();

                filter.mesh = originalMesh.GetComponent<MeshFilter>().sharedMesh;
                deformedVertices = filter.mesh.vertices;
                FindCorners();
                UpdateMesh();
                return;
            }
        }

        //additional checks
        if (cornersIndexes == null) FindCorners();
        if (filter.mesh == null) filter.mesh = originalMesh.GetComponent<MeshFilter>().sharedMesh;
        if (deformedVertices == null) deformedVertices = filter.sharedMesh.vertices;

        if (xThreshold < 0) xThreshold = 0;
        if (yThreshold < 0) yThreshold = 0;
        if (zThreshold < 0) zThreshold = 0;

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
            if (v.x < 0 && v.y > 0 && v.z > 0) //0
            {
                cornersIndexes[0].Add(i);
            }
            if (v.x > 0 && v.y > 0 && v.z > 0) //1
            {
                cornersIndexes[1].Add(i);
            }
            if (v.x < 0 && v.y > 0 && v.z < 0) //2
            {
                cornersIndexes[2].Add(i);
            }
            if (v.x > 0 && v.y > 0 && v.z < 0) //3
            {
                cornersIndexes[3].Add(i);
            }
            if (v.x < 0 && v.y < 0 && v.z > 0) //4
            {
                cornersIndexes[4].Add(i);
            }
            if (v.x > 0 && v.y < 0 && v.z > 0) //5
            {
                cornersIndexes[5].Add(i);
            }
            if (v.x < 0 && v.y < 0 && v.z < 0) //6
            {
                cornersIndexes[6].Add(i);
            }
            if (v.x > 0 && v.y < 0 && v.z < 0) //7
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
                Vector3 origV = originalMesh.GetComponent<MeshFilter>().sharedMesh.vertices[index];

                switch (i)
                {
                    case 0:
                        scaledOffset = new Vector3(-scale.x, scale.y, scale.z);
                        if (origV.x >= -xThreshold) scaledOffset.x = 0;
                        if (origV.y <= yThreshold) scaledOffset.y = 0;
                        if (origV.z <= zThreshold) scaledOffset.z = 0;
                        break;
                    case 1:
                        scaledOffset = new Vector3(scale.x, scale.y, scale.z);
                        if (origV.x <= xThreshold) scaledOffset.x = 0;
                        if (origV.y <= yThreshold) scaledOffset.y = 0;
                        if (origV.z <= zThreshold) scaledOffset.z = 0;
                        break;
                    case 2:
                        scaledOffset = new Vector3(-scale.x, scale.y, -scale.z);
                        if (origV.x >= -xThreshold) scaledOffset.x = 0;
                        if (origV.y <= yThreshold) scaledOffset.y = 0;
                        if (origV.z >= -zThreshold) scaledOffset.z = 0;
                        break;
                    case 3:
                        scaledOffset = new Vector3(scale.x, scale.y, -scale.z);
                        if (origV.x <= xThreshold) scaledOffset.x = 0;
                        if (origV.y <= yThreshold) scaledOffset.y = 0;
                        if (origV.z >= -zThreshold) scaledOffset.z = 0;
                        break;
                    case 4:
                        scaledOffset = new Vector3(-scale.x, -scale.y, scale.z);
                        if (origV.x >= -xThreshold) scaledOffset.x = 0;
                        if (origV.y >= -yThreshold) scaledOffset.y = 0;
                        if (origV.z <= zThreshold) scaledOffset.z = 0;
                        break;
                    case 5:
                        scaledOffset = new Vector3(scale.x, -scale.y, scale.z);
                        if (origV.x <= xThreshold) scaledOffset.x = 0;
                        if (origV.y >= -yThreshold) scaledOffset.y = 0;
                        if (origV.z <= zThreshold) scaledOffset.z = 0;
                        break;
                    case 6:
                        scaledOffset = new Vector3(-scale.x, -scale.y, -scale.z);
                        if (origV.x >= -xThreshold) scaledOffset.x = 0;
                        if (origV.y >= -yThreshold) scaledOffset.y = 0;
                        if (origV.z >= -zThreshold) scaledOffset.z = 0;
                        break;
                    case 7:
                        scaledOffset = new Vector3(scale.x, -scale.y, -scale.z);
                        if (origV.x <= xThreshold) scaledOffset.x = 0;
                        if (origV.y >= -yThreshold) scaledOffset.y = 0;
                        if (origV.z >= -zThreshold) scaledOffset.z = 0;
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
        if (meshCollider != null)
            meshCollider.sharedMesh = filter.sharedMesh;
    }

#if UNITY_EDITOR
    //anything inside this if UNITY_EDITOR will be discarded in build
    private void MyUpdate()
    {
        //update mesh when rotating the object in editor time
        if (transform.rotation.eulerAngles != lastRotation.eulerAngles)
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
        if (!TryGetComponent(out meshCollider) && hasCollider)
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
        Gizmos.matrix = transform.localToWorldMatrix;

        Color[] colors = new Color[8] { Color.yellow, Color.black, Color.blue, Color.green, Color.gray, Color.red, Color.cyan, Color.magenta };
        for (int i = 0; i < filter.sharedMesh.vertices.Length; i++)
        {
            if (cornersIndexes[0].Contains(i)) Gizmos.color = colors[0];
            else if (cornersIndexes[1].Contains(i)) Gizmos.color = colors[1];
            else if (cornersIndexes[2].Contains(i)) Gizmos.color = colors[2];
            else if (cornersIndexes[3].Contains(i)) Gizmos.color = colors[3];
            else if (cornersIndexes[4].Contains(i)) Gizmos.color = colors[4];
            else if (cornersIndexes[5].Contains(i)) Gizmos.color = colors[5];
            else if (cornersIndexes[6].Contains(i)) Gizmos.color = colors[6];
            else if (cornersIndexes[7].Contains(i)) Gizmos.color = colors[7];

            Vector3 localVertexPosition = filter.sharedMesh.vertices[i];

            Gizmos.DrawSphere(localVertexPosition, 0.05f);
        }

        //draw thresholds
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(xThreshold * 2, filter.sharedMesh.bounds.size.y, filter.sharedMesh.bounds.size.z));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(filter.sharedMesh.bounds.size.x, yThreshold * 2, filter.sharedMesh.bounds.size.z));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(filter.sharedMesh.bounds.size.x, filter.sharedMesh.bounds.size.y, zThreshold * 2));
    }
#endif
}