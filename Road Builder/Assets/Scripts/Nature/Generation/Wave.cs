using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    Vector3 waveSource1 = new Vector3(2.0f,0.0f,2.0f);
    public float waveFrequency = 0.53f;
    public float waveHeight = 0.48f;
    public float wavelength = 0.71f;
    public bool Blend = true;
    public bool forceFlatShading = true;

    Mesh mesh;
    Vector3[] verts;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        MeshFilter mf = GetComponent<MeshFilter>();
        makeMeshLowPoly(mf);
    }

    // Update is called once per frame
    void Update()
    {
        wave();
        setBlend();
    }

    MeshFilter makeMeshLowPoly(MeshFilter mf)
    {
        mesh = mf.sharedMesh;
        Vector3[] oldVerts = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = new Vector3[triangles.Length];
        for(int i = 0; i < triangles.Length; i++)
        {
            vertices[i] = oldVerts[triangles[i]];
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        verts = mesh.vertices;
        return mf;
    }

    void setBlend()
    {
        //if(!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
        //{
        //    Blend = false;
        //}

        //if(Blend)
        //{
        //    Shader.EnableKeyword("WATER_BLEND_ON");
        //    if(Camera.main)
        //    {
        //        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        //    }
        //}
        //else
        //{
        //    Shader.DisableKeyword("WATER_BLEND_ON");
        //}
    }

    void wave()
    {
        for(int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            v.y = 0.0f;
            float dist = Vector3.Distance(v, waveSource1);
            dist = (dist % wavelength) / wavelength;
            v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * waveFrequency + (Mathf.PI * 2.0f * dist));
            verts[i] = v;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
