using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 20; // Cantidad de balas que otorga

    private void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsRifleAmmoFull()) // Solo recoge si no está lleno
            {
                SoundManager.Instance.PlaySound3D("PickUp", transform.position);

                player.AddRifleAmmo(ammoAmount);
                Destroy(gameObject); // Destruye la munición tras recogerla
            }
        }
    }
}
