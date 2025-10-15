using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ReplaceWallsWithPrefabs : EditorWindow
{
    public GameObject wallPrefab;
    public GameObject wallHalfPrefab;

    [MenuItem("Tools/Replace Walls With Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceWallsWithPrefabs>("Replace Walls");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Wall Objects with Prefabs", EditorStyles.boldLabel);

        wallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", wallPrefab, typeof(GameObject), false);
        wallHalfPrefab = (GameObject)EditorGUILayout.ObjectField("Wall-Half Prefab", wallHalfPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace Selected Objects"))
        {
            ReplaceSelectedObjects();
        }
    }

    private void ReplaceSelectedObjects()
    {
        if (wallPrefab == null || wallHalfPrefab == null)
        {
            EditorUtility.DisplayDialog("Missing Prefabs", "Please assign both wall and wall-half prefabs.", "OK");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select at least one parent object.", "OK");
            return;
        }

        Undo.RecordObjects(selectedObjects, "Replace Walls With Prefabs");
        
        foreach (GameObject parent in selectedObjects)
        {
            List<Transform> childrenToReplace = new List<Transform>();
            
            // Collect all children first to avoid modification issues during iteration
            foreach (Transform child in parent.transform)
            {
                childrenToReplace.Add(child);
            }
            
            foreach (Transform child in childrenToReplace)
            {
                ReplaceIfNeeded(child);
            }
        }
    }

    private void ReplaceIfNeeded(Transform objectTransform)
    {
        string objectName = objectTransform.name.ToLower();
        GameObject prefabToUse = null;

        if (objectName.StartsWith("wall-half"))
        {
            prefabToUse = wallHalfPrefab;
        }
        else if (objectName.StartsWith("wall"))
        {
            prefabToUse = wallPrefab;
        }

        if (prefabToUse != null)
        {
            // Store transform values
            Vector3 position = objectTransform.position;
            Quaternion rotation = objectTransform.rotation;
            Vector3 scale = objectTransform.localScale;
            Transform parent = objectTransform.parent;
            string originalName = objectTransform.name;

            // Destroy old object
            GameObject oldObject = objectTransform.gameObject;
            
            // Create new object
            GameObject newObject = PrefabUtility.InstantiatePrefab(prefabToUse) as GameObject;
            
            // Apply stored transform values
            newObject.transform.SetParent(parent);
            newObject.transform.position = position;
            newObject.transform.rotation = rotation;
            newObject.transform.localScale = scale;
            newObject.name = originalName;
            
            Undo.RegisterCreatedObjectUndo(newObject, "Create replacement object");
            Undo.DestroyObjectImmediate(oldObject);
        }
    }
}
