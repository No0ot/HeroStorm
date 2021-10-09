using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    Hex[] hexList;

    public HexMesh terrain, water;
    Canvas gridCanvas;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        hexList = new Hex[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }

    public void AddHex(int index, Hex hex)
    {
        hexList[index] = hex;
        hex.chunk = this;
        hex.transform.SetParent(transform, false);
        hex.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        enabled = true;
    }

    private void LateUpdate()
    {
        Triangulate();
        enabled = false;   
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }

    public void Triangulate()
    {
        terrain.Clear();
        water.Clear();

        for (int i = 0; i < hexList.Length; i++)
        {
            Triangulate(hexList[i]);
        }
       
        terrain.Apply();
        water.Apply();
    }

    void Triangulate(Hex hex)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, hex);
        }

      

    }

    void Triangulate(HexDirection direction, Hex hex)
    {
        Vector3 center = hex.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        terrain.AddTriangle(center, v1, v2);
        terrain.AddTriangleColor(hex.Color);

        if (direction <= HexDirection.SE)
            TriangulateConnection(direction, hex, v1, v2);
        if (hex.isUnderwater)
            TriangulateWater(direction, hex, center);
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

        terrain.AddQuad(v1, v2, v3, v4);
        terrain.AddQuadColor(hex.Color, neighbour.Color);

        Hex nextNeighbour = hex.GetNeighbour(direction.Next());
        if (direction <= HexDirection.E && nextNeighbour != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbour.Elevation * HexMetrics.elevationStep;
            terrain.AddTriangle(v2, v4, v5);
            terrain.AddTriangleColor(hex.Color, neighbour.Color, nextNeighbour.Color);
        }
    }
    
    void TriangulateWater(HexDirection direction, Hex hex, Vector3 center)
    {
        center.y = hex.WaterSurfaceY;

        Hex neighbour = hex.GetNeighbour(direction);
        if (neighbour != null && !neighbour.isUnderwater)
            TriangulateWaterShore(direction, hex, neighbour, center);
        else
            TriangulateOpenWater(direction, hex, neighbour, center);
    }

    void TriangulateOpenWater(HexDirection direction, Hex hex, Hex neighbour, Vector3 center)
    {
         Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
         Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

         water.AddTriangle(center, v1, v2);

        if (direction <= HexDirection.SE && neighbour != null)
        {
            Vector3 bridge = HexMetrics.GetBridge(direction);
            Vector3 e1 = v1 + bridge;
            Vector3 e2 = v2 + bridge;

            water.AddQuad(v1, v2, e1, e2);

            if (direction <= HexDirection.E)
            {
                Hex nextNeighbour = hex.GetNeighbour(direction.Next());
                if (nextNeighbour == null || !nextNeighbour.isUnderwater)
                    return;
                water.AddTriangle(v2, e2, v2 + HexMetrics.GetBridge(direction.Next()));
            }
        }
    }

    void TriangulateWaterShore(HexDirection direction, Hex hex, Hex neighbour, Vector3 center)
    {
        Vector3 bridge = HexMetrics.GetBridge(direction);

        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
    }
}
