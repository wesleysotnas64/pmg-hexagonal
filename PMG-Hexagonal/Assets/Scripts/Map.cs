using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float gap;
    
    private Tile[,] tiles;
    public GameObject tilePrefab;

    public Tile[,] Tiles => tiles;
    public int Width => width;
    public int Height => height;

    void Awake()
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

        // 1. Instancia e posiciona todos os tiles no grid
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

        // 2. Conecta os vizinhos de cada tile na grade
        LinkAllNeighbors();

        // Passamos effectiveWidth e effectiveHeight para incluir o gap no padding da câmera
        GameObject cameraController = GameObject.Find("Main Camera");
        if (cameraController != null && cameraController.TryGetComponent<CameraController>(out var camScript))
        {
            camScript.FitCameraToMap(totalWidth, totalHeight, effectiveWidth, effectiveHeight);
        }
    }

    /// <summary>
    /// Popula o array neighbors[6] de cada Tile com base nas coordenadas flat-topped (Odd-Q).
    /// </summary>
    private void LinkAllNeighbors()
    {
        for (int c = 0; c < width; c++)
        {
            for (int l = 0; l < height; l++)
            {
                Tile current = tiles[c, l];
                if (current == null) continue;

                bool isOddColumn = (c % 2 != 0);

                // Deslocamentos (deltaColumn, deltaLine) para cada direção:
                // 0: UpLeft, 1: Up, 2: UpRight, 3: BottomRight, 4: Bottom, 5: BottomLeft
                int[,] evenOffsetDirections = new int[,]
                {
                    { -1,  0 }, // 0: UpLeft
                    {  0,  1 }, // 1: Up
                    {  1,  0 }, // 2: UpRight
                    {  1, -1 }, // 3: BottomRight
                    {  0, -1 }, // 4: Bottom
                    { -1, -1 }  // 5: BottomLeft
                };

                // Colunas Ímpares (c % 2 != 0)
                int[,] oddOffsetDirections = new int[,]
                {
                    { -1,  1 }, // 0: UpLeft
                    {  0,  1 }, // 1: Up
                    {  1,  1 }, // 2: UpRight
                    {  1,  0 }, // 3: BottomRight
                    {  0, -1 }, // 4: Bottom
                    { -1,  0 }  // 5: BottomLeft
                };

                for (int dir = 0; dir < 6; dir++)
                {
                    // Seleciona a matriz de offsets com base na paridade da coluna c (Even vs Odd)
                    int neighborCol = c + (isOddColumn ? oddOffsetDirections[dir, 0] : evenOffsetDirections[dir, 0]);
                    int neighborRow = l + (isOddColumn ? oddOffsetDirections[dir, 1] : evenOffsetDirections[dir, 1]);

                    if (IsWithinBounds(neighborCol, neighborRow))
                    {
                        current.neighbors[dir] = tiles[neighborCol, neighborRow];
                    }
                    else
                    {
                        current.neighbors[dir] = null;
                    }
                }
            }
        }
    }

    public bool IsWithinBounds(int col, int line)
    {
        return col >= 0 && col < width && line >= 0 && line < height;
    }

    public Tile GetTile(int col, int line)
    {
        if (IsWithinBounds(col, line))
        {
            return tiles[col, line];
        }
        return null;
    }

    public void ResetMap()
    {
        // Destrói os GameObjects de tiles que foram instanciados
        if (tiles != null)
        {
            for (int c = 0; c < width; c++)
            {
                for (int l = 0; l < height; l++)
                {
                    if (tiles[c, l] != null)
                    {
                        Destroy(tiles[c, l].gameObject);
                    }
                }
            }
        }

        // Limpa a referência da matriz
        tiles = null;

        // Reinicia e recria a grade
        Initialize();
    }
}