using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField,HideInInspector]
    List<Vector3> points;

    public Path(Vector3 centre)
    {
        points = new List<Vector3>
        {
            centre+Vector3.left,
            centre+(Vector3.left+Vector3.up)*0.5f,
            centre+(Vector3.left+Vector3.up)*0.5f,
            centre+Vector3.right
        };
    }

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }


    public int NumSegments
    {
        get
        {
            return (points.Count - 4) / 3 + 1;
        }
    }

    public void addSegment(Vector3 anchorPos)
    {
        points.Add(points[points.Count - 2] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[]
        {
            points[i * 3],
            points[i * 3 + 1],
            points[i * 3 + 2],
            points[i * 3 + 3]
        };
    }

    public void movePoint(int i, Vector3 pos)
    {
        Vector3 deltaMove = pos - points[i];
        points[i] = pos;

        if(i % 3 == 0)
        {
            if(i + 1 < points.Count)
            {
                points[i + 1] += deltaMove;
            }
            if(i - 1 >= 0)
            {
                points[i - 1] += deltaMove;
            }
        }
        else
        {
            bool nextPointAnchor = (i + 1) % 3 == 0;
            int correspondingControlIndex = (nextPointAnchor) ? i + 2 : i - 2;
            int anchorIndex = (nextPointAnchor) ? i + 1 : i - 1;

            if(correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
            {
                float dist = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
                Vector3 dir = (points[anchorIndex] - pos).normalized;
                points[correspondingControlIndex] = points[anchorIndex] + dir * dist;
            }
        }


    }
}
