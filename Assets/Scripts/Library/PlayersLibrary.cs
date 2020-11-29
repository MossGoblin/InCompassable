using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLibrary : MonoBehaviour
{

    [SerializeField] public Transform[] playerSets;

    public Transform GetSet(int setIndex)
    {
        return playerSets[setIndex];
    }
}
