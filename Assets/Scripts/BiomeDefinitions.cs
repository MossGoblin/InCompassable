using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDefinitions : MonoBehaviour
{
    [Header("Massives")]
    public List<Transform[]> massivesList; // NOTE should be initialized and filled in in the inspector
    public enum Biomes
    {
        DarkForest,
        Stone,
        Bog,
        EnchantedForest,
        Ruins
    }
}
