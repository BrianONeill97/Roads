using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class TransformLS
{
    public Vector3 position;
    public Quaternion rotation;
}

public class Lsystem : MonoBehaviour
{
    [Header("Objects Used")]
    public GameObject obj;
    public string ObjectType;

    [Header("Properties that change the system")]
    public string axiom;
    public int Iterations;
    public float scalingOffset;

    [Range(-180.0f, 180.0f)]
    public float angle = 0.0f;

    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private string currentString;
    private Stack<TransformLS> transformStack = new Stack<TransformLS>();
    private bool isGenerating = false;
    private GameObject Empty;

    GameObject seasonSelector;


    private float startingAngle;
    //Grass
    int grassIterations;
    float SpacingBetweenBlades;

    // Start is called before the first frame update
    void Start()
    {
        seasonSelector = GameObject.Find("OtherSeasons");
        startingAngle = angle;
        if (ObjectType == "Tree")
        {
            Empty = new GameObject("Tree");
            Empty.tag = "Tree";
        }
        else
        {
            grassIterations = int.Parse(GameObject.Find("GrassDensityField").GetComponent<InputField>().text);
            SpacingBetweenBlades = int.Parse(GameObject.Find("BladeSpacing").GetComponent<InputField>().text);
            Empty = new GameObject("Grass");
            Empty.tag = "Tree";
        }

        if (SceneManager.GetActiveScene().name == "Road Drawing")
        {
            if (ObjectType == "Grass")
            {
                Empty.transform.SetParent(GameObject.Find("GrassContainer").transform);
            }
            else
            {
                Empty.transform.SetParent(GameObject.Find("Forest").transform);
            }
        }
        Empty.layer = 11;
        Empty.AddComponent<MeshFilter>();
        Empty.AddComponent<MeshRenderer>();
        StartUpRules();
        GenerateLSystem();
        ResetStartRules();
        Utility.combineMesh(Empty);

        Empty.AddComponent<Rigidbody>();
        Empty.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Empty.GetComponent<Rigidbody>().useGravity = false;
        Empty.AddComponent<BoxCollider>();
        Empty.AddComponent<ObjectCollisions>();

        if (ObjectType == "Tree")
        {
            Empty.AddComponent<TreeStats>();
        }
        else if(ObjectType == "Grass")
        {
            Empty.AddComponent<GrassStats>();
        }

    }

    void GenerateLSystem()
    {
        int count = 0;

        while (count < Iterations)
        {
            if (!isGenerating)
            {
                isGenerating = true;
                Generate();
                count++;
            }

        }

        if(count == Iterations)
        {
            Destroy(gameObject);
        }
    }

