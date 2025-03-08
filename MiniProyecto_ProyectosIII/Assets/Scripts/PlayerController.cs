using UnityEngine;
using Unity.Cinemachine; // Aseg�rate de incluir el espacio de nombres de Cinemachine

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public float gravity = -9.8f;
    public float jumpSpeed = 15f;
    public float rotationSpeed = 100f; // Velocidad de rotaci�n vertical
    public float verticalRotationLimit = 80f; // L�mite de rotaci�n vertical en grados

    private Vector3 velocity;
    private float hInput, vInput;
    private float verticalRotation = 0f;

    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePos;

    private CharacterController characterController;
    private Vector3 moveDirection;
    [SerializeField] private float groundOffset;

    public CinemachineCamera virtualCamera; // Variable p�blica para la c�mara virtual

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor al centro de la pantalla
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
            velocity.y = -2f;

            // Obtener la rotaci�n de la c�mara virtual
            if (virtualCamera != null)
            {
                Transform cam = virtualCamera.transform;

                // Rotaci�n horizontal del jugador
                transform.rotation = Quaternion.Euler(0, cam.eulerAngles.y, 0);

                // Rotaci�n vertical del jugador (limitada)
                verticalRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
                verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

                // Aplicar la rotaci�n vertical a la c�mara virtual
                cam.localEulerAngles = new Vector3(verticalRotation, cam.localEulerAngles.y, cam.localEulerAngles.z);

                // Movimiento en relaci�n con la direcci�n de la c�mara virtual
                Vector3 forward = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
                Vector3 right = new Vector3(cam.right.x, 0, cam.right.z).normalized;

                moveDirection = (forward * vInput + right * hInput).normalized * speed;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpSpeed;
            }
        }

        characterController.Move((moveDirection + velocity) * Time.deltaTime);
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