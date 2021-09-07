using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public HexCoordinates coordinates;

    public Color color;

    [SerializeField]
    public Hex[] neighbours;

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
