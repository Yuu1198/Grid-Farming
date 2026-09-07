using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject objectToPlace;
    public LayerMask groundLayer;
    public Transform objectParent;

    private float cellSize = 2f;
    private GameObject previewObject;

    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

    private Vector2Int currentGridPosition;
    private bool validPreviewPosition = false;

    public bool canPlaceTiles = true;


    private void Start()
    {
        CreatePreviewObject();
    }

    private void Update()
    {
        if (canPlaceTiles)
        {
            UpdatePreviewPosition();

            // Place Object with mouse click
            if (Input.GetMouseButtonDown(0) && validPreviewPosition)
            {
                PlaceObject();
            }
        }
    }

    public void ActivatePlacement()
    {
        CreatePreviewObject();
        canPlaceTiles = true;
    }

    public void DeactivatePlacement()
    {
        canPlaceTiles = false;
        Destroy(previewObject);
    }

    public void SetObjectToPlace(GameObject input)
    {
        objectToPlace = input;
    }

    /// <summary>
    /// Instantiate and set up Preview Object.
    /// </summary>
    private void CreatePreviewObject()
    {
        // Create Preview Object
        previewObject = Instantiate(objectToPlace);
        previewObject.GetComponent<Collider>().enabled = false;

        // Edit Alpha values
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            Material mat = renderer.material;

            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_ScrBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);

            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            mat.renderQueue = 3000;
        }
    }

    /// <summary>
    /// Keep Preview Object moving with the mouse.
    /// </summary>
    private void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Place Preview Object at Hit Position
            Vector3 point = hit.point;

            currentGridPosition = new Vector2Int(
                Mathf.RoundToInt(point.x / cellSize),
                Mathf.RoundToInt(point.z / cellSize)
                );

            // Convert grid position back to world position
            Vector3 snappedPosition = new Vector3(
                currentGridPosition.x * cellSize,
                hit.point.y,
                currentGridPosition.y * cellSize
            );

            previewObject.transform.position = snappedPosition;

            // Change color based on if position is occupied
            if (occupiedPositions.Contains(currentGridPosition))
            {
                validPreviewPosition = false;
                SetPreviewColor(Color.red);
            }
            else
            {
                validPreviewPosition = true;
                SetPreviewColor(new Color(1f, 1f, 1f, 0.5f));
            }
        }
        else
        {
            validPreviewPosition = false;
        }
    }

    /// <summary>
    /// Set Preview object to color.
    /// </summary>
    /// <param name="color">Chosen color</param>
    private void SetPreviewColor(Color color)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    /// <summary>
    /// Place the Tile.
    /// </summary>
    private void PlaceObject()
    {
        Vector3 placementPosition = previewObject.transform.position;

        Instantiate(objectToPlace, placementPosition, Quaternion.identity, objectParent);

        // Mark this grid cell as occupied
        occupiedPositions.Add(currentGridPosition);
    }
}
