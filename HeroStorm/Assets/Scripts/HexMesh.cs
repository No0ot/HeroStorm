using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    [SerializeField]
    List<Vector3> vertices;
    List<int> triangles;

    MeshCollider collider;
    [SerializeField]
    List<Color> colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        collider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

   public void Triangulate(Hex[] hex_list)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for(int i = 0; i < hex_list.Length; i++)
        {
            Triangulate(hex_list[i]);
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();

        collider.sharedMesh = hexMesh;
    }

    void Triangulate(Hex hex)
    {
        for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, hex);
        }

    }

    void Triangulate(HexDirection direction, Hex hex)
    {
        Vector3 center = hex.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        AddTriangle(center, v1, v2);
        AddTriangleColor(hex.color);

        if(direction <= HexDirection.SE)
            TriangulateConnection(direction, hex, v1, v2);

        //AddTriangle(v1, center + HexMetrics.GetFirstCorner(direction), v3);
        //AddTriangleColor(hex.color, (hex.color + prevNeighbour.color + neighbour.color) / 3f, bridgeColor);
        //
        //AddTriangle(v2, v4, center + HexMetrics.GetSecondCorner(direction));
        //AddTriangleColor(hex.color, bridgeColor, (hex.color + neighbour.color + nextNeighbour.color) / 3f);
    }

    void TriangulateConnection(HexDirection direction, Hex hex, Vector3 v1, Vector3 v2)
    {
        Hex neighbour = hex.GetNeighbour(direction);
        if (neighbour == null)
            return;

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbour.Elevation * HexMetrics.elevationStep;

        AddQuad(v1, v2, v3, v4);
        AddQuadColor(hex.color, neighbour.color);

        Hex nextNeighbour = hex.GetNeighbour(direction.Next());
        if(direction <= HexDirection.E && nextNeighbour != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbour.Elevation * HexMetrics.elevationStep;
            AddTriangle(v2, v4, v5);
            AddTriangleColor(hex.color, neighbour.color, nextNeighbour.color);
        }
    }
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    void AddTriangleColor(Color c1)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c1);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
