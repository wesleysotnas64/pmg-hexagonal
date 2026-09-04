using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicEnergyPropagation : MonoBehaviour
{
    public static DynamicEnergyPropagation Instance { get; private set; }

    [Header("Configurações do Algoritmo")]
    [SerializeField] private int initialEnergy = 5;
    [SerializeField] private float closeGrid = 0.5f;

    private Map map;

    public int InitialEnergy 
    { 
        get => initialEnergy; 
        set => initialEnergy = value; 
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GameObject mapObj = GameObject.Find("Map");
        if (mapObj != null)
        {
            map = mapObj.GetComponent<Map>();
        }

        if (map == null)
        {
            Debug.LogError("DynamicEnergyPropagation: Não foi possível encontrar o objeto 'Map' na cena!");
            return;
        }

        int centerCol = map.Width / 2;
        int centerLine = map.Height / 2;

        Generate(map, centerCol, centerLine);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGeneration();
        }
    }

    public void Generate(Map map, int startCol, int startLine)
    {
        if (map == null) return;

        Tile startTile = map.GetTile(startCol, startLine);
        if (startTile == null) return;

        PropagateEnergy(startTile, initialEnergy);
        UpdateAllBorders(map);
    }

    public void ExpandFromBorderTile(Tile borderTile)
    {
        if (map == null || borderTile == null || !borderTile.isBorder) return;

        // Dispara a propagação a partir dos vizinhos invisíveis do tile de borda clicado
        for (int dir = 0; dir < 6; dir++)
        {
            Tile neighbor = borderTile.neighbors[dir];

            // Se o vizinho existe e ainda não está visível na cena
            if (neighbor != null && !neighbor.isVisible)
            {
                int returnedEnergy = PropagateEnergy(neighbor, initialEnergy - 1);

                // Se a propagação gerou um caminho válido, conecta o tile de borda ao novo vizinho
                if (returnedEnergy > 0 && neighbor.isVisible)
                {
                    borderTile.OpenPath(dir);

                    int oppositeDir = (dir + 3) % 6;
                    neighbor.OpenPath(oppositeDir);
                }
            }
        }

        // Recalcula o estado de borda em todo o mapa após a expansão
        UpdateAllBorders(map);
    }

    private int PropagateEnergy(Tile tile, int energy)
    {
        if (tile == null || energy <= 0)
        {
            return 0;
        }

        if (tile.isVisited && Random.value < closeGrid)
        {
            return 0;
        }

        tile.isVisited = true;
        tile.SetVisibility(true);

        int currentLocalEnergy = energy;
        int[] neighborEnergies = new int[6];

        for (int dir = 0; dir < 6; dir++)
        {
            Tile neighbor = tile.neighbors[dir];

            if (neighbor == null) continue;

            neighborEnergies[dir] = PropagateEnergy(neighbor, energy - 1);

            if (neighborEnergies[dir] > 0 && neighbor.isVisible)
            {
                tile.OpenPath(dir);

                int oppositeDir = (dir + 3) % 6;
                neighbor.OpenPath(oppositeDir);
            }
        }

        int maxNeighborEnergy = 0;
        for (int i = 0; i < 6; i++)
        {
            if (neighborEnergies[i] > maxNeighborEnergy)
            {
                maxNeighborEnergy = neighborEnergies[i];
            }
        }

        tile.currentEnergy = Mathf.Max(currentLocalEnergy, maxNeighborEnergy);

        return tile.currentEnergy;
    }

    private void UpdateAllBorders(Map map)
    {
        for (int c = 0; c < map.Width; c++)
        {
            for (int l = 0; l < map.Height; l++)
            {
                Tile tile = map.GetTile(c, l);
                if (tile != null)
                {
                    tile.UpdateBorderState();
                }
            }
        }
    }

    public void RestartGeneration()
    {
        if (map == null) return;

        map.ResetMap();

        int centerCol = map.Width / 2;
        int centerLine = map.Height / 2;

        Generate(map, centerCol, centerLine);
    }
}