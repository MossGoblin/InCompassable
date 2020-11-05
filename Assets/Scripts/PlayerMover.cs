using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraHolderTransform;
    public float initRotation = 0f;

    public bool rightPlayer;
    public float moveSpeed = 6f;
    public float turnSpeed = 8;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
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

        transform.Rotate(0, horMove * turnSpeed, 0);

    }
}