using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledRoadCreator : MonoBehaviour
{
    [Header ("Objects")]
    public GameObject straightRoad;
    public GameObject cornerRoad;
    public GameObject intersection;
    public GameObject TJunction;

    Camera thisCamera;
    private bool _isDraging = false;

    //Variables for Placement
    Quaternion rot; // rotation for tiles.
    Vector3 placementPosition; // position for placing the tiles.
    Vector3 currentMousePosition; // current mouse position on the screen.
    private Vector3 lastPlacedObjectLocation; // Where the last object was placed.
    float angle; // angle used for choosing the roads.

    bool started = false; // turns off the start.
    Vector3 startPoint; // startPoint of the creation.

    GameObject currObj; // reference to previous object.

    string prevDirection = ""; // previous direction moved in.

    List<GameObject> path = new List<GameObject>();

    LineRenderer lr;


    private void Awake()
    {
        thisCamera = Camera.main;
        lr = gameObject.GetComponent<LineRenderer>();
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
            lr.enabled = true;
            lr.positionCount = 2;
            lr.SetPosition(0, startPoint);

            _isDraging = true;
        }


        if (_isDraging)
        {
            //Gets the position of the mouse
            currentMousePosition = _get3dMousePosition();
            Vector3 dir = currentMousePosition - startPoint;
            angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;


            lr.SetPosition(1, currentMousePosition);
            //Debug.Log(angle);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (angle >= 65 && angle <= 115)
            {
                ChooseDirection(straightRoad, "right");
                prevDirection = "right";
            }

            if (angle > -115 && angle < -65)
            {
                ChooseDirection(straightRoad, "left");
                prevDirection = "left";
            }

            if (angle > 155 && angle > -155)
            {
                ChooseDirection(straightRoad, "down");
                prevDirection = "down";
            }
            if (angle < 25 && angle > -25)
            {
                ChooseDirection(straightRoad, "up");
                prevDirection = "up";
            }

            lastPlacedObjectLocation = _get3dMousePosition();
            _isDraging = false;
            lr.enabled = false;
            return;
        }

        if(Input.GetKey(KeyCode.Backspace))
        {
            started = false;
        }

        createIntersection();
        
    }


    // Utility Functions
    void ChooseDirection(GameObject obj, string direction)
    {
        switch(direction)
        {
            //Moving Right
            case "right":
                if(prevDirection == "up")
                {
                    rot = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                    placementPosition.z += (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                    prevDirection = "";
                } // if moving up this turns you right
                else if(prevDirection == "down")
                {
                    rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                    prevDirection = "";
                }// if moving down this turns you right
                else
                {
                    placementPosition.x += (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x + GetSize(straightRoad).x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, straightRoad, Quaternion.identity);
                } // otherwise just move right normally
                break;

            case "left":
                if(prevDirection == "up")
                {
                    rot = Quaternion.Euler(0.0f, 360.0f, 0.0f);
                    placementPosition.z += (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                    prevDirection = "";
                }// if moving up this turns you left
                else if(prevDirection == "down")
                {
                    rot = Quaternion.Euler(0.0f, 450.0f, 0.0f);
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                    prevDirection = "";
                }// if moving down this turns you left
                else
                {
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x - GetSize(straightRoad).x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, straightRoad, Quaternion.identity);
                }// otherwise just move left normally
                break;

            case "up":
                if(prevDirection == "right")
                {
                    rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    placementPosition.x += (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                } // if moving right this turns you upwards
                else if(prevDirection == "left")
                {
                    rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                } // if moving left this turns you upwards
                else
                {
                    rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    placementPosition.z += (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(straightRoad).z);
                    placePrefab(placementPosition, straightRoad, rot);
                } // move up normally 
                break;

            case "down":
                if(prevDirection == "right")
                {
                    rot = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    placementPosition.x += (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                } // if moving right this turns you downwards
                else if (prevDirection == "left")
                {
                    rot = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                } // if moving left this turns you downwards
                else
                {
                    rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(straightRoad).z);
                    placePrefab(placementPosition, straightRoad, rot);
                } // move down normally
                break;
        }
    }

    void placePrefab(Vector3 location, GameObject obj,Quaternion q)
    {
        rot = q;
        GameObject objectC = Instantiate(obj, transform);
        objectC.transform.localPosition = location;
        objectC.transform.localRotation = rot;
        path.Add(objectC);

        currObj = objectC;
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

    void createIntersection()
    {
        if(path.Count > 1)
        {
            for(int i = 0; i < path.Count - 1; i++)
            {
                if(path[i] != null)
                {
                    if (path[i].gameObject.tag == "Road")
                    {
                        if (path[i].transform.rotation != currObj.transform.rotation)
                        {
                            if (Mathf.Approximately(path[i].transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].transform.position.z, currObj.transform.position.z))
                            {
                                Destroy(path[i].gameObject);
                                path.RemoveAt(i);
                                Destroy(currObj);
                                placePrefab(placementPosition, intersection, Quaternion.identity);
                                path.Insert(i, currObj);
                                return;
                            }
                        }
                    }
                    else if (path[i].gameObject.tag == "Corner")
                    {
                            if (Mathf.Approximately(path[i].transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].transform.position.z, currObj.transform.position.z))
                            {
                                if (path[i].transform.rotation.eulerAngles.y == 90.0f || path[i].transform.rotation.eulerAngles.y == 0.0f)
                                {
                                    Destroy(path[i].gameObject);
                                    path.RemoveAt(i);
                                    Destroy(currObj);
                                    placePrefab(placementPosition, TJunction, Quaternion.identity);
                                    path.Insert(i, currObj);
                                    return;
                                }
                                else
                                {
                                Quaternion rotation = Quaternion.Euler(0, 180, 0);
                                Destroy(path[i].gameObject);
                                path.RemoveAt(i);
                                Destroy(currObj);
                                placePrefab(placementPosition, TJunction, rotation);
                                path.Insert(i, currObj);
                                return;
                            }
                            }
                        
                    }
                }
            }
        }
    }
}
