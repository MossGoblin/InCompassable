using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schmetterling : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float newPositionX = transform.position.x + moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);
    }
}
