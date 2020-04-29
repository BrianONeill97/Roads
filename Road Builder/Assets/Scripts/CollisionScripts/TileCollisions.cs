using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollisions : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(gameObject.CompareTag("TJunction"))
        {
            if (Mathf.Approximately(gameObject.transform.position.x, other.gameObject.transform.position.x) && Mathf.Approximately(gameObject.transform.position.y, other.gameObject.transform.position.y) && Mathf.Approximately(gameObject.transform.position.z, other.gameObject.transform.position.z))
            {
                if (other.gameObject.CompareTag("Corner"))
                {
                    Destroy(other.gameObject);
                }
            }
        }

        //Blank Tile Collisions
        if (gameObject.tag == "Plain")
        {
            if (other.gameObject.tag == "Road")
            {
                Destroy(gameObject);
            }

            if (other.gameObject.tag == "Corner")
            {
                Destroy(gameObject);
            }

            if (other.gameObject.tag == "Intersection")
            {
                Destroy(gameObject);
            }

            if (other.gameObject.tag == "Wave")
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if(other.gameObject.CompareTag("Garden"))
            {
                Destroy(other.gameObject);
            }
        }


    }
}