    void Generate()
    {

        string newString = "";
        char[] stringChar = currentString.ToCharArray();

        for (int i = 0; i < stringChar.Length; i++)
        {
            char currentChar = stringChar[i];
            if (rules.ContainsKey(currentChar))
            {
                newString += rules[currentChar];
            }
            else
            {
                newString += currentChar.ToString();
            }
        }
        currentString = newString;

        stringChar = currentString.ToCharArray();

        for (int i = 0; i < stringChar.Length; i++)
        {
            char currentChar = stringChar[i];
            //Translate Forwards
            if (currentChar == 'F') // Move forward while altering the new objects position
            {
                //Move Ahead
                Vector3 initPos = transform.position;
                if (ObjectType == "Grass")
                {
                    transform.Translate(Vector3.up * Utility.GetSize(obj).y / 1.1f);
                }
                else
                {
                    transform.Translate(Vector3.up * Utility.GetSize(obj).y);
                }
                Vector3 distanceBetween = transform.position - initPos;
                GameObject newObj = Instantiate(obj, transform);

                newObj.transform.parent = Empty.transform;
                newObj.transform.localPosition = transform.position - (distanceBetween / 2);
                newObj.transform.localRotation = transform.rotation;
            }
            else if (currentChar == 'T') // Move ahead without creating a new gameobject
            {
                //Move Ahead
                Vector3 initPos = transform.position;
                transform.Translate(Vector3.up * (Utility.GetSize(obj).y / 2));

                Vector3 distanceBetween = transform.position - initPos;

                GameObject newObj = Instantiate(obj, transform);
                newObj.transform.parent = Empty.transform;
                newObj.transform.localPosition = transform.position - (distanceBetween / 2);
                newObj.transform.localRotation = transform.rotation;
            }
            //Translate Forwards without Creating object
            else if (currentChar == 'f') // Move ahead without creating a new gameobject
            {
                Vector3 initPos = transform.position;
                transform.Translate(Vector3.up * Utility.GetSize(obj).y);

            }
            else if (currentChar == 't') // Move ahead without creating a new gameobject
            {
                Vector3 initPos = transform.position;
                transform.Translate(Vector3.up * (Utility.GetSize(obj).y / 2));
            }
            //Rotations
            else if (currentChar == '+') // Yaw to the right (Right in the X Axis)
            {
                transform.Rotate(Vector3.right * angle);
            } // Goes Right
            else if (currentChar == '-') // Yaw to the Left (Left in the X Axis)
            {
                transform.Rotate(Vector3.left * angle);
            } // Goes Left
            else if (currentChar == '&')    // Pitch Forward ( Forward in the Z Axis)
            {
                transform.Rotate(Vector3.forward * angle);
            } // Forward Z
            else if (currentChar == '^')    // Pitch Back ( Back in the Z Axis)
            {
                transform.Rotate(Vector3.back * angle);
            } // Back Z
            else if (currentChar == '¬')    // Pitch Forward ( Forward in the Y Axis)
            {
                //transform.position += new Vector3(0, 4, 0);
                transform.Rotate(Vector3.up * angle);
            } // Forward Y
            else if (currentChar == '|')    // Pitch Back ( Back in the Y Axis)
            {
                //transform.position += new Vector3(0, 4, 0);
                transform.Rotate(Vector3.down * angle);
            } // Back Y
            //Rotation
            else if (currentChar == '~')    // Rotate a random amount between 0 and the angle
            {
                //transform.position += new Vector3(0, 4, 0);
                Quaternion angleRot = Quaternion.Euler(Random.Range(-160, 160), Random.Range(-160, 160), Random.Range(-160, 160));
                transform.rotation = angleRot;

            } // Random Rotation
            else if (currentChar == '*')
            {
                angle = 45;
            } // Changes angle to 45 degrees
            else if (currentChar == '$')
            {
                angle = startingAngle;
            } // Changes back to original angle
            else if (currentChar == 'V')
            {
                Quaternion angleRot = Quaternion.Euler(Random.Range(-15, 15), Random.Range(-15, 15), Random.Range(-15, 15));
                transform.rotation = angleRot;
            } // Rotate randomly across a small rotation range
            else if (currentChar == 'J')
            {
                transform.Translate(new Vector3(Random.Range(-SpacingBetweenBlades, SpacingBetweenBlades), 0, Random.Range(-SpacingBetweenBlades, SpacingBetweenBlades)));
            } // Spacing between each piece
            else if (currentChar == '@')
            {
                int chance = Random.Range(0, 4);

                if(chance == 0)
                {
                    transform.Rotate(Vector3.right * angle);
                }
                else if( chance == 1)
                {
                    transform.Rotate(Vector3.left * angle);
                }
                else if (chance == 2)
                {
                    transform.Rotate(Vector3.forward * angle);
                }
                else if (chance == 3)
                {
                    transform.Rotate(Vector3.back * angle);
                }
            } // Randomly Right left back or Forward
            // Change Object Type
            else if (currentChar == '1')
            {
                obj = Resources.Load("Trees/Summer2") as GameObject;
            } // LEaves
            else if (currentChar == '2')
            {
                obj = Resources.Load("Trees/Cube") as GameObject;
            } // Branch
            else if (currentChar == '3')
            {
                obj = Resources.Load("GrassFolder/petal") as GameObject;
            } // Petals
            else if (currentChar == '4')
            {
                obj = Resources.Load("GrassFolder/GrassPiece") as GameObject;
            } // Grass Blade
            else if (currentChar == '5')
            {
                obj = Resources.Load("GrassFolder/Centre") as GameObject;
            } // Centre of flower
            //Pop and Push
            else if (currentChar == '[') //Push 
            {
                TransformLS tLs = new TransformLS();
                tLs.position = transform.position;
                tLs.rotation = transform.rotation;
                transformStack.Push(tLs);
            } // Pushes to transform stack
            else if (currentChar == ']') // Pop
            {
                TransformLS tLs = transformStack.Pop();
                transform.position = tLs.position;
                transform.rotation = tLs.rotation;
            } // Pops from the transform stack
        }
        isGenerating = false;
    }

