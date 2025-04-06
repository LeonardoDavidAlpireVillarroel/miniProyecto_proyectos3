using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

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
    [SerializeField] public Transform barrelTransform;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private ParticleSystem explosionParticle;

    [Header("Ammo Settings")]
    [SerializeField] private int maxRifleAmmo = 50;
    private int currentRifleAmmo;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Health Settings")]
    [SerializeField] private Healthbar _healthbar;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject rifle;
    [SerializeField] private int currentWeapon = 0; // 0 = Pistola, 1 = Rifle
    [SerializeField] private float throwForce = 40f;
    [SerializeField] GameObject grenadePrefab;

    [Header("Grenade Settings")]
    [SerializeField] private int maxGrenades = 1; // Máximo de granadas que el jugador puede tener
    private int currentGrenades; // Granadas actuales
    [SerializeField] private TextMeshProUGUI grenadeText; // UI de las granadas
    [SerializeField] private TextMeshProUGUI grenadeCooldownText; // UI del temporizador de recarga

    private float grenadeCooldown = 60f; // Tiempo de recarga en segundos
    private float currentCooldown; // Temporizador para la recarga de las granadas
    private bool isCooldownActive = false;

    [Header("UI Derrota")]
    [SerializeField] private GameObject gameOverPanel;


    [SerializeField] private GameObject cameras;


    private CharacterController controller;
    private PlayerInput playerInput;
    private Animator characterAnimator;
    private Transform cameraTransform;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool isShooting;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;

    private void Awake()
    {
        // Component initialization
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;

        // Initialize ammo
        currentRifleAmmo = maxRifleAmmo;
        UpdateAmmoUI();

        // Setup input actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];

        currentGrenades = maxGrenades;
        UpdateGrenadeUI();

        currentCooldown = grenadeCooldown;
        isCooldownActive = false;

        grenadeCooldownText.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Event subscriptions for shooting
        shootAction.performed += StartShooting;
        shootAction.canceled += StopShooting;
    }

    private void OnDisable()
    {
        // Event unsubscriptions for shooting
        shootAction.performed -= StartShooting;
        shootAction.canceled -= StopShooting;
    }

    // Corrección: Métodos deben aceptar 'InputAction.CallbackContext' como parámetro y ser 'void'
    private void StartShooting(InputAction.CallbackContext context)
    {

        if (currentWeapon == 0) // Pistola
        {
            ShootSingleShot();
        }
        else if (currentWeapon == 1 && Input.GetKey(KeyCode.Mouse1) && currentRifleAmmo > 0) // Rifle
        {
            if (!isShooting)
            {

                isShooting = true;
                InvokeRepeating(nameof(ShootSingleShot), 0f, 0.1f);
            }
        }
    }

    private void StopShooting(InputAction.CallbackContext context)
    {
        if (currentWeapon == 1)
        {
            isShooting = false;
            CancelInvoke(nameof(ShootSingleShot));
        }
    }


    private void Update()
    {
        // Llamar a los otros métodos de actualización
        HandleMovement();
        HandleJump();
        HandleWeaponSwitch();
        ThrowGrenade();

        // Lógica de recarga de granadas con temporizador visible
        if (isCooldownActive)
        {
            currentCooldown -= Time.deltaTime;

            // Mostrar el temporizador en la UI
            grenadeCooldownText.text = $"Recarga: {Mathf.Ceil(currentCooldown)}s";

            if (currentCooldown <= 0f)
            {
                currentCooldown = grenadeCooldown;

                // Recargar granadas de una en una
                if (currentGrenades < maxGrenades)
                {
                    currentGrenades++;
                    UpdateGrenadeUI();
                }

                if (currentGrenades == maxGrenades)
                {
                    isCooldownActive = false; // Desactivar el temporizador si ya están llenas
                    grenadeCooldownText.gameObject.SetActive(false); // Ocultar el temporizador
                }
                else
                {
                    // Reiniciar temporizador para la próxima recarga si aún falta recargar más
                    currentCooldown = grenadeCooldown;
                }
            }
        }
    }

    private void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        // Movement input
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = cameraTransform.right * input.x + cameraTransform.forward * input.y;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);

        // Update character animations
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
        UpdateAmmoUI(); // Update UI when switching weapons
    }

    private void ShootSingleShot()
    {
        SoundManager.Instance.PlaySound3D("Shoot", transform.position);

        if (!Input.GetKey(KeyCode.Mouse1)) return;

        if (currentWeapon == 1 && currentRifleAmmo > 0) // Rifle with ammo
        {
            currentRifleAmmo--;
            UpdateAmmoUI(); // Update UI after shooting
        }
        else if (currentWeapon == 1 && currentRifleAmmo == 0) // Rifle without ammo
        {
            return; // Don't shoot if no ammo
        }

        // Instantiate bullet and set its direction
        if (barrelTransform == null || barrelTransform.gameObject == null)
        {
            Debug.LogError("barrelTransform ha sido destruido o no está asignado.");
            return; // Evita que el código continúe si el objeto ya no existe
        }

        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        Vector3 shootDirection = GetShootDirection();
        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);

    }

    public void AddRifleAmmo(int amount)
    {
        currentRifleAmmo = Mathf.Min(currentRifleAmmo + amount, maxRifleAmmo);
        UpdateAmmoUI(); // Update UI after picking up ammo
    }

    private Vector3 GetShootDirection()
    {
        RaycastHit hit;
        Vector3 shootDirection;

        // Calculate shoot direction based on raycast
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

        // Update animations for different weapon states
        if (currentWeapon == 0) // Pistola
        {
            gun.SetActive(true);
            rifle.SetActive(false);
            characterAnimator.SetFloat("rifleSpeed", 0);
            characterAnimator.SetBool("rifleAiming", false);
            characterAnimator.SetFloat("speed", movementSpeed);
            characterAnimator.SetBool("isAiming", isAiming);
        }
        else // Rifle
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
            SoundManager.Instance.PlaySound3D("Explosion", transform.position);

            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            Debug.Log(GameManager.gameManager._playerHealth.Health);
        }

    }

    private void PlayerTakeDmg(int dmg)
    {
        //GameManager.gameManager._playerHealth.DmgUnit(dmg);
        //_healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);
        GameManager.gameManager._playerHealth.DmgUnit(dmg);
        _healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);

        if (GameManager.gameManager._playerHealth.Health <= 0)
        {
            TriggerGameOver();
            cameras.SetActive(false);
        }
    }

    private void PlayerHeal(int healing)
    {
        GameManager.gameManager._playerHealth.HealUnit(healing);
        _healthbar.SetHealth(GameManager.gameManager._playerHealth.Health);
    }

    public bool IsRifleAmmoFull()
    {
        return currentRifleAmmo >= maxRifleAmmo;
    }

    private void UpdateAmmoUI()
    {
        // Update ammo UI based on the selected weapon
        if (currentWeapon == 0) // Pistola (infinite ammo)
        {
            ammoText.text = "∞/∞";
        }
        else if (currentWeapon == 1) // Rifle (limited ammo)
        {
            ammoText.text = $"{currentRifleAmmo}/{maxRifleAmmo}";
        }
    }
    private void ThrowGrenade()
    {
        if (Input.GetKeyDown(KeyCode.Q) && currentGrenades > 0)
        {
            // Lanzar granada
            GameObject grenade = Instantiate(grenadePrefab, barrelTransform.position, Quaternion.identity, bulletParent);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);

            currentGrenades--; // Decrementar granadas disponibles
            UpdateGrenadeUI(); // Actualizar UI

            if (!isCooldownActive)
            {
                isCooldownActive = true; // Activar el temporizador de recarga
                currentCooldown = grenadeCooldown;
                grenadeCooldownText.gameObject.SetActive(true); // Mostrar el temporizador en la UI
            }
        }
    }
    private void UpdateGrenadeUI()
    {
        grenadeText.text = $"{currentGrenades}/{maxGrenades}"; // Muestra las granadas disponibles en la interfaz
    }


    private void TriggerGameOver()
    {
        gameOverPanel.SetActive(true); // Mostrar el panel
        Time.timeScale = 0f; // Pausar el juego
        Cursor.lockState = CursorLockMode.None; // Mostrar cursor
        Cursor.visible = true;
    }
}