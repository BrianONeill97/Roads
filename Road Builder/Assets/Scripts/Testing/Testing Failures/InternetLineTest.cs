using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetLineTest : MonoBehaviour
{
    public GameObject Obj;

    Camera thisCamera;

    private bool _isDraging = false;

    Quaternion rot;

    Vector3 placementPosition;
    Vector3 currentMousePosition;

    //we are going to keep track of the location where we last placed an object.
    private Vector3 _lastPlacedObjectLocation;

    private Vector3 _get3dMousePosition()
    {
        Vector3 newPos = new Vector3();
        newPos = Input.mousePosition;
        newPos.z = (thisCamera.farClipPlane - 5) / 2;

        newPos = thisCamera.ScreenToWorldPoint(newPos);
        return newPos;
    }

    private void Awake()
    {
        thisCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDraging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDraging = false;

            //keep 'old' mouse position up to date, so we do not draw in between dragging the mouse and releasing it. 
            _lastPlacedObjectLocation = _get3dMousePosition();
            return;
        }

        if (_isDraging)
        {
            currentMousePosition = _get3dMousePosition();

            //now, we find how far the mouse has moved since the last step.
            Vector3 distanceTravelled = _lastPlacedObjectLocation - currentMousePosition;

            //Rot of line
            Vector3 distanceV = currentMousePosition - _lastPlacedObjectLocation;


            //since we know the starting and ending point, all we have to do now is add the prefab instances, neatly spaced over the travelled distance.
            Vector3 stepDistance = distanceTravelled / 1;

            //then, we interpolate between the start and end vector
            //you could make this a lot faster by determining how often your object fits in this space, but i'd like to leave you a little bit of a challenge, so i'm abusing the magnitude of the stepDistance ;)
                for (float i = 0; i < stepDistance.magnitude; i += GetSize(Obj).x)
                {
                    float progress = i / stepDistance.magnitude;
                    placementPosition = Vector3.Lerp(_lastPlacedObjectLocation, currentMousePosition, progress);
                    placePrefab(placementPosition, distanceV);
                }


            //and then we update the last-placed-object location
            _lastPlacedObjectLocation = _get3dMousePosition();
        }

        Debug.Log(GetSize(Obj).z);
    }

    void placePrefab(Vector3 location, Vector3 direction)
    {
            rot = Quaternion.FromToRotation(Vector3.right, direction);
            GameObject newObj = Instantiate(Obj, transform);
            newObj.transform.localPosition = location;
            newObj.transform.localRotation = rot;


        
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
