using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    List<Vector3> linePoints = new List<Vector3>();
    LineRenderer lineRenderer;
    public float startWidth = 1.0f;
    public float endWidth = 1.0f;
    public float threshold = 0.001f;
    public GameObject obj;
    Camera thisCamera;
    int lineCount = 0;

    Vector3 lastPos = Vector3.one * float.MaxValue;


    void Awake()
    {
        thisCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = (thisCamera.farClipPlane - 5) / 2;
            Vector3 mouseWorld = thisCamera.ScreenToWorldPoint(mousePos);

            float dist = Vector3.Distance(lastPos, mouseWorld);
            if (dist <= threshold)
                return;

            lastPos = mouseWorld;
            if (linePoints == null)
                linePoints = new List<Vector3>();
            linePoints.Add(mouseWorld);

            UpdateLine();
        }
    }


    void UpdateLine()
    {
        lineRenderer.SetWidth(startWidth, endWidth);
        lineRenderer.SetVertexCount(linePoints.Count);

        for (int i = lineCount; i < linePoints.Count; i++)
        {
            
            lineRenderer.SetPosition(i, linePoints[i]);
            GameObject part = Instantiate(obj, transform);
            part.transform.parent = gameObject.transform;
            Vector3 pos = Vector3.Lerp(lastPos, linePoints[i],0.00000f);
            part.transform.position = pos;
            part.transform.rotation = Quaternion.identity;
        }
        lineCount = linePoints.Count;
    }

}
