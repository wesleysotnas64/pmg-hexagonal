using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    // Instância Singleton
    public static BiomeManager Instance { get; private set; }

    private Dictionary<BiomeType, int> biomeWeights = new Dictionary<BiomeType, int>();

    private void Awake()
    {
        // Garante que apenas uma instância exista na cena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeBiomeWeights();
    }

    private void InitializeBiomeWeights()
    {
        biomeWeights.Clear();

        BiomeType[] availableBiomes = new BiomeType[]
        {
            BiomeType.Forest,
            BiomeType.Lake,
            BiomeType.Plains,
            BiomeType.Desert,
            BiomeType.Swamp,
            BiomeType.Mountain,
            BiomeType.Tundra,
            BiomeType.Jungle,
            BiomeType.Volcano,
            BiomeType.Wasteland
        };

        foreach (BiomeType biome in availableBiomes)
        {
            biomeWeights[biome] = 1;
        }
    }

    public void RegisterBiomeInteraction(BiomeType biome)
    {
        if (biomeWeights.ContainsKey(biome))
        {
            biomeWeights[biome]++;
        }
    }

    public BiomeType GetRandomBiome()
    {
        // 1. Calcula a soma total de todos os pesos da roleta
        int totalWeight = 0;
        foreach (var weight in biomeWeights.Values)
        {
            totalWeight += weight;
        }

        // Caso de segurança se a lista estiver vazia ou sem pesos
        if (totalWeight <= 0) return BiomeType.Plains;

        // 2. Sortear um número aleatório entre 0 (inclusive) e o peso total (exclusive)
        int randomPoint = Random.Range(0, totalWeight);

        // 3. Percorre a roleta acumulando os pesos até encontrar a fatia onde o ponto sorteado caiu
        int currentSum = 0;
        foreach (var kvp in biomeWeights)
        {
            currentSum += kvp.Value;

            if (randomPoint < currentSum)
            {
                return kvp.Key;
            }
        }

        // Retorno fallback por garantia (retorna o primeiro bioma registrado)
        return BiomeType.Plains;
    }
}