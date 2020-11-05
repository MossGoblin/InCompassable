using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float visibilityDistance;
    public Transform playerOne;
    public Transform playerTwo;

    // Start is called before the first frame update
    void Start()
    {
        playerOne = GameObject.Find("PlayerOne").transform;
        playerTwo = GameObject.Find("PlayerTwo").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerOne != null && playerTwo != null)
        {
            float distanceToOne = CheckDistance(playerOne);
            float distanceToTwo = CheckDistance(playerTwo);

            if (distanceToOne <= visibilityDistance || distanceToTwo <= visibilityDistance)
            {
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }
    }

    private float CheckDistance(Transform player)
    {
        float distance;

        float sqrtTwo = Mathf.Sqrt(2f);

        float distX = Mathf.Abs(player.position.x - transform.position.x);
        float distZ = Mathf.Abs(player.position.z - transform.position.z);

        float smaller = Mathf.Min(distX, distZ);
        float larger = Mathf.Min(distX, distZ);

        distance = smaller * sqrtTwo + (larger - smaller);

        return distance;
    }
}
