using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PolygonMeshCreator : MonoBehaviour
{
    [Header("Polygon Vertices (Local Space)")]
    public List<Vector3> vertices = new List<Vector3>()
    {
        new Vector3(-1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1)
    };

    [HideInInspector] public Mesh mesh;

#if UNITY_EDITOR
    private double lastEditTime = -1;
    [SerializeField] private float _updateDelay = 1.0f; // seconds

    private void Update()
    {
        if (!Application.isPlaying && lastEditTime > 0 &&
            EditorApplication.timeSinceStartup - lastEditTime > _updateDelay)
        {
            lastEditTime = -1;
            GenerateMesh();
        }
    }

    // Called from the editor when vertex moved
    public void ScheduleMeshUpdate()
    {
        lastEditTime = EditorApplication.timeSinceStartup;
    }
#endif

    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
        if (vertices.Count < 3)
        {
            Debug.LogWarning("Need at least 3 vertices to create a polygon mesh.");
            return;
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Polygon Mesh";
        }
        else mesh.Clear();

        // Flatten to 2D for triangulation (XZ plane)
        Vector2[] verts2D = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
            verts2D[i] = new Vector2(vertices[i].x, vertices[i].z);

        int[] triangles = TriangulateConvexPolygon(verts2D);

        // ✅ Reverse triangle winding so normals face upward
        System.Array.Reverse(triangles);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Simple planar UVs
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        mesh.uv = uvs;

        GetComponent<MeshFilter>().sharedMesh = mesh;
        Debug.Log("✅ Polygon mesh generated successfully!");
    }

    private int[] TriangulateConvexPolygon(Vector2[] verts)
    {
        List<int> triangles = new List<int>();
        for (int i = 1; i < verts.Length - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }
        return triangles.ToArray();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null || vertices.Count == 0) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 a = transform.TransformPoint(vertices[i]);
            Vector3 b = transform.TransformPoint(vertices[(i + 1) % vertices.Count]);
            Gizmos.DrawLine(a, b);
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(PolygonMeshCreator))]
public class PolygonMeshCreatorEditor : Editor
{
    private void OnSceneGUI()
    {
        var creator = (PolygonMeshCreator)target;

        if (creator.vertices == null || creator.vertices.Count == 0)
            return;

        Handles.color = Color.cyan;
        for (int i = 0; i < creator.vertices.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 worldPos = creator.transform.TransformPoint(creator.vertices[i]);
            Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(creator, "Move Polygon Vertex");
                creator.vertices[i] = creator.transform.InverseTransformPoint(newWorldPos);
                EditorUtility.SetDirty(creator);
                creator.ScheduleMeshUpdate(); // 🔥 Schedule rebuild
            }
        }
    }
}
#endif
