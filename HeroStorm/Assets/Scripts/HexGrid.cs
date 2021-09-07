using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    [SerializeField]
    Hex HexPrefab;

    public int width = 6;
    public int height = 6;

    Hex[] HexList;

    public Text hexLabelPrefab;

    Canvas gridCanvas;
    HexMesh hexMesh;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        HexList = new Hex[width * height];

        for(int z = 0, i = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
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

        Hex newHex = HexList[i] = Instantiate<Hex>(HexPrefab);
        newHex.transform.SetParent(transform, false);
        newHex.transform.localPosition = position;
        newHex.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        Text newLabel = Instantiate<Text>(hexLabelPrefab);
        newLabel.rectTransform.SetParent(gridCanvas.transform, false);
        newLabel.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        newLabel.text = newHex.coordinates.ToStringOnSeparateLines();
    }

    // Start is called before the first frame update
    void Start()
    {
        hexMesh.Triangulate(HexList);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell (Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at" + coordinates.ToString());
    }
}
