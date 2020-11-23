using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Biome : IBiome
{

    public string Name { get; private set; }
    public int Index { get; private set; }

    [SerializeField]
    public List<Transform> Massives { get; private set; }
    public Biome(string name, int index)
    {
        this.Name = name;
        this.Index = index;
    }

    public bool PreflightCheck()
    {
        if (this.Name == null ||
            this.Massives.Count < Enum.GetNames(typeof(Patterns.Pattern)).Length)
        {
            return false;
        }
        return true;
    }

    public void AddMassives(Transform[] massives)
    {
        Massives.AddRange(massives);
    }

    public Transform GetMassive(Patterns.Pattern pattern)
    {
        return Massives[(int)pattern];
    }
}

public interface IBiome
{
    bool PreflightCheck();
    void AddMassives(Transform[] massives);
    Transform GetMassive(Patterns.Pattern pattern);
}