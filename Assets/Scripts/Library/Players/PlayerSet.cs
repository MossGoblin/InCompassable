using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSet : MonoBehaviour
{
    [SerializeField] private Transform[] players;

    public Transform GetPlayer(int index)
    {
        return players[index];
    }

    public Transform[] GetAllPlayers()
    {
        return players;
    }
}
