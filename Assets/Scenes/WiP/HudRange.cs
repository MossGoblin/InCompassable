using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudRange : MonoBehaviour
{
    public Camera hudCamera;
    [Range(0f, 1f)]
    public float hudBuffer = 0f;

    public Transform target;
    public Transform marker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckVisibility();
    }

        private void CheckVisibility()
    {
            Vector3 screenPoint = hudCamera.WorldToViewportPoint(target.position); // DBG NOT GETTING CORRECT VECTOR MAYBE
            bool onScreen = screenPoint.x > (0 + hudBuffer) 
                         && screenPoint.x < (1 - hudBuffer) 
                         && screenPoint.y > (0 + hudBuffer) 
                         && screenPoint.y < (1 - hudBuffer);


            Debug.Log($"x: {screenPoint.x} - y: {screenPoint.y} - z {screenPoint.z}");

            if (onScreen)
            {
                marker.gameObject.SetActive(false);
            }
            else
            {
                marker.gameObject.SetActive(true);
            }
    }
}
