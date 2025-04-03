using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Grenade : MonoBehaviour
{
    public float delay = 1f;
    private float countDown;
    private bool hasExploded = false;
    public GameObject explosionEffect;
    [SerializeField] private float radius = 10f;

    private void Start()
    {
        countDown = delay;
    }

    private void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0 && !hasExploded)
        {
            hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Collider[] collidersToAffect = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToAffect)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {
                NavMeshAgent agent = nearbyObject.GetComponent<NavMeshAgent>();
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

                if (agent != null)
                {
                    agent.enabled = false; // Desactivar NavMeshAgent
                }

                if (rb != null)
                {
                    rb.useGravity = false;  // Desactivar gravedad para elevar
                    rb.isKinematic = false; // Permitir que la física lo afecte
                }

                nearbyObject.transform.position += Vector3.up * 1; // Elevar enemigo

                // Iniciar la corrutina para restaurar el movimiento
                StartCoroutine(ReactivateEnemy(agent, rb, 5f));
            }
        }

        // Destruir la granada después de un pequeño retraso
        //Destroy(gameObject, 0.5f);
    }

    private IEnumerator ReactivateEnemy(NavMeshAgent agent, Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = true; // Restablecer el Rigidbody para que el NavMeshAgent funcione correctamente
        }

        if (agent != null)
        {
            agent.enabled = true; // Reactivar el NavMeshAgent para que pueda moverse de nuevo
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !hasExploded)
        {
            hasExploded = true;
            Explode(); // Explota inmediatamente al colisionar
        }
    }
}
