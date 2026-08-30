using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float gap = 0.1f; // Espaçamento adicional entre os tiles
    
    private Tile[,] tiles;
    public GameObject tilePrefab;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        tiles = new Tile[width, height];

        SpriteRenderer sr = tilePrefab.GetComponent<SpriteRenderer>();
        
        // Aplica o gap nas dimensões efetivas do tile
        float effectiveWidth = sr.bounds.size.x + gap;
        float effectiveHeight = sr.bounds.size.y + gap;

        // Mantém a proporção geométrica dos hexágonos flat-topped
        float horizontalSpacing = effectiveWidth * 0.75f;
        float verticalSpacing = effectiveHeight;

        float totalWidth = (width - 1) * horizontalSpacing;
        float totalHeight = (height - 1) * verticalSpacing;
        
        if (width > 1)
        {
            totalHeight += verticalSpacing * 0.5f;
        }

        float offsetX = totalWidth / 2f;
        float offsetY = totalHeight / 2f;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject tileObj = Instantiate(tilePrefab, transform);
                
                Tile tileScript = tileObj.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.Initialize();
                    tileScript.SetCoords(j, i);
                }

                tiles[j, i] = tileScript;

                float posX = (j * horizontalSpacing) - offsetX;
                float posY = (i * verticalSpacing) - offsetY;

                if (j % 2 != 0)
                {
                    posY += verticalSpacing * 0.5f;
                }

                tileObj.transform.position = new Vector3(posX, posY, 0f);
            }
        }

        // Passamos effectiveWidth e effectiveHeight para incluir o gap no padding da câmera
        FitCameraToMap(totalWidth, totalHeight, effectiveWidth, effectiveHeight);
    }

    private void FitCameraToMap(float totalWidth, float totalHeight, float spriteWidth, float spriteHeight)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.orthographic) return;

        mainCamera.transform.position = new Vector3(0f, 0f, -10f);

        float mapWidthWithPadding = totalWidth + spriteWidth;
        float mapHeightWithPadding = totalHeight + spriteHeight;

        float sizeBasedOnHeight = mapHeightWithPadding / 2f;
        float screenAspect = (float)Screen.width / Screen.height;
        float sizeBasedOnWidth = (mapWidthWithPadding / 2f) / screenAspect;

        mainCamera.orthographicSize = Mathf.Max(sizeBasedOnHeight, sizeBasedOnWidth);
    }
}