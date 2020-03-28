using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseEvents : MonoBehaviour
{
    private Color start;
    public bool allowChange = true;
    public bool selected = false;

    private void Awake()
    {
        start = GetComponent<Renderer>().material.color;
    }

    private void OnMouseOver()
    {
        selected = true;
        if (allowChange)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }


    }

    private void OnMouseExit()
    {
        selected = false;
        if (allowChange)
        {
            GetComponent<Renderer>().material.color = start;
        }
    }

    public void revertToOrgColor()
    {
        GetComponent<Renderer>().material.color = start;
    }
}
