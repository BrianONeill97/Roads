using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrudeShape : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2[] verts2D = new Vector2[] 
    {
        new Vector2(0,1),
        new Vector2(1,1),
        new Vector2(1,1),
        new Vector2(2,0),
        new Vector2(3,0),
        new Vector2(4,0)
    };
    public Vector2[] normal;
    public float[] us;

    public int[] lines = new int[]
    {
        0, 1,
        2, 3,
        3, 4,
        4, 5
    };

}


