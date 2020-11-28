using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryBiomes : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform GetElement(int biome, int index)
    {
        return biomes[0].GetComponent<Biome>().GetElement(index); // DBG Only 1 biome so far
        // return biomes[biome].GetComponent<Biome>().GetElement(index);
    }
}
