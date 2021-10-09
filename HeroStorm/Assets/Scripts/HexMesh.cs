using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListPool<T>
{
    static Stack<List<T>> stack = new Stack<List<T>>();

    public static List<T> Get()
    {
        if (stack.Count > 0)
            return stack.Pop();

        return new List<T>();
    }

    public static void Add(List<T> list)
    {
        list.Clear();
        stack.Push(list);
    }
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    [System.NonSerialized]List<Vector3> vertices;
    [System.NonSerialized]List<int> triangles;
    [System.NonSerialized]List<Color> colors;
    [System.NonSerialized]List<Vector2> uvs;

    MeshCollider collider;

    public bool useCollider, useColor, useUVCoordinates;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        if(useCollider)
            collider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
    }

  public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        if(useColor)
            colors = ListPool<Color>.Get();
        if (useUVCoordinates)
            uvs = ListPool<Vector2>.Get();
        triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        if (useColor)
        {
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
        }
        if(useUVCoordinates)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);
        hexMesh.RecalculateNormals();
        if(useCollider)
            collider.sharedMesh = hexMesh;
    }

    Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * HexMetrics.hexPerturbStrength;
        //position.y += (sample.y * 2f - 1f) * HexMetrics.hexPerturbStrength;
        position.z += (sample.z * 2f - 1f) * HexMetrics.hexPerturbStrength;
        return position;
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    public void AddTriangleColor(Color c1)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c1);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        vertices.Add(Perturb(v4));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    public void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin,vMax));
        uvs.Add(new Vector2(uMax,vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }
}
