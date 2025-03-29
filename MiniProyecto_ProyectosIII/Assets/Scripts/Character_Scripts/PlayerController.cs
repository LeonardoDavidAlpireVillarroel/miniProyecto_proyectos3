//using UnityEngine;
//using Unity.Cinemachine; // Asegúrate de incluir el espacio de nombres de Cinemachine
//using UnityEngine.InputSystem;
//using System;

//[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
//public class PlayerController : MonoBehaviour
//{
//    [SerializeField]
//    private float playerSpeed = 2.0f;
//    [SerializeField]
//    private float jumpHeight = 1.0f;
//    [SerializeField]
//    private float gravityValue = -9.81f;
//    [SerializeField]
//    private float rotationSpeed = 50f;
//    private Vector3 move;
//    [SerializeField]
//    private GameObject bulletPrefab;
//    [SerializeField]
//    private Transform barrelTransform;
//    [SerializeField]
//    private Transform bulletParent;
//    private LayerMask collisionLayers;
//    [SerializeField]
//    private int currentWeapon = 0;


//    private CharacterController controller;
//    private PlayerInput playerInput;
//    private Vector3 playerVelocity;
//    private bool groundedPlayer;
//    private Transform cameraTransform;
//    private Animator characterAnimator;

//    private InputAction moveAction;
//    private InputAction jumpAction;
//    private InputAction shootAction;

//    private void Awake()
//    {
//        controller = GetComponent<CharacterController>();
//        playerInput = GetComponent<PlayerInput>();
//        characterAnimator = GetComponent<Animator>();
//        cameraTransform = Camera.main.transform;
//        moveAction = playerInput.actions["Move"];
//        jumpAction = playerInput.actions["Jump"];
//        shootAction = playerInput.actions["Shoot"];
//    }

//    private void OnEnable()
//    {
//        shootAction.performed += _ => ShootGun();
//    }

//    private void OnDisable()
//    {
//        shootAction.performed -= _ => ShootGun();
//    }
//    private void ShootGun()
//    {
//        if (!Input.GetKey(KeyCode.Mouse1)) return; // Si no está presionando el botón derecho, no dispara.

//        // Instancia la bala en la posición del barril con la rotación de la cámara
//        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
//        BulletController bulletController = bullet.GetComponent<BulletController>();

//        if (bulletController != null)
//        {
//            // Determinar la dirección del disparo con un Raycast
//            RaycastHit hit;
//            Vector3 shootDirection;

//            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, collisionLayers))
//            {
//                shootDirection = (hit.point - barrelTransform.position).normalized;
//            }
//            else
//            {
//                shootDirection = cameraTransform.forward;
//            }

//            // Rotar la bala en la dirección del disparo
//            bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
//        }
//    }

//    void Update()
//    {
//        MoveCharacter();
//        Jump();
//        HandleWeaponSwitch();
//    }

//    void MoveCharacter()
//    {
//        groundedPlayer = controller.isGrounded;
//        if (groundedPlayer && playerVelocity.y < 0)
//        {
//            playerVelocity.y = 0f;
//        }

//        Vector2 input = moveAction.ReadValue<Vector2>();
//        move = new Vector3(input.x, 0, input.y);
//        move = (cameraTransform.right * input.x) + (cameraTransform.forward * input.y);
//        move.y = 0f;
//        controller.Move(move * Time.deltaTime * playerSpeed);

//        CharacterAnimations();

//        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
//        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//    }

//    void Jump()
//    {
//        if (jumpAction.triggered && groundedPlayer)
//        {
//            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
//        }

//        playerVelocity.y += gravityValue * Time.deltaTime;
//        controller.Move(playerVelocity * Time.deltaTime);
//    }

//    void HandleWeaponSwitch()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            currentWeapon = 0; // Equipar Gun
//            characterAnimator.SetInteger("weaponType", 0);
//        }
//        else if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            currentWeapon = 1; // Equipar Rifle
//            characterAnimator.SetInteger("weaponType", 1);
//        }
//    }
//    void CharacterAnimations()
//    {
//        if (currentWeapon == 0)
//        {
//            characterAnimator.SetFloat("rifleSpeed", 0f);
//            characterAnimator.SetBool("rifleAiming", false);

//            if (move == Vector3.zero)
//            {
//                characterAnimator.SetFloat("speed", 0f);

