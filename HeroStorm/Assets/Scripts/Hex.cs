using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public HexCoordinates coordinates;

    public Color color;

    [SerializeField]
    public Hex[] neighbours;

    public RectTransform uiRect;

    int elevation;
    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
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
}
