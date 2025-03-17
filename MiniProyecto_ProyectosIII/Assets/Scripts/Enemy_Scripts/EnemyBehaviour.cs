using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float enemySpeed = 2f;
    [SerializeField] private float chaseSpeed = 9f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    private Animator enemyAnimator;
    private GameObject player;
    private int patrolState;
    private float patrolTimer;
    private Quaternion targetRotation;
    private bool isAttacking;

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > detectionRange)
        {
            Patrol();
        }
        else if (distanceToPlayer > attackRange && !isAttacking)
        {
            ChasePlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    private void Patrol()
    {
        enemyAnimator.SetBool("isChasing", false);
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

    private void ChasePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed);
        transform.Translate(Vector3.forward * chaseSpeed * Time.deltaTime);

        enemyAnimator.SetBool("isPatrolling", false);
        enemyAnimator.SetBool("isChasing", true);
        enemyAnimator.SetBool("isAttacking", false);
    }

    private void AttackPlayer()
    {
        enemyAnimator.SetBool("isPatrolling", false);
        enemyAnimator.SetBool("isChasing", false);
        enemyAnimator.SetBool("isAttacking", true);
        isAttacking = true;
    }

    public void ResetAttack()
    {
        enemyAnimator.SetBool("isAttacking", false);
        isAttacking = false;
    }
}
