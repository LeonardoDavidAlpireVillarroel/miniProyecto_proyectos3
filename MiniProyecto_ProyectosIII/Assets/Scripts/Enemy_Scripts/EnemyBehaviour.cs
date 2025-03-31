using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float hearRadius = 5f;
    [SerializeField] private LayerMask targetPlayer;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Transform player;
    [SerializeField] private ParticleSystem explosionParticle;

    public UnitHealth _enemyHealth = new UnitHealth(100, 100);


    private Animator enemyAnimator;
    private NavMeshAgent agent;
    private bool isChasing = false;

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
    }

    void Update()
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
        if (player == null) return false; // Añadimos una comprobación para evitar el error si el jugador no está asignado.

        // Posición del origen del Raycast (ajustado a la altura del enemigo)
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

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            EnemyTakeDmg(25);
            Debug.Log(_enemyHealth.Health);

            // Al recibir un disparo, el enemigo comienza a perseguir al jugador
            isChasing = true;
            enemyAnimator.SetBool("isChasing", true);

            if (_enemyHealth.Health == 0)
            {
                Vector3 explosionPos = transform.position;
                explosionPos.y = Mathf.Max(explosionPos.y, 1.5f); // Asegurar que no baje más allá de un punto
                Instantiate(explosionParticle, explosionPos, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
    private void EnemyTakeDmg(int dmg)
    {
        _enemyHealth.DmgUnit(dmg);

    }
}
