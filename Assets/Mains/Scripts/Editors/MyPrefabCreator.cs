using UnityEditor;
using UnityEngine;

public static class MyPrefabCreator
{
    private const string prefabPath = "Assets/Mains/Prefabs/MyPrefab.prefab"; // Source prefab path

    [MenuItem("Assets/Create/Prefabs/MyPrefab")]
    public static void CreatePrefabInstance()
    {
        // Load the source prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found at path: " + prefabPath);
            return;
        }

        // Determine the folder to save in (current selection or default to "Assets")
        string folderPath = "Assets";
        if (Selection.activeObject != null)
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // If a folder is selected, use it; if a file, use its parent folder
                folderPath = System.IO.Directory.Exists(selectedPath) ? selectedPath : System.IO.Path.GetDirectoryName(selectedPath);
            }
        }

        // Generate a unique path for the new prefab
        string newPrefabPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/NewMyPrefab.prefab");

        // Create the prefab
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(prefab, newPrefabPath);
        AssetDatabase.Refresh();

        // Select the newly created prefab
        Selection.activeObject = newPrefab;
    }
}
