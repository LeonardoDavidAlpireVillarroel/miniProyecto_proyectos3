//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("Movement Settings")]
//    [SerializeField] private float playerSpeed = 2.0f;
//    [SerializeField] private float jumpHeight = 1.0f;
//    [SerializeField] private float gravityValue = -9.81f;
//    [SerializeField] private float rotationSpeed = 50f;

//    [Header("Shooting Settings")]
//    [SerializeField] private GameObject bulletPrefab;
//    [SerializeField] private Transform barrelTransform;
//    [SerializeField] private Transform bulletParent;
//    [SerializeField] private LayerMask collisionLayers;
//    [SerializeField] private ParticleSystem explosionParticle;

//    [SerializeField] Healthbar _healthbar;

//    private CharacterController controller;
//    private PlayerInput playerInput;
//    private Animator characterAnimator;
//    private Transform cameraTransform;

//    private Vector3 playerVelocity;
//    private bool groundedPlayer;
//    [SerializeField]
//    private int currentWeapon = 0;
//    [SerializeField]
//    private GameObject gun;
//    [SerializeField]
//    private GameObject rifle;

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

//    private void OnEnable() => shootAction.performed += _ => ShootGun();
//    private void OnDisable() => shootAction.performed -= _ => ShootGun();

//    private void Update()
//    {
//        HandleMovement();
//        HandleJump();
//        HandleWeaponSwitch();
//    }

//    private void HandleMovement()
//    {
//        groundedPlayer = controller.isGrounded;
//        if (groundedPlayer && playerVelocity.y < 0)
//            playerVelocity.y = 0f;

//        Vector2 input = moveAction.ReadValue<Vector2>();
//        Vector3 move = cameraTransform.right * input.x + cameraTransform.forward * input.y;
//        move.y = 0f;
//        controller.Move(move * Time.deltaTime * playerSpeed);
//        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);

//        UpdateCharacterAnimations(move, input);
//    }

//    private void HandleJump()
//    {
//        if (jumpAction.triggered && groundedPlayer)
//            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);

//        playerVelocity.y += gravityValue * Time.deltaTime;
//        controller.Move(playerVelocity * Time.deltaTime);
//    }

//    private void HandleWeaponSwitch()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeapon = 0;
//        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeapon = 1;
//        characterAnimator.SetInteger("weaponType", currentWeapon);
//    }

//    private void ShootGun()
//    {
//        if (!Input.GetKey(KeyCode.Mouse1)) return;
//        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
//        Vector3 shootDirection = GetShootDirection();
//        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
//    }

//    private Vector3 GetShootDirection()
//    {
//        RaycastHit hit;
//        Vector3 shootDirection;

//        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, collisionLayers))
//        {
//            // Verifica si el punto de impacto está detrás del barril del arma
//            Vector3 hitDirection = (hit.point - barrelTransform.position).normalized;
//            float dotProduct = Vector3.Dot(hitDirection, cameraTransform.forward);

//            if (dotProduct > 0) // Solo usa el punto si está al frente del jugador
//            {
//                shootDirection = hitDirection;
//            }
//            else
//            {
//                shootDirection = cameraTransform.forward; // Usa la dirección de la cámara si el punto es erróneo
//            }
//        }
//        else
//        {
//            shootDirection = cameraTransform.forward; // Si no hay impacto, usa la dirección de la cámara
//        }

//        return shootDirection;
//    }

//    private void UpdateCharacterAnimations(Vector3 move, Vector2 input)
//    {
//        bool isAiming = Input.GetKey(KeyCode.Mouse1);
//        bool isRunning = Input.GetKey(KeyCode.LeftShift);
//        float movementSpeed = (move == Vector3.zero) ? 0f : (isRunning ? 3f : 1f);

//        if (currentWeapon == 0)
//        {
//            gun.SetActive(true);
//            rifle.SetActive(false);
//            characterAnimator.SetFloat("rifleSpeed", 0);
//            characterAnimator.SetBool("rifleAiming", false);
//            characterAnimator.SetFloat("speed", movementSpeed);
//            characterAnimator.SetBool("isAiming", isAiming);

//        }
//        else
//        {
//            gun.SetActive(false);
//            rifle.SetActive(true);
//            characterAnimator.SetFloat("speed", 0);
//            characterAnimator.SetBool("isAiming", false);
//            characterAnimator.SetFloat("rifleSpeed", movementSpeed);
//            characterAnimator.SetBool("rifleAiming", isAiming);
//        }
//    }
//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.CompareTag("Enemy"))
//        {
//            PlayerTakeDmg(25);
//            Instantiate(explosionParticle, transform.position, Quaternion.identity);
//            Debug.Log(GameManager.gameManager._playerHealth.Health);
//        }
//    }

//    private void PlayerTakeDmg(int dmg)
//    {
//        GameManager.gameManager._playerHealth.DmgUnit(dmg);
//        _healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);
//    }
//    private void PlayerHeal(int healing)
//    {
//        GameManager.gameManager._playerHealth.HealUnit(healing);
//        _healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);
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
    [SerializeField] private ParticleSystem explosionParticle;

    [SerializeField] private Healthbar _healthbar;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Animator characterAnimator;
    private Transform cameraTransform;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private int currentWeapon = 0; // 0 = Pistola, 1 = Rifle
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject rifle;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private bool isShooting; // Control del disparo automático

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
        shootAction.performed += _ => StartShooting();
        shootAction.canceled += _ => StopShooting();
    }

    private void OnDisable()
    {
        shootAction.performed -= _ => StartShooting();
        shootAction.canceled -= _ => StopShooting();
    }

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

    private void StartShooting()
    {
        if (currentWeapon == 0) // Pistola: Disparo único
        {
            ShootSingleShot();
        }
        else if (currentWeapon == 1 && Input.GetKey(KeyCode.Mouse1)) // Rifle: Disparo continuo si apunta
        {
            if (!isShooting)
            {
                isShooting = true;
                InvokeRepeating(nameof(ShootSingleShot), 0f, 0.1f); // Disparo automático cada 0.1s
            }
        }
    }

    private void StopShooting()
    {
        if (currentWeapon == 1)
        {
            isShooting = false;
            CancelInvoke(nameof(ShootSingleShot)); // Detiene el disparo automático del rifle
        }
    }

    private void ShootSingleShot()
    {
        if (!Input.GetKey(KeyCode.Mouse1)) return; // Disparo solo si está apuntando

        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        Vector3 shootDirection = GetShootDirection();
        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
    }

    private Vector3 GetShootDirection()
    {
        RaycastHit hit;
        Vector3 shootDirection;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity, collisionLayers))
        {
            Vector3 hitDirection = (hit.point - barrelTransform.position).normalized;
            float dotProduct = Vector3.Dot(hitDirection, cameraTransform.forward);

            shootDirection = (dotProduct > 0) ? hitDirection : cameraTransform.forward;
        }
        else
        {
            shootDirection = cameraTransform.forward;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            PlayerTakeDmg(25);
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }
    }

    private void PlayerTakeDmg(int dmg)
    {
        GameManager.gameManager._playerHealth.DmgUnit(dmg);
        _healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);
    }

    private void PlayerHeal(int healing)
    {
        GameManager.gameManager._playerHealth.HealUnit(healing);
        _healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);
    }
}
