using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Math Version of Lerp = (1-t)a +tb

public class Road : UniqueMesh
{
    
    Vector2[] verts;
    Vector2[] normals;
    float[] us;
    int[] lines = new int[]{
    0, 1,
    2, 3,
    3, 4,
    4, 5
    };

    Vector3[] vertices = new Vector3[]
    {
        new Vector3( 1,0,1),
        new Vector3(-1,0,1),
        new Vector3(1,0,-1),
        new Vector3(-1,0,-1)
    };

    //Since its a simple mesh using vector3.up would also work
    Vector3[] normalPos = new Vector3[]
    {
        Vector3.up,
        Vector3.up,
        Vector3.up,
        Vector3.up,
    };

    Vector2[] uvs = new Vector2[]
    {
                  //U V
        new Vector2(0,1),
        new Vector2(0,0),
        new Vector2(1,1),
        new Vector2(1,0)
    };

    int[] triangles = new int[]
    {
        0,2,3,
        3,1,0
    };

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>(); // gets mesh filter on object
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        Mesh mesh = mf.sharedMesh;
       ExtrudeShape(mesh, new ExtrudeShape(),);

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normalPos;
        mesh.uv = uvs;
        mesh.triangles = triangles;

    }

    public void Extrude(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path)
    {
        int vertsInShape = shape.verts2D.Length;
        int segments = path.Length - 1;
        int edgeLoops = path.Length;
        int vertCount = vertsInShape * edgeLoops;
        int triCount = shape.lines.Length * segments;
        int triIndexCount = triCount * 3;

        int[] triangleIndices = new int[triIndexCount];
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        for (int i = 0; i < path.Length; i++)
        {
            int offset = i * vertsInShape;
            for (int j = 0; j < vertsInShape; j++)
            {
                int id = offset + j;
                vertices[id] = path[i].LocalWorld(shape.verts2D[j]);
                normals[id] = path[i].LocalToWorldDirection(shape.normal[i]);
                uvs[id] = new Vector2(shape.us[i], i / ((float)edgeLoops));
            }
        }
        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int offset = i * vertsInShape;
            for (int l = 0; l < lines.Length; l += 2)
            {
                int a = offset + lines[l] + vertsInShape;
                int b = offset + lines[l];
                int c = offset + lines[l + 1];
                int d = offset + lines[l + 1] + vertsInShape;
                triangleIndices[ti] = a; ti++;
                triangleIndices[ti] = b; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = d; ti++;
                triangleIndices[ti] = a; ti++;
            }
        }



        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normalPos;
        mesh.uv = uvs;
        mesh.triangles = triangles;

    }



    Vector3 GetPoint(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        return pts[0] * (omt2 * omt) +
                pts[1] * (3f * omt2 * t) +
                pts[2] * (3f * omt * t2) +
                pts[3] * (t2 * t);
    }

    Vector3 GetTangent(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        Vector3 tangent = pts[0] * (-omt2) +
          pts[1] * (3 * omt2 -2 * omt) +
          pts[2] * (3 * t2 * 2 * t) +
          pts[3] * (t2);

        return tangent.normalized;
    }

    Vector3 GetNormal3D(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }

    Quaternion GetOrientation3D( Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetTangent(pts, t);
        Vector3 nrm = GetNormal3D(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }
}

public struct OrientedPoint
{
    public Vector3 Position;
    public Quaternion rotation;

    public OrientedPoint(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.rotation = rotation;
    }

    public Vector3 LocalWorld(Vector3 point)
    {
        return Position + rotation * point;
    }

    public Vector3 WorldToLocal(Vector3 point)
    {
        return Quaternion.Inverse(rotation) * (point - Position);
    }

    public Vector3 LocalToWorldDirection(Vector3 dir)
    {
        return rotation * dir;
    }

}

