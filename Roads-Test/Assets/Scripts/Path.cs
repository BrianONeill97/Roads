using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    [SerializeField,HideInInspector]
    List<Vector3> points;
    [SerializeField, HideInInspector]
    bool isClosed;
    [SerializeField, HideInInspector]
    bool autoSetPoints;

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

    public bool AutoSetControlPoints
    {
        get
        {
            return autoSetPoints;
        }
        set
        {
            if(autoSetPoints != value)
            {
                autoSetPoints = value;
                if(autoSetPoints)
                {
                    AutoSetAll();
                }
            }
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
            // each segment starts at 4,7,10,13 all divide by 3 and rouded down gives the number you want for an OPEN path
            // Even for a close path which starts at 6,9,12,15 divided by 3 still works perfectly
            return points.Count / 3; ;
        }
    }

    public void addSegment(Vector3 anchorPos)
    {
        points.Add(points[points.Count - 2] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);

        if(autoSetPoints)
        {
            AutoSetAllAffectedControlPoints(points.Count - 1);
        }
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[]
        {
            points[i * 3],
            points[i * 3 + 1],
            points[i * 3 + 2],
            points[Loop(i * 3 + 3)]
        };
    }

    public void movePoint(int i, Vector3 pos)
    {
        Vector3 deltaMove = pos - points[i];
        if (i % 3 == 0 || !autoSetPoints)
        {

            points[i] = pos;

            if (autoSetPoints)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {
                if (i % 3 == 0)
                {
                    if (i + 1 < points.Count || isClosed)
                    {
                        points[Loop(i + 1)] += deltaMove;
                    }
                    if (i - 1 >= 0 || isClosed)
                    {
                        points[Loop(i - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool nextPointAnchor = (i + 1) % 3 == 0;
                    int correspondingControlIndex = (nextPointAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointAnchor) ? i + 1 : i - 1;

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                    {
                        float dist = (points[Loop(anchorIndex)] - points[Loop(correspondingControlIndex)]).magnitude;
                        Vector3 dir = (points[Loop(anchorIndex)] - pos).normalized;
                        points[Loop(correspondingControlIndex)] = points[Loop(anchorIndex)] + dir * dist;
                    }
                }
            }
        }


    }

    public void toggleOpen()
    {
        isClosed = !isClosed;

        if(isClosed)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add(points[0] * 2 - points[1]);

            if (autoSetPoints)
            {
                AutoSetAnchorControlPoints(0);
                AutoSetAnchorControlPoints(points.Count - 3);
            }
        }
        else
        {
            points.RemoveRange(points.Count - 2, 2);
            if(autoSetPoints)
            {
                AutoStartAndEnd();
            }

        }
    }

    void AutoSetAllAffectedControlPoints(int updatedAnchors)
    {
        for(int i = updatedAnchors - 3; i <= updatedAnchors + 3; i+=3)
        {
            if(i >= 0 && i < points.Count || isClosed)
            {
                AutoSetAnchorControlPoints(Loop(i));
            }
        }
        AutoStartAndEnd();
    }

    void AutoSetAll()
    {
        for(int i = 0; i < points.Count; i+=3)
        {
            AutoSetAnchorControlPoints(i);
        }
        AutoStartAndEnd();

    }

    void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector3 anchorPos = points[anchorIndex];
        Vector3 dir = Vector2.zero;
        float[] neighbourDistances = new float[3];

        if (anchorIndex - 3 >= 0 || isClosed)
        {
            Vector3 offset = points[Loop(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
        if (anchorIndex + 3 >= 0 || isClosed)
        {
            Vector3 offset = points[Loop(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighbourDistances[1] = -offset.magnitude;
        }

        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[Loop(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
            }
        }

    }

    void AutoStartAndEnd()
    {
        if(!isClosed)
        {
            points[1] = (points[0] + points[2]) * 0.5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * 0.5f;
        }
    }

    int Loop(int i)
    {
        // points. cout is so negative values are handled
        return (i + points.Count) % points.Count;
    }
      
        
}
