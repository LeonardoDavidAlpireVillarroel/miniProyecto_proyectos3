using UnityEngine;
using Unity.Cinemachine; // Asegúrate de incluir el espacio de nombres de Cinemachine
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 50f;
    private Vector3 move;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Transform barrelTransform;
    [SerializeField]
    private Transform bulletParent;
    private LayerMask collisionLayers;



    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;
    private Animator characterAnimator;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => ShootGun();
    }
    private void ShootGun()
    {
        if (!Input.GetKey(KeyCode.Mouse1)) return; // Si no está presionando el botón derecho, no dispara.

        // Instancia la bala en la posición del barril con la rotación de la cámara
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();

        if (bulletController != null)
        {
            // Determinar la dirección del disparo con un Raycast
            RaycastHit hit;
            Vector3 shootDirection;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, collisionLayers))
            {
                shootDirection = (hit.point - barrelTransform.position).normalized;
            }
            else
            {
                shootDirection = cameraTransform.forward;
            }

            // Rotar la bala en la dirección del disparo
            bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
        }
    }

    void Update()
    {
        MoveCharacter();
        Jump();
    }

    void MoveCharacter()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        move = new Vector3(input.x, 0, input.y);
        move = (cameraTransform.right * input.x) + (cameraTransform.forward * input.y);
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        CharacterAnimations();

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void CharacterAnimations()
    {
        if (move == Vector3.zero)
        {

            characterAnimator.SetFloat("speed", 0f);

            if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
            {
                characterAnimator.SetBool("isAiming", true);
            }
            else
            {
                characterAnimator.SetBool("isAiming", false);
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            characterAnimator.SetFloat("speed", 2f);
            if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
            {
                characterAnimator.SetBool("isAiming", true);
            }
            else
            {
                characterAnimator.SetBool("isAiming", false);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
            {
                characterAnimator.SetBool("isAiming", true);
            }
            else
            {
                characterAnimator.SetBool("isAiming", false);
            }
            characterAnimator.SetFloat("speed", 1f);
        }
    }
}