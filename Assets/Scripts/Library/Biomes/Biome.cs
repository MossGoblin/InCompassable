using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    [SerializeField] private LibraryBiomes.Biomes name;
    [SerializeField] private Color color;
    [SerializeField] private Transform[] elements;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetElement(int index)
    {
        return elements[index];
    }

    public Transform GetElement(LibraryElements.Elements elementName)
    {
        return elements[(int)elementName];
    }
}
