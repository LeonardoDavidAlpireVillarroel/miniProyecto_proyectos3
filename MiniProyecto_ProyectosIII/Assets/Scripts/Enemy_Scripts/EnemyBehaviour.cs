using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] public float chaseSpeed = 5f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float hearRadius = 5f;
    [SerializeField] private LayerMask targetPlayer;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Transform player;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private GameObject ammoDroped;

    public UnitHealth _enemyHealth = new UnitHealth(100, 100);

    private Animator enemyAnimator;
    private NavMeshAgent agent;
    private bool isChasing = false;
    private GameManager gameManager;

    void Start()
    {
        // Inicialización de componentes
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;

        // Obtener la referencia al GameManager
        if (GameManager.gameManager != null)
        {
            gameManager = GameManager.gameManager;
        }
        else
        {
            Debug.LogError("GameManager no está asignado en la escena.");
        }
    }

    void Update()
    {
        // Comportamiento del enemigo
        if (agent.enabled) // Solo perseguir si el NavMeshAgent está habilitado
        {
            if (!isChasing)
            {
                DetectPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            // Si el NavMeshAgent está deshabilitado, no hacer nada
            isChasing = false;
            enemyAnimator.SetBool("isChasing", false);
        }
    }

    // Métodos de detección del jugador
    private void DetectPlayer()
    {
        bool playerSeen = PlayerViewDetection();
        bool playerHeard = PlayerHearDetection();

        if (playerSeen || playerHeard)
        {
            isChasing = true;
            enemyAnimator.SetBool("isChasing", true);
        }
    }

    private bool PlayerViewDetection()
    {
        if (player == null) return false; // Comprobar que el jugador está asignado

        // Posición del origen del Raycast (ajustada a la altura del enemigo)
        Vector3 origin = transform.position + Vector3.up * 1.5f; // Ajusta la altura si es necesario

        // Dirección hacia el jugador
        Vector3 direction = (player.transform.position - origin).normalized;

        // Distancia entre el enemigo y el jugador
        float distanceToTarget = Vector3.Distance(origin, player.transform.position);
        Debug.DrawRay(origin, direction * distanceToTarget, Color.red, 1f);

        // Verificar si el jugador está dentro del ángulo de visión
        if (Vector3.Angle(transform.forward, direction) < viewAngle / 2)
        {
            // Verificar si el jugador está dentro del radio de visión
            if (distanceToTarget <= viewRadius)
            {
                // Realizar el Raycast para verificar si hay obstáculos en el camino
                if (!Physics.Raycast(origin, direction, out RaycastHit hit, distanceToTarget, obstacleMask))
                {
                    return true; // El jugador es visible
                }
            }
        }

        return false; // No se detecta al jugador
    }

    private bool PlayerHearDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= hearRadius; // Si el jugador está dentro del radio de audición, lo detecta
    }

    // Método para perseguir al jugador
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    // Método para recibir daño
    private void EnemyTakeDmg(int dmg)
    {
        _enemyHealth.DmgUnit(dmg);
    }

    // Método para gestionar la colisión con el jugador y las balas
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // El enemigo muere al colisionar con el jugador
            gameManager.RemoveEnemy(gameObject);
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {

            // El enemigo recibe daño
            EnemyTakeDmg(15);
            Debug.Log(_enemyHealth.Health);

            // El enemigo comienza a perseguir al jugador después de recibir un disparo
            isChasing = true;
            enemyAnimator.SetBool("isChasing", true);

            // Si el enemigo muere, genera la explosión y la munición
            if (_enemyHealth.Health <= 0)
            {
                Vector3 explosionPos = transform.position;
                explosionPos.y = Mathf.Max(explosionPos.y, 1.5f); // Asegura que la posición de la explosión sea apropiada
                Instantiate(explosionParticle, explosionPos, Quaternion.identity); // Explosión
                SoundManager.Instance.PlaySound3D("Explosion", transform.position);

                Instantiate(ammoDroped, transform.position+Vector3.up * 0.7f, Quaternion.identity); // Munición caída
                Destroy(gameObject); // Destruye el enemigo
                gameManager.RemoveEnemy(gameObject);
            }
        }
        if (other.CompareTag("Environment"))
        {
            // Activar NavMeshAgent cuando colisiona con un objeto "Environment"
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;  // Reactivar NavMeshAgent
            }
        }
    }
}

