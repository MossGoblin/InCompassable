using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Dictionary<Transform, bool> players;
    [SerializeField] private int setIndex;
    [SerializeField] private int activePlayers;

    void Awake()
    {
        // fill in the players dictionary - all 3 possible players from the chosen set
        PlayersLibrary library = FindObjectOfType<PlayersLibrary>();
        Transform playerSet = library.GetSet(setIndex);
        players = new Dictionary<Transform, bool>();

        foreach (Transform player in playerSet.GetComponent<PlayerSet>().GetAllPlayers())
        {
            players.Add(player, false);
        }

        // number of players in play
        activePlayers = 0;
    }

    public Transform GetRandomPlayer()
    {
        if (activePlayers >= players.Count)
        {
            Debug.LogError("No more inactive players to choose from");
            return null;
        }

        Transform player = null;
        bool playerNotFound = true;
        while (playerNotFound)
        {
            player = players.ElementAt(Random.Range(0, players.Count)).Key;
            if (players[player] == false)
            {
                players[player] = true;
                activePlayers++;
                playerNotFound = false;
            }
        }

        return player;
    }

}
