using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSet : MonoBehaviour
{
    [SerializeField] private Transform[] transforms;

    public Transform GetElement()
    {
        int index = Random.Range(0, transforms.Length);
        return transforms[index];
    }
}
