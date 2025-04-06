using UnityEngine;

public class HealPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 25;

    private void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                UnitHealth playerHealth = GameManager.gameManager._playerHealth;


                if (playerHealth != null && playerHealth.Health < 100)
                {
                    playerController.SendMessage("PlayerHeal", healAmount);
                    SoundManager.Instance.PlaySound3D("Heal", transform.position);

                    Destroy(gameObject);
                }
            }
        }
    }
}
