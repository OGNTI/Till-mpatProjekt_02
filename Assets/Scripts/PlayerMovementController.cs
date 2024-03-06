using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class MovementController : MonoBehaviour
{
    Vector2 inputVector = Vector2.zero;

    CharacterController characterController;

    [SerializeField] float speed = 5;

    float velocityY = 0;

    bool jumpPressed = false;

    [SerializeField] float jumpPower = 8;

    [SerializeField] float gravityMultiplier = 3;

    void Awake()
    {
        characterController = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
        movement *= speed;

        if (characterController.isGrounded)
        {
            velocityY = -1f;

            if (jumpPressed)
            {
                velocityY = jumpPower;
            }
        }

        velocityY += Physics.gravity.y * gravityMultiplier;
        movement.y = velocityY;

        characterController.Move(movement * Time.deltaTime);

        jumpPressed = false;
    }

    void OnMove(InputValue value) => inputVector = value.Get<Vector2>();

    void OnJump(InputValue value) => jumpPressed = true;
}
