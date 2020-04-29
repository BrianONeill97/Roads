using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEnable : MonoBehaviour
{
    GameObject mainLight;

    private void Start()
    {
        mainLight = GameObject.Find("MainLight").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(mainLight != null)
        {       
            if (mainLight.gameObject.activeInHierarchy)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
