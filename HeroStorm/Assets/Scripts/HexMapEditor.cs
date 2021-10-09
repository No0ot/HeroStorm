using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    int activeElevation;
    int activeWaterLevel;

    bool applyColor;
    bool applyElevation = true;
    bool applyWaterLevel = true;

    int brushSize;

    void Awake()
    {
        SelectColor(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetHex(hit.point));
        }
    }

    void EditCells(Hex center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for(int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetHex(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetHex(new HexCoordinates(x, z)));
            }
        }
    } 

    void EditCell(Hex hex)
    {
        if (hex)
        {
            if (applyColor)
                hex.Color = activeColor;

            if (applyElevation)
                hex.Elevation = activeElevation;
            if (applyWaterLevel)
                hex.WaterLevel = activeWaterLevel;
        }
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if(applyColor)
            activeColor = colors[index];
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void setApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel (float level)
    {
        activeWaterLevel = (int)level;
    }
}
