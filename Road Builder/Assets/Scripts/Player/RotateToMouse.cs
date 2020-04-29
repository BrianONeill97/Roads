using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    public Camera myCamera;
    private Vector3 MousePositionViewport = Vector3.zero;
    // Update is called once per frame
    private void Update()
    {
        float X = Input.GetAxis("Mouse X") * 2;
        float Y = Input.GetAxis("Mouse Y") * 2;

        transform.Rotate(0, X, 0); // Player rotates on Y axis, your Cam is child, then rotates too


        // To scurity check to not rotate 360º 
        if (myCamera.transform.eulerAngles.x + (-Y) > 80 && myCamera.transform.eulerAngles.x + (-Y) < 280)
        { }
        else
        {

            myCamera.transform.RotateAround(transform.position, myCamera.transform.right, -Y);
        }
    }
}
