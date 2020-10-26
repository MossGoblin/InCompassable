﻿ using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{

    public CharacterController controller;
    public Transform cameraHolderTransform;
    public float currentRotation;
    public float initRotation = 0f;
    private Vector3 initPlayerRotation;
    public float rotationSpeed;


    public bool rightPlayer;
    public float moveSpeed = 6f;
    public float turnSpeed = 8;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        currentRotation = initRotation;
        initPlayerRotation = transform.rotation.eulerAngles;
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horMove;
        float verMove;
        if (rightPlayer)
        {
            horMove = Input.GetAxis("HorizontalRight");
            verMove = Input.GetAxis("VerticalRight");
        }
        else
        {
            horMove = Input.GetAxis("HorizontalLeft");
            verMove = Input.GetAxis("VerticalLeft");
        }

        Vector3 move = transform.forward * verMove;
        controller.Move(moveSpeed * Time.deltaTime * move);

        transform.Rotate(0, horMove, 0);
        // camera rotation
        //cameraHolderTransform.Rotate(0, -horMove, 0);

    }
}
