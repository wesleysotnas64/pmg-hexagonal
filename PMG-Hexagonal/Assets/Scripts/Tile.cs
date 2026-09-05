using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    [Header("Coordenadas")]
    public int line;
    public int column;

    [Header("Estados do Algoritmo")]
    public bool isVisible;
    public bool isVisited;
    public bool isBorder;
    public int currentEnergy;

    [Header("Estruturas de Conexão (6 Lados)")]
    public Tile[] neighbors = new Tile[6];
    public bool[] isPathOpen = new bool[6];
    public GameObject[] paths = new GameObject[7];

    [Header("Informações do Bioma")]
    public BiomeType biomeType = BiomeType.None;

    public void Initialize()
    {
        line = 0;
        column = 0;
        isVisited = false;
        isBorder = false;
        currentEnergy = 0;

        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = null;
            isPathOpen[i] = false;
            if (paths[i] != null)
            {
                paths[i].SetActive(false);
            }
        }

        SetVisibility(false);
    }

    public void SetCoords(int column, int line)
    {
        this.column = column;
        this.line = line;
    }

    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        gameObject.SetActive(visible);
    }

    public void OpenPath(int direction)
    {
        if (direction < 0 || direction >= 6) return;

        isPathOpen[direction] = true;
        if (paths[direction] != null)
        {
            paths[direction].SetActive(true);
        }
    }

    public void UpdateBorderState()
    {
        isBorder = false;

        // Se o próprio tile não estiver visível, ele não é uma "borda ativa" de expansão
        if (!isVisible) return;

        for (int i = 0; i < 6; i++)
        {
            // É borda se falta algum vizinho no mapa OU se o vizinho existe mas está invisível
            if (neighbors[i] == null || !neighbors[i].isVisible)
            {
                isBorder = true;
                break;
            }
        }
    }

    public void SetBiome(BiomeType newBiome)
    {
        biomeType = newBiome;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color colrTile = Color.white;

        switch (biomeType)
        {
            case BiomeType.Forest:
                // Verde escuro
                colrTile = new Color(0.18f, 0.49f, 0.19f);
                break;

            case BiomeType.Lake:
                // Azul aquático
                colrTile = new Color(0.15f, 0.55f, 0.82f);
                break;

            case BiomeType.Plains:
                // Verde claro (grama)
                colrTile = new Color(0.48f, 0.78f, 0.35f);
                break;

            case BiomeType.Desert:
                // Amarelo areia
                colrTile = new Color(0.92f, 0.78f, 0.45f);
                break;

            case BiomeType.Swamp:
                // Verde lodoso / Lodo
                colrTile = new Color(0.32f, 0.38f, 0.22f);
                break;

            case BiomeType.Mountain:
                // Cinza rochoso
                colrTile = new Color(0.55f, 0.57f, 0.60f);
                break;

            case BiomeType.Tundra:
                // Branco/Azul gélido
                colrTile = new Color(0.82f, 0.93f, 0.96f);
                break;

            case BiomeType.Jungle:
                // Verde denso/tropical
                colrTile = new Color(0.08f, 0.38f, 0.12f);
                break;

            case BiomeType.Volcano:
                // Vermelho escuro/Lava
                colrTile = new Color(0.72f, 0.22f, 0.15f);
                break;

            case BiomeType.Wasteland:
                // Roxo tóxico/Sombrio
                colrTile = new Color(0.42f, 0.28f, 0.48f);
                break;

            default:
                colrTile = Color.white;
                break;
        }

        // Calcula uma versão 30% mais escura da cor do tile para o caminho
        float darknessFactor = 0.7f;
        Color colorPath = new Color(
            colrTile.r * darknessFactor,
            colrTile.g * darknessFactor,
            colrTile.b * darknessFactor,
            colrTile.a
        );

        if (spriteRenderer != null)
        {
            spriteRenderer.color = colrTile;
        }

        foreach (GameObject path in paths)
        {
            if (path != null && path.TryGetComponent<SpriteRenderer>(out var pathSpriteRenderer))
            {
                pathSpriteRenderer.color = colorPath;
            }
        }
    }

    // Interação via click será modificada posterioemente
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isBorder && DynamicEnergyPropagation.Instance != null)
        {
            BiomeManager.Instance.RegisterBiomeInteraction(biomeType);
            DynamicEnergyPropagation.Instance.ExpandFromBorderTile(this);
        }
    }
}