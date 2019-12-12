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
    public GameObject grassTile;

    Camera thisCamera;
    private bool _isDraging = false; // Bool for if im dragging

    //Variables for Placement
    Quaternion rot; // rotation for tiles.
    Vector3 placementPosition; // position for placing the tiles.
    Vector3 currentMousePosition; // current mouse position on the screen.
    float selectorAngle; // angle used for choosing the roads.

    bool started = false; // turns off the start.
    Vector3 startPoint; // startPoint of the creation.

    GameObject currObj; // reference to previous object.

    string prevDirection = ""; // previous direction moved in.

    List<GameObject> path = new List<GameObject>(); // List of all the road tiles in the path.

    LineRenderer lr; // line renderer.

    //int floodCount = 0;

    private GameObject[][] ground;
    public int ColumnLength;
    public int RowHeight;


    private void Awake()
    {
        thisCamera = Camera.main;
        lr = gameObject.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        createGrid();
    }

    void Update()
    {
        thisCamera.transform.position = new Vector3(placementPosition.x, thisCamera.transform.position.y, placementPosition.z);
        //Lets you start the road again at a new place.
        //if (Input.GetKey(KeyCode.Backspace))
        //{
        //    started = false;
        //}

        ////Fill in the area that the mouse is in.
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    floodCount = 0;
        //    FloodFill(new Vector3(_get3dMousePosition().x,placementPosition.y,_get3dMousePosition().z)); // TODO:Should work automaticaaly next
        //}

        //Create the roads with the mouse.
        if (Input.GetMouseButtonDown(0))
        {         
            if (started == false)//After the first tile of the road has been placed
            {
                //Creates first tile,Sets the directional tools start point
                startPoint = _get3dMousePosition();
                placePrefab(startPoint,straightRoad,Quaternion.identity);
                placementPosition = startPoint;
                started = true;
            }
            // shows the tool when held 
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
            selectorAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;


            lr.SetPosition(1, currentMousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (selectorAngle >= 65 && selectorAngle <= 115)
            {
                ChooseDirection(straightRoad, "right");
                prevDirection = "right";
            }

            if (selectorAngle > -115 && selectorAngle < -65)
            {
                ChooseDirection(straightRoad, "left");
                prevDirection = "left";
            }

            if (selectorAngle > 155 && selectorAngle > -155)
            {
                ChooseDirection(straightRoad, "down");
                prevDirection = "down";
            }
            if (selectorAngle < 25 && selectorAngle > -25)
            {
                ChooseDirection(straightRoad, "up");
                prevDirection = "up";
            }

            _isDraging = false;
            lr.enabled = false;
            return;
        }

        createIntersection();
        tJunction();
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
                    startPoint = new Vector3(placementPosition.x + GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                    prevDirection = "";
                } // if moving up this turns you right
                else if(prevDirection == "down")
                {
                    rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x + GetSize(straightRoad).x, placementPosition.y, placementPosition.z);
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
                    startPoint = new Vector3(placementPosition.x - GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    placePrefab(placementPosition, cornerRoad, rot);
                    prevDirection = "";
                }// if moving up this turns you left
                else if(prevDirection == "down")
                {
                    rot = Quaternion.Euler(0.0f, 450.0f, 0.0f);
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x - GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
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
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(straightRoad).z);
                    placePrefab(placementPosition, cornerRoad, rot);
                } // if moving right this turns you upwards
                else if(prevDirection == "left")
                {
                    rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(cornerRoad).z);
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
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(straightRoad).z);
                    placePrefab(placementPosition, cornerRoad, rot);
                } // if moving right this turns you downwards
                else if (prevDirection == "left")
                {
                    rot = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(straightRoad).z);
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
            for(int i = 0; i < path.Count -1; i++)
            {
                if(path[i] != null)
                {
                    if (path[i].gameObject.tag == "Road")
                    {
                        if (path[i].transform.rotation != currObj.transform.rotation) // iF the rotation of the road tile that im hitting is not the same then add intersection
                        {
                            if (Mathf.Approximately(path[i].transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].transform.position.z, currObj.transform.position.z))
                            {
                                    replaceObject(path[i], intersection, Quaternion.identity, i);
                                    return;
                            }
                        }
                    }

                    if(path[i].gameObject.tag == "Intersection")
                    {
                            if (Mathf.Approximately(path[i].transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].transform.position.z, currObj.transform.position.z))
                            {
                                replaceObject(path[i], intersection, Quaternion.identity, i);
                                return;
                            }
                    }
                }
            }
        }
    }

    void tJunction()
    {
        if (path.Count > 1)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path[i] != null)
                {
                    if (path[i].gameObject.tag == "Corner") // Replaces with a T-Junction
                    {
                        if (Mathf.Approximately(path[i].transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].transform.position.z, currObj.transform.position.z))
                        {
                            if (path[i].transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 0.0f  || path[i].transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 90.0f)
                            {
                                replaceObject(path[i], TJunction, Quaternion.identity, i);
                                return;
                            }

                            if (path[i].transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 0.0f || path[i].transform.rotation.eulerAngles.y == 180.0f && currObj.transform.rotation.eulerAngles.y == 0.0f)
                            {
                                replaceObject(path[i], TJunction, Quaternion.Euler(0.0f, 90.0f, 0.0f), i);
                                return;
                            }

                            if (path[i].transform.rotation.eulerAngles.y == 0.0f && currObj.transform.rotation.eulerAngles.y == 0.0f || path[i].transform.rotation.eulerAngles.y == 270.0f && currObj.transform.rotation.eulerAngles.y == 0.0f)
                            {
                                replaceObject(path[i], TJunction, Quaternion.Euler(0.0f, 270.0f, 0.0f), i);
                                return;
                            }

                            if (path[i].transform.rotation.eulerAngles.y == 180.0f && currObj.transform.rotation.eulerAngles.y == 90.0f || path[i].transform.rotation.eulerAngles.y == 270.0f && currObj.transform.rotation.eulerAngles.y == 90.0f)
                            {
                                replaceObject(path[i], TJunction, Quaternion.Euler(0.0f, 180.0f, 0.0f), i);
                                return;
                            }
                        }

                    }
                }
            }
        }
    }

    void replaceObject(GameObject obectbeingReplaced, GameObject objectReplacing, Quaternion rot, int placeInList)
    {
        Quaternion rotation = rot;
        Destroy(obectbeingReplaced);
        path.RemoveAt(placeInList);
        Destroy(currObj);
        placePrefab(placementPosition, objectReplacing, rot);
        path.Insert(placeInList, currObj);
    }

    //void FloodFill(Vector3 pos)
    //{
    //    floodCount++;
    //    Collider[] intersecting = Physics.OverlapSphere(pos, GetSize(grassTile).x * 0.5f);
    //    if (intersecting.Length != 0)
    //    {
    //        return;
    //    }

    //    if(floodCount > 60)
    //    {
    //        return;
    //    }


    //    placePrefab(pos, grassTile, Quaternion.identity);

    //    FloodFill(new Vector3(pos.x, pos.y, pos.z - GetSize(grassTile).z));
    //    FloodFill(new Vector3(pos.x + GetSize(grassTile).x, pos.y, pos.z));
    //    FloodFill(new Vector3(pos.x, pos.y, pos.z + GetSize(grassTile).z));
    //   // FloodFill(new Vector3(pos.x - GetSize(grassTile).x, pos.y, pos.z));
            
        
    //    return;
    //}

    void createGrid()
    {
        for(int i = 0; i < ColumnLength; i++)
        {
            for(int j = 0; j < RowHeight; j++)
            {
                Instantiate(grassTile, new Vector3(GetSize(grassTile).x * i, 77.5f, GetSize(grassTile).z * j), Quaternion.identity);
            }
        }
    }
}
