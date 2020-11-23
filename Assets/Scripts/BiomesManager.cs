using System;
using System.Collections.Generic;
using UnityEngine;

public class BiomesManager
{
    public List<Biome> Biomes { get; private set; }

    public BiomesManager(BiomeDefinitions definitions)
    {
        int biomeTypesCount = Enum.GetNames(typeof(BiomeDefinitions.Biomes)).Length;
        
        for (int count = 0; count < biomeTypesCount; count ++)
        {
            string biomeName = ((BiomeDefinitions.Biomes)count).ToString();
            Biome newBiome = new Biome(biomeName, count);

            newBiome.AddMassives(definitions.massivesList[count]);

            Biomes.Add(newBiome);
        }
    }
}
