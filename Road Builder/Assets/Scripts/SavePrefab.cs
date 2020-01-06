using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SavePrefab : MonoBehaviour
{
    public GameObject inputField;
    [MenuItem("Track/Save Prefab")]
    public void CreatePrefab()
    {
        Debug.Log("Saving");
        GameObject track = GameObject.FindGameObjectWithTag("Track");

        string path = "Assets/Prefabs/Tracks/" + inputField.GetComponent<InputField>().text + ".prefab";

        path = AssetDatabase.GenerateUniqueAssetPath(path);

        PrefabUtility.SaveAsPrefabAssetAndConnect(track, path, InteractionMode.UserAction);
        Debug.Log("Saved");
    }

}
