using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour
{
    [MenuItem("EditorTool/DeleteAllPlayerPrefs")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Delete All PlayerPrefs");
    }
}
