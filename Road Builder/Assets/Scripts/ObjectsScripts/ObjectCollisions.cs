using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollisions : MonoBehaviour
{
    public Color m_originalColor;
    private void Awake()
    {
        m_originalColor = GetComponent<Renderer>().material.color;
    }
    private void OnCollisionEnter(Collision other)
    {
        //TREES
        if (gameObject.CompareTag("Tree"))
        {
            if (other.gameObject.CompareTag("House"))
            {
                Destroy(gameObject);
            }
        
            if (other.gameObject.CompareTag("Road"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Corner"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Intersection"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("TJunction"))
            {
                Destroy(gameObject);
            }
        }
        //HOUSE
        if(gameObject.CompareTag("House"))
        {
            if(other.gameObject.CompareTag("House"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Road"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Corner"))
            {
                Destroy(gameObject);
            }
        }
        //WATER
        if (gameObject.CompareTag("Wave"))
        {
            if (other.gameObject.CompareTag("Wave"))
            {
                if (Mathf.Approximately(other.gameObject.transform.position.x, transform.position.x) && Mathf.Approximately(other.gameObject.transform.position.y, transform.position.y) && Mathf.Approximately(other.gameObject.transform.position.z, transform.position.z))
                {
                    Destroy(other.gameObject);
                }
                else
                {
                    return;
                }
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
        //ROCKS
        if (gameObject.CompareTag("Rock"))
        {
            if(other.gameObject.CompareTag("Tree"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("House"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Road"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Corner"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("Intersection"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag("TJunction"))
            {
                Destroy(gameObject);
            }
        }


            if (other.gameObject.CompareTag("TJunction"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Intersection"))
        {
            Destroy(gameObject);
        }
    }
}
