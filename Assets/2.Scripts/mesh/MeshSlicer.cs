using System.Collections.Generic;
using UnityEngine;

public class MeshSlicer
{
    public static void Slice(GameObject obj, float height)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        var scale = obj.transform.lossyScale;
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Mesh topMesh = new Mesh();
        Mesh bottomMesh = new Mesh();

        Vector3[] topVertices = vertices;
        Vector3[] bottomVertices = vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y > height)
            {
                bottomVertices[i] = new Vector3(vertices[i].x, height, vertices[i].z);
            }
            else
            {
                topVertices[i] = new Vector3(vertices[i].x, height, vertices[i].z);
            }
        }

        topMesh.vertices = topVertices;
        topMesh.triangles = triangles;

        bottomMesh.vertices = bottomVertices;
        bottomMesh.triangles = triangles;

        meshFilter.mesh = topMesh;

        GameObject bottomPart = new GameObject("BottomPart");
        bottomPart.transform.position = obj.transform.position;
        bottomPart.AddComponent<MeshFilter>().mesh = bottomMesh;
        bottomPart.AddComponent<MeshRenderer>().material = obj.GetComponent<MeshRenderer>().material;
        bottomPart.transform.localScale = scale;
    }

    public static void Slice2(GameObject target, float sliceHeight)
    {
        Mesh mesh = target.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        // For simplicity, we'll assume the slice is horizontal (y-axis).
        // We separate vertices into those above and below the slice.
        // More complex slicing would require more advanced logic.
        List<Vector3> aboveVerts = new List<Vector3>();
        List<Vector3> belowVerts = new List<Vector3>();

        foreach (var vertex in vertices)
        {
            if (vertex.y >= sliceHeight)
                aboveVerts.Add(vertex);
            else
                belowVerts.Add(vertex);
        }

        // TODO: Create two new meshes using the above and below vertices.
        // This is where the complexity lies, as you must also split triangles that straddle the slice,
        // compute new UVs, etc. This example only separates vertices.
        // For a full solution, you'd likely need a more advanced algorithm and potentially external libraries.

        // For now, just color the original mesh to visualize the slice.
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].y >= sliceHeight)
                colors[i] = Color.red;  // Color vertices above the slice in red.
            else
                colors[i] = Color.blue; // Color vertices below the slice in blue.
        }
        mesh.colors = colors;
    }
}