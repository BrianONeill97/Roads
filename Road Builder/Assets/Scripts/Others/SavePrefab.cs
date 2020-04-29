using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class SavePrefab : MonoBehaviour
{
    GameObject track;
    public GameObject inputField;
    //[MenuItem("Track/Save Prefab")]

    private void Start()
    {
        //TrackNumber.trackNum++;
        //track = GameObject.FindGameObjectWithTag("Track");

        //// Create the Top level Folder
        //string guidTOP = AssetDatabase.CreateFolder("Assets/Prefabs", "Track" + TrackNumber.trackNum);
        //string newFolderTOP = AssetDatabase.GUIDToAssetPath(guidTOP);

        //// Create the Mesh folder
        //string guidMESH = AssetDatabase.CreateFolder("Assets/Prefabs/Track" + TrackNumber.trackNum, "Meshes");
        //string newFolderMESH = AssetDatabase.GUIDToAssetPath(guidMESH);
    }

    public void CreatePrefab()
    {
        //Debug.Log("Saving");

        //string path = "Assets/Prefabs/Tracks/" + inputField.GetComponent<InputField>().text + ".prefab";

        //path = AssetDatabase.GenerateUniqueAssetPath(path);

        //PrefabUtility.SaveAsPrefabAssetAndConnect(track, path, InteractionMode.UserAction);
        //Debug.Log("Saved");
    }
}
