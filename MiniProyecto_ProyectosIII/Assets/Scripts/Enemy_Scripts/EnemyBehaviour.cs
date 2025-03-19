using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float enemySpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float viewRadius;
    [SerializeField] private float viewAngle;
    [SerializeField] private float hearRadius;
    [SerializeField] private float attackRange;

    [SerializeField] private LayerMask targetPlayer;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private GameObject player;

    private Animator enemyAnimator;
    private int patrolState;
    private float patrolTimer;
    private Quaternion targetRotation;
    private bool isChasing = false;
    private bool isAttacking = false;

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isAttacking)
            return; // No hacer nada si está atacando

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer();
        }
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= 2f)
        {
            patrolState = Random.Range(0, 2);
            patrolTimer = 0;
        }

        switch (patrolState)
        {
            case 0:
                enemyAnimator.SetBool("isPatrolling", false);
                break;
            case 1:
                targetRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                patrolState++;
                break;
            case 2:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * enemySpeed * Time.deltaTime);
                enemyAnimator.SetBool("isPatrolling", true);
                break;
        }
    }

    private void DetectPlayer()
    {
        if (PlayerViewDetection() || PlayerHearDetection())
        {
            isChasing = true;
            enemyAnimator.SetBool("isChasing", true);
            enemyAnimator.SetBool("isPatrolling", false);
        }
    }

    private bool PlayerViewDetection()
    {
        Vector3 playerTarget = (player.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

        if (Vector3.Angle(transform.forward, playerTarget) < viewAngle / 2)
        {
            if (distanceToTarget <= viewRadius)
            {
                if (!Physics.Raycast(transform.position, playerTarget, distanceToTarget, obstacleMask))
                {
                    Debug.Log("Te vi");
                    return true;
                }
            }
        }
        return false;
    }

    private bool PlayerHearDetection()
    {
        float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToTarget <= hearRadius)
        {
            Debug.Log("Te oí");
            return true;
        }

        return false;
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            return;
        }

        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.position += direction * chaseSpeed * Time.deltaTime;

        enemyAnimator.SetBool("isAttacking", false);
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        enemyAnimator.SetBool("isChasing", false);
        enemyAnimator.SetBool("isAttacking", true);
    }

    public void ResetAttack()
    {
        enemyAnimator.SetBool("isAttacking", false);
        isAttacking = false;

        // Verificar si el jugador sigue cerca, si no, volver a perseguir
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > attackRange)
        {
            isChasing = true;
            enemyAnimator.SetBool("isChasing", true);
        }
    }
}
