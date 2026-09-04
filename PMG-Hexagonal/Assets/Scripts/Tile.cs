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
    public GameObject[] paths = new GameObject[6];

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

    public void SetColor(Color color)
    {
        foreach (GameObject path in paths)
        {
            if (path != null && path.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.color = color;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isBorder && DynamicEnergyPropagation.Instance != null)
        {
            DynamicEnergyPropagation.Instance.ExpandFromBorderTile(this);
        }
    }
}