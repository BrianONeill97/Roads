using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Utility
{
    public static Vector3 GetSize(GameObject obj)
    {
        Vector3 dimensions;
        float width = obj.GetComponent<Renderer>().bounds.size.x * obj.transform.localScale.x;
        float height = obj.GetComponent<Renderer>().bounds.size.y * obj.transform.localScale.y;
        float depth = obj.GetComponent<Renderer>().bounds.size.z * obj.transform.localScale.z;
        dimensions = new Vector3(width, height, depth);
        return dimensions;
    } // Gets the ACTUAL size of the tile}

    //Gets the mouse position in the game
    public static Vector3 _get3dMousePosition()
    {
        Vector3 newPos = new Vector3();
        newPos = Input.mousePosition;
        newPos.z = Camera.main.farClipPlane - 1;
        newPos = Camera.main.ScreenToWorldPoint(newPos);
        return newPos;
    } // Gets the mouse position 

    //Gets magnitude
    public static float GetMag(Vector3 start, Vector3 end)
    {
        float mag = Mathf.Sqrt(((start.x * end.x) + (start.y * start.y) + (start.z * end.z)));
        return mag;
    } // Gets the magnitude of the line Vector

    // Combines the meshs of all the children
    public static void combineMesh(GameObject t_object)
    {
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = t_object.transform.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = t_object.transform.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = t_object.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = t_object.transform.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = t_object.AddComponent<MeshRenderer>();
        }

        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);

        // Destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            MonoBehaviour.DestroyImmediate(oldMesh);
        }

        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;


        for (int i = 0; i < t_object.transform.childCount; i++)
        {
            MonoBehaviour.Destroy(t_object.transform.GetChild(i).gameObject);
        }

    }
    public static int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }

    // Save Asset
    public static void SaveAsset(GameObject t_object)
    {
        //MeshFilter m_meshFilter = t_object.GetComponent<MeshFilter>();

        //if(m_meshFilter)
        //{
        //    string path = "Assets/Prefabs/Track" + TrackNumber.trackNum.ToString() + "/Meshes/" + t_object.name.ToString() + TrackNumber.meshNum.ToString();
        //    Debug.Log("Saved Mesh to:" + path);
        //    AssetDatabase.CreateAsset(m_meshFilter.mesh, path);
        //    TrackNumber.meshNum++;
        //}
    }
}
