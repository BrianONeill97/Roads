using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeighBours : MonoBehaviour
{
    public int neighbourCount = 0;
    void OnCollisionEnter(Collision collision)
    {
        neighbourCount = neighbourCount + 1;
        Debug.Log("Hi");
    }
}
