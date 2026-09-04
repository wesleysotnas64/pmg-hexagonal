using UnityEngine;
using UnityEngine.InputSystem;

public class DynamicEnergyPropagation : MonoBehaviour
{
    [Header("Configurações do Algoritmo")]
    [SerializeField] private int initialEnergy = 5;
    [SerializeField] private float closeGrid = 0.5f;

    private Map map;

    public int InitialEnergy 
    { 
        get => initialEnergy; 
        set => initialEnergy = value; 
    }

    private void Start()
    {
        // Localiza a instância do Map na cena
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

        // Calcula o tile central para o teste inicial
        int centerCol = map.Width / 2;
        int centerLine = map.Height / 2;

        // Executa o algoritmo a partir do centro
        Generate(map, centerCol, centerLine);
    }

    private void Update()
    {
        // Verifica se a tecla 'R' foi pressionada
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartGeneration();
        }
    }

    public void Generate(Map map, int startCol, int startLine)
    {
        if (map == null)
        {
            Debug.LogError("DynamicEnergyPropagation: O parâmetro 'map' está nulo!");
            return;
        }

        Tile startTile = map.GetTile(startCol, startLine);
        if (startTile == null)
        {
            Debug.LogError($"DynamicEnergyPropagation: Posição inicial ({startCol}, {startLine}) fora dos limites do mapa!");
            return;
        }

        // Inicia a propagação utilizando a energia configurada
        PropagateEnergy(startTile, initialEnergy);

        // Atualiza quais tiles passam a ser bordas após essa expansão
        UpdateAllBorders(map);
    }

    /// <summary>
    /// Função recursiva de propagação de energia sob demanda.
    /// </summary>
    private int PropagateEnergy(Tile tile, int energy)
    {
        // 1. Verificações de parada: Tile inexistente ou energia esgotada
        if (tile == null || energy <= 0)
        {
            return 0;
        }

        // 2. Teste de fechamento de grade (CLOSE_GRID) para evitar ciclos indesejados
        if (tile.isVisited && Random.value < closeGrid)
        {
            return 0;
        }

        // 3. Revela o tile e marca como visitado
        tile.isVisited = true;
        tile.SetVisibility(true);

        int currentLocalEnergy = energy;
        int[] neighborEnergies = new int[6];

        // 4. Propagação para as 6 direções hexagonais
        for (int dir = 0; dir < 6; dir++)
        {
            Tile neighbor = tile.neighbors[dir];

            if (neighbor == null) continue;

            // Tenta propagar
            neighborEnergies[dir] = PropagateEnergy(neighbor, energy - 1);

            // Conecta SOMENTE se o vizinho processou energia E se ele realmente ficou visível
            if (neighborEnergies[dir] > 0 && neighbor.isVisible)
            {
                tile.OpenPath(dir);

                int oppositeDir = (dir + 3) % 6;
                neighbor.OpenPath(oppositeDir);
            }
        }

        // 5. Determina o nível máximo de energia retornado pelos vizinhos
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

    /// <summary>
    /// Atualiza o estado isBorder em todos os tiles do mapa.
    /// </summary>
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

        // Reseta o mapa e recria os tiles limpos
        map.ResetMap();

        // Recompula o centro e gera novamente
        int centerCol = map.Width / 2;
        int centerLine = map.Height / 2;

        Generate(map, centerCol, centerLine);
    }
}