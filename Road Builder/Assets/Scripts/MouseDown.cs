using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDown : MonoBehaviour
{
    public bool clickInside = false;
    public bool OnMouseDown()
    {
        Debug.Log("Clicked");
        clickInside = true;
        return clickInside;
    }

    public bool resetDown()
    {
        clickInside = true;
        return clickInside;
    }
}
