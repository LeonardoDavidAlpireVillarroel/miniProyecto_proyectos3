using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float jumpHeight = 2f;
    public float speed = 3f;
    public float rotationSpeed = 90f;
    public float gravity = -9.8f;
    public float jumpSpeed = 15f;

    private Vector3 velocity;
    private float hInput, vInput;

    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePos;

    private CharacterController characterController;
    private Vector3 moveVelocity;
    private Vector3 turnVelocity;
    [SerializeField]private float groundOffset;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        GetDirectionAndMove();
        ApplyGravity();
    }

    void GetDirectionAndMove()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        if (IsGrounded())
        {
            velocity.y = -2f; // Pequeño valor negativo para que no quede flotando
            moveVelocity = transform.forward * speed * vInput;
            turnVelocity = transform.up * rotationSpeed * hInput;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpSpeed;
            }
        }

        characterController.Move((moveVelocity + velocity) * Time.deltaTime);
        transform.Rotate(turnVelocity * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        if (characterController != null)
        {
            Gizmos.color = Color.red;
            spherePos = new Vector3(transform.position.x, transform.position.y - characterController.height / 2 + groundOffset, transform.position.z);
            Gizmos.DrawWireSphere(spherePos, characterController.radius - 0.05f);
        }
    }
    bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - characterController.height / 2 + groundOffset, transform.position.z);
        return Physics.CheckSphere(spherePos, characterController.radius - 0.05f, groundMask);
    }
}

