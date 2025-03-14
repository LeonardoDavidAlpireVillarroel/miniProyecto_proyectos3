using UnityEngine;
using Unity.Cinemachine; // Aseg�rate de incluir el espacio de nombres de Cinemachine
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    //public float speed = 5f;
    //public float gravity = -9.8f;
    //public float jumpSpeed = 15f;
    //public float rotationSpeed = 100f; // Velocidad de rotaci�n vertical
    //public float verticalRotationLimit = 80f; // L�mite de rotaci�n vertical en grados

    //private Vector3 velocity;
    //private float hInput, vInput;
    //private float verticalRotation = 0f;

    //[SerializeField] private LayerMask groundMask;
    //private Vector3 spherePos;

    //private CharacterController characterController;
    //private Vector3 moveDirection;
    //[SerializeField] private float groundOffset;

    //public CinemachineCamera virtualCamera; // Variable p�blica para la c�mara virtual
    //private Animator characterAnimator;

    //void Awake()
    //{
    //    characterController = GetComponent<CharacterController>();
    //    characterAnimator = GetComponent<Animator>();
    //    Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor al centro de la pantalla
    //}

    //void Update()
    //{
    //    GetDirectionAndMove();
    //    ApplyGravity();
    //}

    //void GetDirectionAndMove()
    //{
    //    hInput = Input.GetAxis("Horizontal");
    //    vInput = Input.GetAxis("Vertical");


    //    float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 1.5f : speed; // Aumenta la velocidad al presionar Shift

    //    if (IsGrounded())
    //    {
    //        velocity.y = -2f;

    //        // Obtener la rotaci�n de la c�mara virtual
    //        if (virtualCamera != null)
    //        {
    //            Transform cam = virtualCamera.transform;

    //            // Rotaci�n horizontal del jugador
    //            transform.rotation = Quaternion.Euler(0, cam.eulerAngles.y, 0);

    //            // Rotaci�n vertical del jugador (limitada)
    //            verticalRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
    //            verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

    //            // Aplicar la rotaci�n vertical a la c�mara virtual
    //            cam.localEulerAngles = new Vector3(verticalRotation, cam.localEulerAngles.y, cam.localEulerAngles.z);

    //            // Movimiento en relaci�n con la direcci�n de la c�mara virtual
    //            Vector3 forward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
    //            Vector3 right = new Vector3(cam.right.x, 0, cam.right.z).normalized;

    //            moveDirection = (forward * vInput + right * hInput).normalized * currentSpeed;
    //            CharacterAnimations();

    //            if (Input.GetButtonDown("Jump"))
    //            {
    //                velocity.y = jumpSpeed;
    //            }
    //        }

    //        characterController.Move((moveDirection + velocity) * Time.deltaTime);

    //        // Verificar si el jugador se est� moviendo y actualizar la animaci�n

    //    }

    //}
    //void CharacterAnimations()
    //{
    //    if (moveDirection == Vector3.zero)
    //    {

    //        characterAnimator.SetFloat("speed", 0f);

    //        if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
    //        {
    //            characterAnimator.SetBool("isAiming", true);
    //        }
    //        else
    //        {
    //            characterAnimator.SetBool("isAiming", false);
    //        }
    //    }
    //    else if (Input.GetKey(KeyCode.LeftShift))
    //    {
    //        characterAnimator.SetFloat("speed", 2f);
    //        if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
    //        {
    //            characterAnimator.SetBool("isAiming", true);
    //        }
    //        else
    //        {
    //            characterAnimator.SetBool("isAiming", false);
    //        }
    //    }
    //    else
    //    {
    //        if (Input.GetKey(KeyCode.Mouse1)) // Click derecho del mouse
    //        {
    //            characterAnimator.SetBool("isAiming", true);
    //        }
    //        else
    //        {
    //            characterAnimator.SetBool("isAiming", false);
    //        }
    //        characterAnimator.SetFloat("speed", 1f);
    //    }
    //}

    //void ApplyGravity()
    //{
    //    if (!IsGrounded())
    //    {
    //        velocity.y += gravity * Time.deltaTime;
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    if (characterController != null)
    //    {
    //        Gizmos.color = Color.red;
    //        spherePos = new Vector3(transform.position.x, transform.position.y - characterController.height / 2 + groundOffset, transform.position.z);
    //        Gizmos.DrawWireSphere(spherePos, characterController.radius - 0.05f);
    //    }
    //}

    //bool IsGrounded()
    //{
    //    spherePos = new Vector3(transform.position.x, transform.position.y - characterController.height / 2 + groundOffset, transform.position.z);
    //    return Physics.CheckSphere(spherePos, characterController.radius - 0.05f, groundMask);
    //}

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float rotationSpeed = 50f;
    private Vector3 move;



    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;
    private Animator characterAnimator;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
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