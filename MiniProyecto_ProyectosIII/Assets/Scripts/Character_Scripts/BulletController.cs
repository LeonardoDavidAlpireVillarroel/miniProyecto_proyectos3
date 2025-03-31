using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public LayerMask CollisionLayers = 1;
    public float Speed = 500;
    public float Lifespan = 3;

    [Tooltip("Stretch factor in the direction of motion while flying")]
    public float Stretch = 6;

    private float m_Speed;
    private Vector3 m_SpawnPoint;

    void OnValidate()
    {
        Speed = Mathf.Max(1, Speed);
        Lifespan = Mathf.Max(0.2f, Lifespan);
    }

    void OnEnable()
    {
        m_Speed = Speed;
        m_SpawnPoint = transform.position;
        SetStretch(1);
        StartCoroutine(DeactivateAfter());

        // Ignora la colisión entre la bala y el jugador
        Collider bulletCollider = GetComponent<Collider>();
        Collider playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
        if (bulletCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(bulletCollider, playerCollider);
        }
    }

    void Update()
    {
        if (m_Speed > 0)
        {
            if (CheckCollision()) return; // Si colisiona, salir del Update para evitar movimientos innecesarios
            MoveBullet();
        }
    }

    private bool CheckCollision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, m_Speed * Time.deltaTime, CollisionLayers, QueryTriggerInteraction.Ignore))
        {
            transform.position = hitInfo.point;
            Destroy(gameObject); // Destruir la bala al impactar
            return true; // Indicar que hubo colisión
        }
        return false;
    }

    private void MoveBullet()
    {
        float deltaPos = m_Speed * Time.deltaTime;
        transform.position += deltaPos * transform.forward;
        SetStretch(Mathf.Min(1 + deltaPos * Stretch, Vector3.Distance(transform.position, m_SpawnPoint)));
    }

    void SetStretch(float stretch)
    {
        var scale = transform.localScale;
        scale.z = stretch;
        transform.localScale = scale;
    }

    IEnumerator DeactivateAfter()
    {
        yield return new WaitForSeconds(Lifespan);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
