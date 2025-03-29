//using UnityEngine;
//using UnityEngine.AI;

//public class EnemyBehaviour : MonoBehaviour
//{
//    [SerializeField] private float chaseSpeed = 5f;
//    [SerializeField] private float rotationSpeed = 5f;
//    [SerializeField] private float viewRadius;
//    [SerializeField] private float viewAngle;
//    [SerializeField] private float hearRadius;
//    [SerializeField] private float attackRange;

//    [SerializeField] private LayerMask targetPlayer;
//    [SerializeField] private LayerMask obstacleMask;
//    [SerializeField] private GameObject player;

//    private Animator enemyAnimator;
//    private bool isChasing = false;
//    private bool isAttacking = false;
//    private NavMeshAgent agent;

//    void Start()
//    {
//        enemyAnimator = GetComponent<Animator>();
//        agent = GetComponent<NavMeshAgent>();
//        agent.speed = chaseSpeed;
//    }

//    void Update()
//    {
//        if (isAttacking)
//        {
//            CheckAttackDistance(); // Verifica si debe seguir atacando
//        }
//        else if (isChasing)
//        {
//            ChasePlayer();
//        }
//        else
//        {
//            DetectPlayer();
//        }
//    }

//    private void DetectPlayer()
//    {
//        if (PlayerViewDetection() || PlayerHearDetection())
//        {
//            isChasing = true;
//            enemyAnimator.SetBool("isChasing", true);
//            agent.SetDestination(player.transform.position);
//        }
//    }

//    private bool PlayerViewDetection()
//    {
//        Vector3 origin = transform.position + Vector3.up * 1.5f;
//        Vector3 direction = (player.transform.position - origin).normalized;
//        float distanceToTarget = Vector3.Distance(origin, player.transform.position);

//        if (Vector3.Angle(transform.forward, direction) < viewAngle / 2)
//        {
//            if (distanceToTarget <= viewRadius)
//            {
//                if (!Physics.Raycast(origin, direction, distanceToTarget, obstacleMask))
//                {
//                    return true;
//                }
//            }
//        }
//        return false;
//    }

//    private bool PlayerHearDetection()
//    {
//        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);
//        return distanceToTarget <= hearRadius;
//    }

//    private void ChasePlayer()
//    {
//        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

//        if (distanceToPlayer <= attackRange)
//        {
//            StartAttack();
//        }
//        else
//        {
//            agent.SetDestination(player.transform.position);
//        }
//    }

//    private void StartAttack()
//    {
//        isAttacking = true;
//        isChasing = false;
//        agent.isStopped = true;
//        enemyAnimator.SetBool("isChasing", false);
//        enemyAnimator.SetBool("isAttacking", true);
//    }

//    private void CheckAttackDistance()
//    {
//        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

//        if (distanceToPlayer > attackRange)
//        {
//            StopAttack(); // El jugador se alejó, vuelve a perseguirlo
//        }
//    }

//    private void StopAttack()
//    {
//        isAttacking = false;
//        isChasing = true;
//        agent.isStopped = false;
//        enemyAnimator.SetBool("isAttacking", false);
//        enemyAnimator.SetBool("isChasing", true);
//    }
//}

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
            ExplosionAndDestroy();
        }
    }
    private void ExplosionAndDestroy()
    {
        Instantiate(explosionParticle, transform.position + Vector3.up * 2.5f, Quaternion.identity);
        Destroy(gameObject);

    }
}
