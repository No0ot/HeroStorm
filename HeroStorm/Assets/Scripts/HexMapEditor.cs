using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    int activeElevation;

    void Awake()
    {
        SelectColor(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetHex(hit.point));
        }
    }

    void EditCell(Hex hex)
    {
        hex.color = activeColor;
        hex.Elevation = activeElevation;
        hexGrid.Refresh();
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
}
