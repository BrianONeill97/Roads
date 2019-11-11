using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    List<Vector3> linePoints = new List<Vector3>();
    LineRenderer lineRenderer;
    public float startWidth = 1.0f;
    public float endWidth = 1.0f;

    public GameObject obj;
    Camera thisCamera;

    bool test = false;
    int lineCount = 0;
    Vector3 startP;
    Vector3 endP;

    /// <summary>
    /// Testing
    /// </summary>
    int numOfSegs = 0;
    int AlonThePath = 0;


    void Awake()
    {
        thisCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startP = Input.mousePosition;
            startP.z = (thisCamera.farClipPlane - 5) / 2;
            startP = thisCamera.ScreenToWorldPoint(startP);

            linePoints.Add(startP);
            test = true;
        }


        if(test == true)
        {
            if (!Input.GetMouseButton(0))
            {
                endP = Input.mousePosition;
                endP.z = (thisCamera.farClipPlane - 5) / 2;
                endP = thisCamera.ScreenToWorldPoint(endP);
                linePoints.Add(endP);
                startP = endP;
            }
            else
            {
                test = false;
            }
        }


        //if (Input.GetMouseButton(0))
        //{
        //    //linePoints.Add(endP);
        //}

        //if(Input.GetMouseButtonUp(0))
        //{
        //    linePoints.Add(endP);
        //    startP = endP;
        //    fP = false;
        //}
        if(linePoints.Count > 1)
        {
            StartCoroutine(GenerateRoad(startP, endP));

            UpdateLine();
        }
    }



    void UpdateLine()
    {
        //lineRenderer.SetWidth(startWidth, endWidth);
        //lineRenderer.SetVertexCount(linePoints.Count);

        for (int i = 0; i < linePoints.Count; i++)
        {
            Debug.DrawLine(linePoints[i], linePoints[i+1], Color.white, 100f, false);
            //    lineRenderer.SetPosition(i, linePoints[i]);
 
        }
    }

    IEnumerator GenerateRoad(Vector3 start, Vector3 end)
    {
        //float mag = Vector3.Distance(start, end);
        //mag = Mathf.Round(mag / GetSize(obj).x);

        //Vector3 dist = new Vector3
        //(
        //    start.x - end.x,
        //    start.y - end.y,
        //    start.z - end.z
        //);

        //Vector3 normDist = Vector3.Normalize(dist);

        numOfSegs += 1;
        AlonThePath = 1 / (numOfSegs);

        for (int i = 1; i < numOfSegs; i++)
        {
            Vector3 pos = start +(end - start)* (AlonThePath*i);
            Instantiate(obj, pos, Quaternion.identity);
        }
        yield return null;

    }

    Vector3 GetSize(GameObject obj)
    {
        Vector3 dimensions;
        float width = obj.GetComponent<Renderer>().bounds.size.x * transform.localScale.x;
        float height = obj.GetComponent<Renderer>().bounds.size.y * transform.localScale.y;
        float depth = obj.GetComponent<Renderer>().bounds.size.z * transform.localScale.z;
        dimensions = new Vector3(width, height, depth);
        return dimensions;
    }

}
