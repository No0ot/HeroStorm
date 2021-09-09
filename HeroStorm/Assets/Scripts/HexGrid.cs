using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    [SerializeField]
    Hex HexPrefab;
    public HexGridChunk chunkPrefab;

    int hexCountX = 6;
    int hexCountZ = 6;

    public int chunkCountX = 4;
    public int chunkCountZ = 3;

    Hex[] hexList;
    HexGridChunk[] hexChunkList;

    public Text hexLabelPrefab;

    public Color defaultColor = Color.green;

    private void Awake()
    {
        hexCountX = chunkCountX * HexMetrics.chunkSizeX;
        hexCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateHexs();
    }

    void CreateChunks()
    {
        hexChunkList = new HexGridChunk[chunkCountX * chunkCountZ];

        for(int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for(int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk newChunk = hexChunkList[i++] = Instantiate(chunkPrefab);
                newChunk.transform.SetParent(transform);
            }
        }
    }

    void CreateHexs()
    {
        hexList = new Hex[hexCountX * hexCountZ];

        for (int z = 0, i = 0; z < hexCountZ; z++)
        {
            for (int x = 0; x < hexCountX; x++)
            {
                CreateHex(x, z, i++);
            }
        }
    }

    void CreateHex(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        Hex newHex = hexList[i] = Instantiate<Hex>(HexPrefab);
        newHex.transform.localPosition = position;
        newHex.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        newHex.Color = defaultColor;

        if(x > 0)
        {
            newHex.SetNeighbour(HexDirection.W, hexList[i - 1]);
        }
        if(z > 0)
        {
            if((z & 1) == 0)
            {
                newHex.SetNeighbour(HexDirection.SE, hexList[i - hexCountX]);
                if(x > 0)
                {
                    newHex.SetNeighbour(HexDirection.SW, hexList[i - hexCountX - 1]);
                }
            }
            else
            {
                newHex.SetNeighbour(HexDirection.SW, hexList[i - hexCountX]);
                if(x < hexCountX - 1)
                {
                    newHex.SetNeighbour(HexDirection.SE, hexList[i - hexCountX + 1]);
                }
            }
        }

        Text newLabel = Instantiate<Text>(hexLabelPrefab);
        newLabel.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        newLabel.text = newHex.coordinates.ToStringOnSeparateLines();
        newHex.uiRect = newLabel.rectTransform;

        newHex.Elevation = 0;

        AddCellToChunk(x, z, newHex);
    }

    void AddCellToChunk(int x, int z, Hex hex)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = hexChunkList[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddHex(localX + localZ * HexMetrics.chunkSizeX, hex);
    }

    public Hex GetHex (Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * hexCountX + coordinates.Z / 2;
        return hexList[index];
        Debug.Log("touched at" + coordinates.ToString());
    }

    public Hex GetHex(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= hexCountZ)
            return null;
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= hexCountX)
            return null;
        return hexList[x + z * hexCountX];
    }

    public void ShowUI(bool visible)
    {
        for(int i = 0; i < hexChunkList.Length; i++)
        {
            hexChunkList[i].ShowUI(visible);
        }
    }
}
