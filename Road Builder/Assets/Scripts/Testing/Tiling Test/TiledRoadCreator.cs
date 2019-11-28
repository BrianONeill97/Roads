using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledRoadCreator : MonoBehaviour
{
    [Header ("Objects")]
    public GameObject straightRoad;
    public GameObject cornerRoad;

    Camera thisCamera;
    private bool _isDraging = false;

    Quaternion rot;
    Vector3 placementPosition;
    Vector3 currentMousePosition;
    private Vector3 lastPlacedObjectLocation;
    float angle;

    bool started = false;
    Vector3 startPoint;


    // Tests
    Vector3 newStart;
    GameObject prevObj;
    bool turningLeft = true;
    bool turningRight = true;

    private void Awake()
    {
        thisCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (started == false)
            {
                startPoint = _get3dMousePosition();
                placePrefab(startPoint,straightRoad,Quaternion.identity);
                placementPosition = startPoint;
                started = true;
            }
                _isDraging = true;
        }


        if (_isDraging)
        {
            //Gets the position of the mouse
            currentMousePosition = _get3dMousePosition();

            Vector3 dir = currentMousePosition - startPoint;
            angle = Vector3.Angle(dir, transform.forward);

            Debug.DrawLine(startPoint, currentMousePosition);
            Debug.Log(angle);

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (angle > 75 && angle < 105)
            {
                //Position
                //Rot
                //Offset
                ChooseDirection(straightRoad, "right");
            }

            if(angle > 105)
            {
                //Position
                //Rot
                //Offset
                ChooseDirection(straightRoad, "down");

            }
            if (angle < 75 )
            {
                //Position
                //Rot
                //Offset
                ChooseDirection(straightRoad, "up");
            }

            lastPlacedObjectLocation = _get3dMousePosition();
            _isDraging = false;
            return;
        }
    }


    // Utility Functions
    void ChooseDirection(GameObject obj, string direction)
    {
        switch(direction)
        {
            case "right":
                turningLeft = true;
                turningRight = true;

                placementPosition.x += (GetSize(cornerRoad).x);
                newStart = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                startPoint = newStart;
                placePrefab(placementPosition, straightRoad, Quaternion.identity);

                break;

            case "left":
                turningLeft = true;
                turningRight = true;

                placementPosition.x -= (GetSize(cornerRoad).x);
                newStart = new Vector3(placementPosition.x - GetSize(obj).x, placementPosition.y, placementPosition.z);
                startPoint = newStart;
                placePrefab(placementPosition, straightRoad, Quaternion.identity);

                break;

            case "up":
                rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                if(turningLeft == true)
                {
                    placementPosition.x += (GetSize(cornerRoad).x);
                    newStart = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(obj).z);
                    startPoint = newStart;
                    placePrefab(placementPosition, cornerRoad, rot);
                    turningLeft = false;
                }
                else
                {
                    placementPosition.z += (GetSize(cornerRoad).z);
                    newStart = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(obj).z);
                    startPoint = newStart;
                    placePrefab(placementPosition, straightRoad, rot);
                }

                break;

            case "down":
                rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                if (turningRight == true)
                {
                    placementPosition.x += (GetSize(cornerRoad).x);
                    newStart = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(obj).z);
                    startPoint = newStart;
                    placePrefab(placementPosition, cornerRoad, Quaternion.identity);
                    turningRight = false;
                }
                else
                {
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    newStart = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(obj).z);
                    startPoint = newStart;
                    placePrefab(placementPosition, straightRoad, rot);
                }
                break;
        }
    }

    void placePrefab(Vector3 location, GameObject obj,Quaternion q)
    {
        rot = q;
        GameObject objectC = Instantiate(obj, transform);
        objectC.transform.localPosition = location;
        objectC.transform.localRotation = rot;
        prevObj = objectC;
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


    private Vector3 _get3dMousePosition()
    {
        Vector3 newPos = new Vector3();
        newPos = Input.mousePosition;
        newPos.z = (thisCamera.farClipPlane - 5) / 2;
        newPos = thisCamera.ScreenToWorldPoint(newPos);
        return newPos;
    }

    float GetMag(Vector3 start, Vector3 end)
    {
        float mag = Mathf.Sqrt(((start.x * end.x) + (start.y * start.y) + (start.z * end.z)));
        return mag;
    }

}
