using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCont : MonoBehaviour
{

    int distance = 5;

    public Camera myCam;

    float h;
    float v;

    void Start()
    {

    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        transform.position = transform.position + myCam.transform.forward * distance * Time.deltaTime * v;

        transform.position = transform.position + myCam.transform.right * distance * Time.deltaTime * h;
    }
}
