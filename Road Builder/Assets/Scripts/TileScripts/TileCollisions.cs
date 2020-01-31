using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollisions : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
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

            if(other.gameObject.tag =="Wave")
            {
                Destroy(gameObject);
            }
        }
    }
}
