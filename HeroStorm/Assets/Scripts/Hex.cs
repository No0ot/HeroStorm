using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public HexCoordinates coordinates;
    public HexGridChunk chunk;

    int waterLevel;

    Color color;

    [SerializeField]
    public Hex[] neighbours;

    public RectTransform uiRect;

    int elevation = int.MinValue;

    public float WaterSurfaceY
    {
        get
        {
            return (waterLevel + HexMetrics.waterElevationOffset) * (HexMetrics.elevationStep);
        }
    }
    public bool isUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

    public int WaterLevel 
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }
            waterLevel = value;
            Refresh();
        }
    }
    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if (color == value)
                return;
            color = value;
            Refresh();
        }
    }

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
                return;
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
            Refresh();
        }
    }


    public Hex GetNeighbour (HexDirection direction)
    {
        return neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, Hex hex)
    {
        neighbours[(int)direction] = hex;
        hex.neighbours[(int)direction.Opposite()] = this;
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for(int i = 0; i < neighbours.Length; i++)
            {
                Hex neighbour = neighbours[i];
                if (neighbour != null && neighbour.chunk != chunk)
                    neighbour.chunk.Refresh();
            }
        }
    }
}
