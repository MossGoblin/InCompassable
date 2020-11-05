using UnityEngine;

public class PlayerMovertest : MonoBehaviour
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

    // code injection

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    private Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
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

        transform.Rotate(0, horMove * turnSpeed, 0);

        //moveDirection = new Vector3(horMove, 0.0f, verMove);
        //moveDirection *= speed;
        //controller.Move(moveDirection * Time.deltaTime);
    }
}