    void StartUpRules()
    {
        if (ObjectType == "Tree")
        {
            int num = Random.Range(0, 8);

            if (num == 0)
            {
                //Basic Tree(XYYY '90 degrees')
                axiom = "XYYY";
                Iterations = 1;
                angle = 90;
                rules.Add('X', "2F[~F]F[~F]F[~F][~F][~F][~F][~F]"); // Upwards and add the branch
                rules.Add('Y', "1[+F[~F]F[~F]][-F[~F]F[~F]][^F[~F]F[~F]][&F[~F]F[~F]]F");
            }
            else if (num == 1)
            {
                // Slightly Changed Basic Tree (Axiom = X, Angle = '90 degrees', 1 iteration)
                axiom = "X";
                Iterations = 1;
                angle = 90;
                rules.Add('X', "FF" +
                    "[1+FF]" +
                    "[1-FF]" +
                    "[1^FF]" +
                    "[1&FF]" +
                    "t" +
                    "[1~FF]" +
                    "[1~FF]" +
                    "[1~FF]" +
                    "[1~FF]" +
                    "t" +
                    "[1+F]" +
                    "[1-F]" +
                    "[1^F]" +
                    "[1&F]" +
                    "F"); // Upwards and add the branch
            }
            else if (num == 2)
            {
                // Cone Tree (Axiom = X, Angle = 90, 1 iteration)
                axiom = "X";
                Iterations = 1;
                angle = 90;
                rules.Add('X', "F1" +
                "[+FF]" +
                "[-FF]" +
                "[^FF]" +
                "[&FF]" +
                "[^*+$FF]" +
                "[^*-$FF]" +
                "[&*+$FF]" +
                "[&*-$FF]" +
                "t" +
                "t" +
                "[+FT]" +
                "[-FT]" +
                "[^FT]" +
                "[&FT]" +
                "[^*+$FT]" +
                "[^*-$FT]" +
                "[&*+$FT]" +
                "[&*-$FT]" +
                "t" +
                "t" +
                "[+TT]" +
                "[-TT]" +
                "[^TT]" +
                "[&TT]" +
                "[^*+$TT]" +
                "[^*-$TT]" +
                "[&*+$TT]" +
                "[&*-$TT]" +
                "t" +
                "t" +
                "[+F]" +
                "[-F]" +
                "[^F]" +
                "[&F]" +
                "[^*+$F]" +
                "[^*-$F]" +
                "[&*+$F]" +
                "[&*-$F]" +
                "t" +
                "t" +
                "[+T]" +
                "[-T]" +
                "[^T]" +
                "[&T]" +
                "[^*+$T]" +
                "[^*-$T]" +
                "[&*+$T]" +
                "[&*-$T]" +
                "F");
            }
            else if (num == 3)
            {
                // Round Tree ( AXIOM = XYZQQZY, Angle = 90, iterations = 1)
                axiom = "XYZQQZY";
                Iterations = 1;
                angle = 90;
                rules.Add('X', "F[*~F$]F[*+F$][*-F$][*^F$][*&F$]");

                rules.Add('Y', "T" +
                "[1+T]" +
                "[1-T]" +
                "[1^T]" +
                "[1&T]" +
                "[1^*+$TT]" +
                "[1^*-$TT]" +
                "[1&*+$TT]" +
                "[1&*-$TT]");


                rules.Add('Z', "T" +
                "[1+FT]" +
                "[1-FT]" +
                "[1^FT]" +
                "[1&FT]" +
                "[1^*+$FT]" +
                "[1^*-$FT]" +
                "[1&*+$FT]" +
                "[1&*-$FT]");

                rules.Add('Q', "T" +
                "[1+FF]" +
                "[1-FF]" +
                "[1^FF]" +
                "[1&FF]" +
                "[1^*+$FF]" +
                "[1^*-$FF]" +
                "[1&*+$FF]" +
                "[1&*-$FF]");
            }
            else if (num == 4)
            {
                // Flat Top Tree - Dracaena Draco (AXIOM = XYZQWE, Angle = 90, Iterations = 1)
                axiom = "XYZQWE";
                Iterations = 1;
                angle = 90;
                rules.Add('X', "F[*~F$]F[*+FF$][*-FF$][*^FF$][*&FF$]F1F");
                rules.Add('Y',
                "[1+FF]" +
                "[1-FF]" +
                "[1^FF]" +
                "[1&FF]" +
                "[1^*+$FF]" +
                "[1^*-$FF]" +
                "[1&*+$FF]" +
                "[1&*-$FF]");
                rules.Add('Z',
                "[1+FFF]" +
                "[1-FFF]" +
                "[1^FFF]" +
                "[1&FFF]" +
                "[1^*+$FFF]" +
                "[1^*-$FFF]" +
                "[1&*+$FFF]" +
                "[1&*-$FFF]");
                rules.Add('Q',
                "-ff+t" +
                "[1+FFF]" +
                "[1-FFF]" +
                "[1^FFF]" +
                "[1&FFF]" +
                "[1^*+$FFF]" +
                "[1^*-$FFF]" +
                "[1&*+$FFF]" +
                "[1&*-$FFF]");

                rules.Add('W',
                "+ffff-t" +
                "[1+FFF]" +
                "[1-FFF]" +
                "[1^FFF]" +
                "[1&FFF]" +
                "[1^*+$FFF]" +
                "[1^*-$FFF]" +
                "[1&*+$FFF]" +
                "[1&*-$FFF]");

                rules.Add('E',
                "-fff+t" +
                "[1+FFF]" +
                "[1-FFF]" +
                "[1^FFF]" +
                "[1&FFF]" +
                "[1^*+$FFF]" +
                "[1^*-$FFF]" +
                "[1&*+$FFF]" +
                "[1&*-$FFF]");
            }
            else if (num == 5)
            {
                // Oval Tree ( AXIOM = XYZWWWQZ, Angle = 90, iterations = 1)
                axiom = "XYZWWWQZ";
                Iterations = 1;
                angle = 90;
                rules.Add('X', "2F[*~F$]F[*+F$][*-F$][*^F$][*&F$]");

                rules.Add('Y', "T" +
                "[1+TT]" +
                "[1-TT]" +
                "[1^TT]" +
                "[1&TT]" +
                "[1^*+$TT]" +
                "[1^*-$TT]" +
                "[1&*+$TT]" +
                "[1&*-$TT]");


                rules.Add('Z', "T" +
                "[1+FT]" +
                "[1-FT]" +
                "[1^FT]" +
                "[1&FT]" +
                "[1^*+$FT]" +
                "[1^*-$FT]" +
                "[1&*+$FT]" +
                "[1&*-$FT]");

                rules.Add('Q', "T" +
                "[1+FF]" +
                "[1-FF]" +
                "[1^FF]" +
                "[1&FF]" +
                "[1^*+$FF]" +
                "[1^*-$FF]" +
                "[1&*+$FF]" +
                "[1&*-$FF]");

                rules.Add('W', "F" +
                          "[1+FF]" +
                          "[1-FF]" +
                          "[1^FF]" +
                          "[1&FF]" +
                          "[1^*+$FF]" +
                          "[1^*-$FF]" +
                          "[1&*+$FF]" +
                          "[1&*-$FF]");
            }
            else if (num == 6)
            {
                // Some Kind Tree ( AXIOM = 2X1VCP, Angle = 90.0, iterations = 4)
                axiom = "2X1VCP";
                Iterations = 4;
                angle = 90.0f;
                rules.Add('X', "F");
                rules.Add('C', "[+FF]" +
                                "[-FF]" +
                                "[^FF]" +
                                "[&FF]" +
                                "[1 ^ *+$FF]" +
                                "[1^*-$FF]" +
                                "[1&*+$FF]" +
                                "[1&*-$FF]");
                rules.Add('P', "t[+F]" +
                                "[-F]" +
                                "[^F]" +
                                "[&F]");
            }
            else if (num == 7)
            {
                //Random Tree(XVCXVC '90 degrees')
                axiom = "XVCXVC";
                Iterations = 2;
                angle = 90;
                rules.Add('X', "2$F");
                rules.Add('C', "[*@FF" +
                    "[1tF]" +
                    "[~tT~tT]" +
                    "[~tT~tT]" +
                    "[~tT~tT]" +
                    "[~tT~tT]" +
                    "[~tT~tT]" +
                    "]");

            }
        }
        else if (ObjectType == "Grass")
        {
            int num = Random.Range(0, 5);

            if (num == 0)
            {
                //Three Piece grass(X '90 degrees')
                axiom = "X";
                Iterations = grassIterations;
                angle = 90;
                rules.Add('X', "[J[VFVF][+f-VFVF][+t^tVFVF][+t&tVFVF]]");
            }
            else if (num == 1)
            {
                //Three Piece grass(X '90 degrees')
                axiom = "X";
                Iterations = grassIterations;
                angle = 90;
                rules.Add('X', "[J[VFVF][+f-VFVF][+t^tVFVF][+t&tVFVF]]");
            }
            else if (num == 2)
            {
                //Three Piece single grass(X '90 degrees')
                axiom = "X";
                Iterations = grassIterations;
                angle = 90;
                rules.Add('X', "[J[VFVF][VFVF][VFVF]]");
            }
            else if (num == 3)
            {
                //Three Piece single grass(X '90 degrees')
                axiom = "X";
                Iterations = grassIterations;
                angle = 90;
                rules.Add('X', "[JVFVF][JVFVF][JVFVF]");
            }
            else if (num == 4)
            {
                //Flower(X '90 degrees')
                axiom = "X";
                Iterations = grassIterations;
                angle = 90;
                rules.Add('X', "4J[VF[5F][3&fffF][3^fffF][3+¬fffF][3-¬fffF]]"); // Upwards and add the branch
            }
        }

        currentString = axiom;
    }

    void ResetStartRules()
    {
        angle = startingAngle;
        rules.Clear();
    }

    private void Update()
    {
        grassIterations = int.Parse(GameObject.Find("GrassDensityField").GetComponent<InputField>().text);
        SpacingBetweenBlades = int.Parse(GameObject.Find("BladeSpacing").GetComponent<InputField>().text);
    }
}