//                if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
//                {
//                    characterAnimator.SetBool("isAiming", true);
//                }
//                else
//                {
//                    characterAnimator.SetBool("isAiming", false);
//                }
//            }
//            else if (Input.GetKey(KeyCode.LeftShift))
//            {
//                characterAnimator.SetFloat("speed", 2f);
//                if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
//                {
//                    characterAnimator.SetBool("isAiming", true);
//                }
//                else
//                {
//                    characterAnimator.SetBool("isAiming", false);
//                }
//            }
//            else
//            {
//                if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
//                {
//                    characterAnimator.SetBool("isAiming", true);
//                }
//                else
//                {
//                    characterAnimator.SetBool("isAiming", false);
//                }
//                characterAnimator.SetFloat("speed", 1f);
//            }
//        }
//        else 
//        {
//            characterAnimator.SetFloat("speed", 0f);
//            characterAnimator.SetBool("isAiming", false);

//            if (move == Vector3.zero)
//            {
//                characterAnimator.SetFloat("rifleSpeed", 0f);

//                if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
//                {
//                    characterAnimator.SetBool("rifleAiming", true);
//                }
//                else
//                {
//                    characterAnimator.SetBool("rifleAiming", false);
//                }
//            }
//            else if (Input.GetKey(KeyCode.LeftShift))
//            {
//                characterAnimator.SetFloat("rifleSpeed", 2f);
//                if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
//                {
//                    characterAnimator.SetBool("rifleAiming", true);
//                }
//                else
//                {
//                    characterAnimator.SetBool("rifleAiming", false);
//                }
//            }
//            else
//            {
//                if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
//                {
//                    characterAnimator.SetBool("rifleAiming", true);
//                }
//                else
//                {
//                    characterAnimator.SetBool("rifleAiming", false);
//                }
//                characterAnimator.SetFloat("rifleSpeed", 1f);
//            }
//        }
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private LayerMask collisionLayers;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Animator characterAnimator;
    private Transform cameraTransform;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField]
    private int currentWeapon = 0;
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject rifle;

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

    private void OnEnable() => shootAction.performed += _ => ShootGun();
    private void OnDisable() => shootAction.performed -= _ => ShootGun();

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleWeaponSwitch();
    }

    private void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = cameraTransform.right * input.x + cameraTransform.forward * input.y;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);

        UpdateCharacterAnimations(move, input);
    }

    private void HandleJump()
    {
        if (jumpAction.triggered && groundedPlayer)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = 1;
        characterAnimator.SetInteger("weaponType", currentWeapon);
    }

    private void ShootGun()
    {
        if (!Input.GetKey(KeyCode.Mouse1)) return;
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        Vector3 shootDirection = GetShootDirection();
        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
    }

    //private Vector3 GetShootDirection()
    //{
    //    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, Mathf.Infinity, collisionLayers))
    //        return (hit.point - barrelTransform.position).normalized;
    //    return cameraTransform.forward;
    //}
    private Vector3 GetShootDirection()
    {
        RaycastHit hit;
        Vector3 shootDirection;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, collisionLayers))
        {
            // Verifica si el punto de impacto está detrás del barril del arma
            Vector3 hitDirection = (hit.point - barrelTransform.position).normalized;
            float dotProduct = Vector3.Dot(hitDirection, cameraTransform.forward);

            if (dotProduct > 0) // Solo usa el punto si está al frente del jugador
            {
                shootDirection = hitDirection;
            }
            else
            {
                shootDirection = cameraTransform.forward; // Usa la dirección de la cámara si el punto es erróneo
            }
        }
        else
        {
            shootDirection = cameraTransform.forward; // Si no hay impacto, usa la dirección de la cámara
        }

        return shootDirection;
    }

    private void UpdateCharacterAnimations(Vector3 move, Vector2 input)
    {
        bool isAiming = Input.GetKey(KeyCode.Mouse1);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float movementSpeed = (move == Vector3.zero) ? 0f : (isRunning ? 3f : 1f);

        if (currentWeapon == 0)
        {
            gun.SetActive(true);
            rifle.SetActive(false);
            characterAnimator.SetFloat("rifleSpeed", 0);
            characterAnimator.SetBool("rifleAiming", false);
            characterAnimator.SetFloat("speed", movementSpeed);
            characterAnimator.SetBool("isAiming", isAiming);

        }
        else
        {
            gun.SetActive(false);
            rifle.SetActive(true);
            characterAnimator.SetFloat("speed", 0);
            characterAnimator.SetBool("isAiming", false);
            characterAnimator.SetFloat("rifleSpeed", movementSpeed);
            characterAnimator.SetBool("rifleAiming", isAiming);
        }
    }
}
