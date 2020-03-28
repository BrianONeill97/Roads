using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCont : MonoBehaviour
{

    int distance = 5;
    public int m_speed = 5;
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

        transform.position = transform.position + myCam.transform.forward * distance * Time.deltaTime * v * m_speed;

        transform.position = transform.position + myCam.transform.right * distance * Time.deltaTime * h * m_speed;

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + myCam.transform.up.y * distance * Time.deltaTime * m_speed, transform.position.z);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - myCam.transform.up.y * distance * Time.deltaTime * m_speed, transform.position.z);
        }
    }
}
