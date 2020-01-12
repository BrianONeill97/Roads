using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TiledRoadCreator : MonoBehaviour
{
    [Header("Objects")]
    GameObject straightRoad;
    GameObject cornerRoad;
    GameObject intersection;
    GameObject TJunction; 
    GameObject grassTile; 
    GameObject ramp;
    GameObject roadLamp;
    GameObject bench;
    GameObject bin;

    [Header("Options")]
    public GameObject dropDown;
    public GameObject Canvas;

    public bool Clutter = false;
    bool allowCreation = true;

    //Track to be saved
    private GameObject track;


    GameObject currObj; // reference to previous object.
    List<GameObject> path = new List<GameObject>(); // List of all the road tiles in the path.
    List<GameObject> roadSideObjects = new List<GameObject>();

    LineRenderer lr; // line renderer
    Camera thisCamera;

    private bool _isDraging = false; // Bool for if im dragging
    bool started = false; // turns off the start.

    //Variables for Placement
    Quaternion rot;

    Vector3 placementPosition; // position for placing the tiles.
    Vector3 startPoint; // startPoint of the creation.

    float selectorAngle; // angle used for choosing the roads.
    string prevDirection = ""; // previous direction moved in.
    static int trackNumber = 0;


    private void Awake()
    {
        //straightRoad = Resources.Load("City/Road") as GameObject;
        //cornerRoad = Resources.Load("City/corner") as GameObject;
        //intersection = Resources.Load("City/Intersection") as GameObject;
        //TJunction = Resources.Load("City/T-Junction") as GameObject;
        //grassTile = Resources.Load("Plain") as GameObject;
        //ramp = Resources.Load("City/Ramp") as GameObject;

        thisCamera = Camera.main;
        lr = gameObject.GetComponent<LineRenderer>();
        track = new GameObject("Track" + trackNumber);
        track.gameObject.tag = "Track";
    }

    void Update()
    {

        foreach(GameObject temp in GameObject.FindGameObjectsWithTag("Lamp"))
        {
            roadSideObjects.Add(temp);
        }

        changeTileType();
        createIntersection();
        tJunction();
 

        thisCamera.transform.position = new Vector3(placementPosition.x, thisCamera.transform.position.y, placementPosition.z);
        //KEYBOARD PRESSES
        //ALTER Y OF THE ROADS
        if (Input.GetKeyUp(KeyCode.UpArrow))
                {
                    RampUp();
                }

        if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    RampDown();
                }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //MOUSE PRESSES
            //Create the roads with the mouse.
            if (Input.GetMouseButtonDown(0))
            {
                if (started == false)//After the first tile of the road has been placed
                {
                    //Creates first tile,Sets the directional tools start point
                    startPoint = _get3dMousePosition();
                    createTile(startPoint, straightRoad, Quaternion.identity, straightRoad.gameObject.tag);
                    placementPosition = startPoint;
                    started = true;
                }
                // shows the tool when held 
                lr.enabled = true;
                lr.positionCount = 2;
                lr.SetPosition(0, startPoint);

                _isDraging = true;
            }
            //Changes the line rotation.
            if (_isDraging)
            {
                //Gets the position of the mouse
                Vector3 dir = _get3dMousePosition() - startPoint;
                selectorAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                lr.SetPosition(1, _get3dMousePosition());
            }
            //Gets the angle of the line and then selects the road from that.
            if (Input.GetMouseButtonUp(0))
            {
                if (selectorAngle >= 65 && selectorAngle <= 115)
                {
                    ChooseDirectionRoads(straightRoad, "right");
                    prevDirection = "right";
                }
                if (selectorAngle > -115 && selectorAngle < -65)
                {
                    ChooseDirectionRoads(straightRoad, "left");
                    prevDirection = "left";
                }
                if (selectorAngle > 155 && selectorAngle > -155)
                {
                    ChooseDirectionRoads(straightRoad, "down");
                    prevDirection = "down";
                }
                if (selectorAngle < 25 && selectorAngle > -25)
                {
                    ChooseDirectionRoads(straightRoad, "up");
                    prevDirection = "up";
                }

                _isDraging = false;
                lr.enabled = false;

                if (Clutter == true)
                {
                    if (allowCreation == true)
                    {
                        createClutter(prevDirection);
                    }
                }
            }
        }
    }

    //Create the roads
    void ChooseDirectionRoads(GameObject obj, string direction)
    {
        switch(direction)
        {
            //Moving Right
            case "right":
                if(prevDirection == "up")
                {
                    placementPosition.z += (GetSize(cornerRoad).z); // moves placement for road
                    startPoint = new Vector3(placementPosition.x + GetSize(cornerRoad).x, placementPosition.y, placementPosition.z); // moves startpoint
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 270.0f, 0.0f),cornerRoad.gameObject.tag);
                    prevDirection = "";
                } // if moving up this turns you right
                else if(prevDirection == "down")
                {
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x + GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 180.0f, 0.0f),cornerRoad.gameObject.tag);
                    prevDirection = "";
                }// if moving down this turns you right
                else
                {
                    placementPosition.x += (GetSize(obj).x);
                    startPoint = new Vector3(placementPosition.x + GetSize(obj).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, obj, Quaternion.identity,obj.gameObject.tag);
                } // otherwise just move right normally
                break;

            case "left":
                if(prevDirection == "up")
                {
                    placementPosition.z += (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x - GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, cornerRoad, Quaternion.identity,cornerRoad.gameObject.tag);
                    prevDirection = "";
                }// if moving up this turns you left
                else if(prevDirection == "down")
                {
                    placementPosition.z -= (GetSize(cornerRoad).z);
                    startPoint = new Vector3(placementPosition.x - GetSize(cornerRoad).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 90.0f, 0.0f),cornerRoad.gameObject.tag);
                    prevDirection = "";
                }// if moving down this turns you left
                else
                {
                    placementPosition.x -= (GetSize(obj).x);
                    startPoint = new Vector3(placementPosition.x - GetSize(obj).x, placementPosition.y, placementPosition.z);
                    createTile(placementPosition, obj, Quaternion.identity,obj.gameObject.tag);
                }// otherwise just move left normally
                break;

            case "up":
                if(prevDirection == "right")
                {
                    placementPosition.x += (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(straightRoad).z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 90.0f, 0.0f),cornerRoad.gameObject.tag);
                } // if moving right this turns you upwards
                else if(prevDirection == "left")
                {
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(cornerRoad).z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 180.0f, 0.0f),cornerRoad.gameObject.tag);
                } // if moving left this turns you upwards
                else
                {
                    placementPosition.z += (GetSize(obj).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(obj).z);
                    createTile(placementPosition, obj, Quaternion.Euler(0.0f, 90.0f, 0.0f),obj.gameObject.tag);
                } // move up normally 
                break;

            case "down":
                if(prevDirection == "right")
                {
                    placementPosition.x += (GetSize(obj).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(obj).z);
                    createTile(placementPosition, cornerRoad, Quaternion.identity,cornerRoad.gameObject.tag);
                } // if moving right this turns you downwards
                else if (prevDirection == "left")
                {
                    placementPosition.x -= (GetSize(cornerRoad).x);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(straightRoad).z);
                    createTile(placementPosition, cornerRoad, Quaternion.Euler(0.0f, 270.0f, 0.0f),cornerRoad.gameObject.tag);
                } // if moving left this turns you downwards
                else
                {
                    placementPosition.z -= (GetSize(obj).z);
                    startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(obj).z);
                    createTile(placementPosition, obj, Quaternion.Euler(0.0f, 90.0f, 0.0f),obj.gameObject.tag);
                } // move down normally
                break;
        }
    }

    void createIntersection()
    {

        if (path.Count > 1)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (path[i] != null)
                {
                    if (Mathf.Approximately(path[i].gameObject.transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].gameObject.transform.position.y, currObj.transform.position.y) && Mathf.Approximately(path[i].gameObject.transform.position.z, currObj.transform.position.z))
                    {
                        //Creates Intersection when tow road hit from the side
                        if (path[i].gameObject.gameObject.tag == "Road")
                        {
                            if (path[i].gameObject.transform.rotation != currObj.transform.rotation) // iF the rotation of the road tile that im hitting is not the same then add intersection
                            {

                                replaceObject(path[i].gameObject, intersection, Quaternion.identity, i);
                                return;

                            }
                        }
                        else if (path[i].gameObject.gameObject.tag == "TJunction")                         // Creates intersection from an intersection
                        {
                            if (currObj.gameObject.tag == "Road")
                            {
                                if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation == Quaternion.identity ||
                                    path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 180.0f ||
                                    path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 90.0f ||
                                    path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 270.0f)
                                {
                                    replaceObject(path[i].gameObject, intersection, Quaternion.identity, i);
                                    return;
                                }
                            }
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
                    if (Mathf.Approximately(path[i].gameObject.transform.position.x, currObj.transform.position.x) && Mathf.Approximately(path[i].gameObject.transform.position.y, currObj.transform.position.y) && Mathf.Approximately(path[i].gameObject.transform.position.z, currObj.transform.position.z))
                    {
                        //Corner Checks
                        if (path[i].gameObject.gameObject.tag == "Corner") // Replaces with a T-Junction
                        {
                            if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation == Quaternion.identity || path[i].gameObject.transform.rotation.eulerAngles.y == 180.0f && currObj.transform.rotation == Quaternion.identity)
                            {
                                replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 90.0f, 0.0f), i);
                                return;
                            }

                            if (path[i].gameObject.transform.rotation.eulerAngles.y == 90.0f && currObj.transform.rotation.eulerAngles.y == 90.0f || path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation.eulerAngles.y == 90.0f)
                            {
                                replaceObject(path[i].gameObject, TJunction, Quaternion.identity, i);
                                return;
                            }

                            if (path[i].gameObject.transform.rotation == Quaternion.identity && currObj.transform.rotation == Quaternion.identity || path[i].gameObject.transform.rotation.eulerAngles.y == 270.0f && currObj.transform.rotation == Quaternion.identity)
                            {
                                replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 270.0f, 0.0f), i);
                                return;
                            }

                            if (path[i].gameObject.transform.rotation.eulerAngles.y == 180.0f && currObj.transform.rotation.eulerAngles.y == 90.0f || path[i].gameObject.transform.rotation.eulerAngles.y == 270.0f && currObj.transform.rotation.eulerAngles.y == 90.0f)
                            {
                                replaceObject(path[i].gameObject, TJunction, Quaternion.Euler(0.0f, 180.0f, 0.0f), i);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }


    // __    __.  ___________. __   __       __  .___________.____    ____     _______  __    __. __    __.   ______. ___________. __    ______   .__   __.      _______.
    //|  |  |  | |           ||  | |  |     |  | |           |\   \  /   /    |   ____||  |  |  | |  \ |  |  /      ||           ||  |  /  __  \  |  \ |  |     /       |
    //|  |  |  | `---|  |----`|  | |  |     |  | `---|  |----` \   \/   /     |  |__   |  |  |  | |   \|  | |  ,----'`---|  |----`|  | |  |  |  | |   \|  |    |   (----`
    //|  |  |  |     |  |     |  | |  |     |  |     |  |       \_ _/      |   __|  |  |  |  | |  . `  | |  |         |  |     |  | |  |  |  | |  . `  |     \   \    
    //|  `--'  |     |  |     |  | |  `----.|  |     |  |         |  |        |  |     |  `--'  | |  |\   | |  `----.    |  |     |  | |  `--'  | |  |\   | .----)   |   
    // \______/      |__|     |__| |_______||__|     |__|         |__|        |__|      \______/  |__| \__|  \______|    |__|     |__|  \______/  |__| \__| |_______/  

    // Used to create a tile
    void createTile(Vector3 location, GameObject obj,Quaternion q,string tileTag)
    {
        Collider[] hit = Physics.OverlapSphere(location, 0);
        if (hit.Length > 0 )
        {
            if(hit[0].gameObject.tag == "Intersection")
            {
                tileTag = "";
                return;
            }

            if (hit[0].gameObject.tag == "Corner")
            {
                rot = q;
                GameObject objectC = Instantiate(obj, track.transform);
                objectC.transform.localPosition = location;
                objectC.transform.localRotation = q;
                if (tileTag != "Lamp")
                {
                    path.Add(objectC);
                    currObj = objectC;
                    tileTag = currObj.gameObject.tag;
                }
                return;
            }

            if (tileTag == "Road")
            {
                if(hit[0].transform.localRotation == q)
                { 
                    tileTag = "";
                    return;
                }
                else
                {
                    rot = q;
                    GameObject objectC = Instantiate(obj, track.transform);
                    objectC.transform.localPosition = location;
                    objectC.transform.localRotation = q;
                    if (tileTag != "Lamp")
                    {
                        path.Add(objectC);
                        currObj = objectC;
                        tileTag = currObj.gameObject.tag;
                    }
                }
            }
            else
            {
                rot = q;
                GameObject objectC = Instantiate(obj, track.transform);
                objectC.transform.localPosition = location;
                objectC.transform.localRotation = q;
                if (tileTag != "Lamp")
                {
                    path.Add(objectC);
                    currObj = objectC;
                    tileTag = currObj.gameObject.tag;
                }
            }
        }
        else
        {
            rot = q;
            GameObject objectC = Instantiate(obj, track.transform);
            objectC.transform.localPosition = location;
            objectC.transform.localRotation = q;
            if(tileTag != "Lamp")
            {
                path.Add(objectC);
                currObj = objectC;
                tileTag = currObj.gameObject.tag;
            }
        }
    } // Creates a tile 

    // Gets the Actual size
    Vector3 GetSize(GameObject obj)
    {
        Vector3 dimensions;
        float width = obj.GetComponent<Renderer>().bounds.size.x * transform.localScale.x;
        float height = obj.GetComponent<Renderer>().bounds.size.y * transform.localScale.y;
        float depth = obj.GetComponent<Renderer>().bounds.size.z * transform.localScale.z;
        dimensions = new Vector3(width, height, depth);
        return dimensions;
    } // Gets the ACTUAL size of the tile

    //Gets the mouse position in the game
    private Vector3 _get3dMousePosition()
    {
        Vector3 newPos = new Vector3();
        newPos = Input.mousePosition;
        newPos.z = (thisCamera.farClipPlane - 5) / 2;
        newPos = thisCamera.ScreenToWorldPoint(newPos);
        return newPos;
    } // Gets the mouse position 

    //Gets magnitude
    float GetMag(Vector3 start, Vector3 end)
    {
        float mag = Mathf.Sqrt(((start.x * end.x) + (start.y * start.y) + (start.z * end.z)));
        return mag;
    } // Gets the magnitude of the line Vector

    //Replaces the object with a new objject
    void replaceObject(GameObject obectbeingReplaced, GameObject objectReplacing, Quaternion rot, int placeInList)
    {
        Quaternion rotation = rot;
        Destroy(obectbeingReplaced);
        path.RemoveAt(placeInList);
        Destroy(currObj);
        createTile(placementPosition, objectReplacing, rot,objectReplacing.gameObject.tag);
        path.Insert(placeInList, currObj);
        if (Clutter == true)
        {
            removeClutter();
        }
    } // REplace the game object in the list with a new one

    void emptyPath()
    {
        for(int i = 0; i < path.Count; i++)
        {
            path.RemoveAt(i);
        }
        Destroy(track);
        trackNumber++;
    } // Empty the path for the new track

    public void createNewPath()
    {
        emptyPath();
        Destroy(currObj);
        //started = false;
        track = new GameObject("Track" + trackNumber);
    }

    void changeTileType()
    {
        if (dropDown.GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text == "Select Road Type")
        {
            Canvas.transform.GetChild(0).gameObject.GetComponent<Text>().enabled = true;
        }

        if (dropDown.GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text == "City")
        {
                Canvas.transform.GetChild(0).gameObject.GetComponent<Text>().enabled = false;
                straightRoad = Resources.Load("City/Road") as GameObject;
                cornerRoad = Resources.Load("City/corner") as GameObject;
                intersection = Resources.Load("City/Intersection") as GameObject;
                TJunction = Resources.Load("City/T-Junction") as GameObject;
                grassTile = Resources.Load("Plain") as GameObject;
                ramp = Resources.Load("City/Ramp") as GameObject;
                roadLamp = Resources.Load("StreetLamp") as GameObject;
                bench = Resources.Load("bench") as GameObject;
                bin = Resources.Load("bin") as GameObject;
        }
        if (dropDown.GetComponent<Dropdown>().options[dropDown.GetComponent<Dropdown>().value].text == "Country")
        {
            Canvas.transform.GetChild(0).gameObject.GetComponent<Text>().enabled = false;
            straightRoad = Resources.Load("Country/Road") as GameObject;
            cornerRoad = Resources.Load("Country/corner") as GameObject;
            intersection = Resources.Load("Country/Intersection") as GameObject;
            TJunction = Resources.Load("Country/T-Junction") as GameObject;
            grassTile = Resources.Load("Plain") as GameObject;
            ramp = Resources.Load("City/Ramp") as GameObject;
            roadLamp = Resources.Load("StreetLamp") as GameObject;
            bench = Resources.Load("bench") as GameObject;
            bin = Resources.Load("bin") as GameObject;

        }
    }

    void createClutter(string dir)
    {
        if(path.Count > 0)
        {
            if((path.Count % 2) != 0)
            {
                if (path[path.Count - 1].gameObject.tag == "Road")
                {
                    if (prevDirection == "left" || prevDirection == "right")
                    {
                        createTile(new Vector3(path[path.Count - 1].transform.position.x, path[path.Count - 1].transform.position.y, path[path.Count - 1].transform.position.z - GetSize(path[path.Count - 1].gameObject).z / 3.5f), roadLamp, path[path.Count - 1].transform.rotation * roadLamp.transform.rotation, roadLamp.gameObject.tag);
                        return;
                    }
                    else if (prevDirection == "up" || prevDirection == "down")
                    {
                        createTile(new Vector3(path[path.Count - 1].transform.position.x - GetSize(path[path.Count - 1].gameObject).z / 3.5f, path[path.Count - 1].transform.position.y, path[path.Count - 1].transform.position.z), roadLamp, path[path.Count - 1].transform.rotation * roadLamp.transform.rotation, roadLamp.gameObject.tag);
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    void removeClutter()
    {
            //Collider[] hit = Physics.OverlapSphere(placementPosition, 2.9f);
            //if (hit.Length > 0)
            //{
            //    for (int i = 0; i < hit.Length; i++)
            //    {
            //            if (hit[i].gameObject.tag == "Lamp")
            //            {
            //                if (roadSideObjects.Count > 0)
            //                {
            //                    if (hit[i].transform.position != roadSideObjects[roadSideObjects.Count].transform.position)
            //                    {
            //                        Debug.Log("hit");
            //                        //Destroy(hit[0]);
            //                    }
            //                }
            //            }
            //    }
            //}
    }

    //Testing functions (NOT PERMANANT)
    void RampUp()
    {
        if (prevDirection == "right")
        {
            placementPosition.x += (GetSize(ramp).x);
            startPoint = new Vector3(placementPosition.x + GetSize(ramp).x, placementPosition.y, placementPosition.z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y / 5.5f);
            createTile(placementPosition, ramp, Quaternion.identity,ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + (GetSize(ramp).y - GetSize(straightRoad).y / 2);
        }

        if (prevDirection == "left")
        {
            placementPosition.x -= (GetSize(ramp).x);
            startPoint = new Vector3(placementPosition.x - GetSize(ramp).x, placementPosition.y, placementPosition.z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y / 5.5f);
            createTile(placementPosition, ramp, Quaternion.Euler(0, 180, 0), ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + (GetSize(ramp).y - GetSize(straightRoad).y / 2);
        }

        if (prevDirection == "up")
        {
            placementPosition.z += (GetSize(ramp).z);
            startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(ramp).z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y / 5.5f);
            createTile(placementPosition, ramp, Quaternion.Euler(0, 270, 0), ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + (GetSize(ramp).y - GetSize(straightRoad).y / 2);
        }

        if (prevDirection == "down")
        {
            placementPosition.z -= (GetSize(ramp).z);
            startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(ramp).z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y / 5.5f);
            createTile(placementPosition, ramp, Quaternion.Euler(0, 90, 0), ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + (GetSize(ramp).y - GetSize(straightRoad).y / 2);
        }
    }

    void RampDown()
    {
        if (prevDirection == "right")
        {
            placementPosition.x += (GetSize(ramp).x);
            startPoint = new Vector3(placementPosition.x + GetSize(ramp).x, placementPosition.y, placementPosition.z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y * 0.82f);
            createTile(placementPosition, ramp, Quaternion.Euler(0, 180, 0), ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + GetSize(straightRoad).y / 2;
        }

        if (prevDirection == "left")
        {
            placementPosition.x -= (GetSize(ramp).x);
            startPoint = new Vector3(placementPosition.x - GetSize(ramp).x, placementPosition.y, placementPosition.z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y * 0.82f);
            createTile(placementPosition, ramp, Quaternion.identity, ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + GetSize(straightRoad).y / 2;
        }

        if (prevDirection == "up")
        {
            placementPosition.z += (GetSize(ramp).z);
            startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z + GetSize(ramp).z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y * 0.82f);
            createTile(placementPosition, ramp, Quaternion.Euler(0, 90, 0),ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + GetSize(straightRoad).y / 2;
        }

        if (prevDirection == "down")
        {
            placementPosition.z -= (GetSize(ramp).z);
            startPoint = new Vector3(placementPosition.x, placementPosition.y, placementPosition.z - GetSize(ramp).z);
            placementPosition.y = placementPosition.y - (GetSize(ramp).y * 0.82f);
            createTile(placementPosition, ramp, Quaternion.Euler(0, 270, 0), ramp.gameObject.tag);
            placementPosition.y = placementPosition.y + GetSize(straightRoad).y / 2;
        }
    }

}
////////
//TODO:
//
////////