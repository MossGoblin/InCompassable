using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomesLibrary : MonoBehaviour
{
    [SerializeField] private Transform[] biomes;
    public enum Biomes
    {
        StoneForest,
        DarkForest,
        EnchantedForest,
        Bog,
        FantasyRuins,
        AlienRuins
    }

     public Transform GetElement(int biome, int index)
    {
        return biomes[0].GetComponent<Biome>().GetElement(index); // DBG Only 1 biome so far
        // return biomes[biome].GetComponent<Biome>().GetElement(index);
    }
}
