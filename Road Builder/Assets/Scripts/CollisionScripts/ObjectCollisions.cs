using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectCollisions : MonoBehaviour
{
    public int count;

    private void Start()
    {
        count = 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!other.gameObject.CompareTag(gameObject.tag))
        {
            count++;
        }
        
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

            if(other.gameObject.CompareTag("Wave"))
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

            if(other.gameObject.CompareTag("Rock"))
            {
                Destroy(other.gameObject);
            }

            if (other.gameObject.CompareTag("Tree"))
            {
                Destroy(other.gameObject);
            }
            //if (!other.gameObject.CompareTag("Road"))
            //{
            //    if (!other.gameObject.CompareTag("Ramp"))
            //    {
            //        Destroy(other.gameObject);
            //    }
            //    else
            //    {
            //        return;
            //    }            
            //}
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

        //Fences
        if(gameObject.CompareTag("Fence"))
        {
            if(!other.gameObject.CompareTag("Plain") && !other.gameObject.CompareTag("Fence") && !other.gameObject.CompareTag("Wall") && !other.gameObject.CompareTag("Floor"))
            {
                Destroy(gameObject.transform.parent.gameObject);
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

    private void OnCollisionExit(Collision collision)
    {
        count--;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Road Drawing")
        {
            if (!gameObject.CompareTag("Wave"))
            {
                if (count <= 0)
                {
                    Destroy(gameObject);
                    count = 0;
                }
            }
        }
    }
}
