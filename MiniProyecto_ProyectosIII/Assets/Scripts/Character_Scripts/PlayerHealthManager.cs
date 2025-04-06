using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public UnitHealth health;

    void Awake()
    {
        health = new UnitHealth(100, 100); // Vida inicial al cargar la escena
    }

    public void ResetHealth()
    {
        health = new UnitHealth(100, 100); // Reinicia la vida a 100
    }

    public UnitHealth GetHealth()
    {
        return health;
    }
}
