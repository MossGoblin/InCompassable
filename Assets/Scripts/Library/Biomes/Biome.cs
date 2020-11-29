﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    [SerializeField] private LibraryBiomes.Biomes name;
    [SerializeField] private Color color;
    [SerializeField] private ElementSet[] elements; // one set for each element
 
        void Start()
    {
        CheckInventory();
    }

    private void CheckInventory()
    {
        int numberOfElements = LibraryElements.GetNumberOfElements();
        if (elements.Length < numberOfElements)
        {
            Debug.LogError($"Biome {name} does not define enough elements");
        }
    }

    public Transform GetElement(LibraryElements.Elements elementName)
    {
        int elementNumber = (int)elementName;
        return GetElement(elementNumber);
    }

    public Transform GetElement(int elementNumber)
    {
        return elements[elementNumber].GetElement();
    }
}
