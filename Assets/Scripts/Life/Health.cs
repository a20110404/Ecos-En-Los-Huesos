using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private FloatValueSO currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealthPercent => currentHealth.Value * 100f;
    public FloatValueSO HealthSO => currentHealth;

    private void Start()
    {
        if (currentHealth != null)
        {
            currentHealth.Value = 1; // Salud inicial al 100%
        }
        else
        {
            Debug.LogError("Health: currentHealth FloatValueSO no asignado en " + gameObject.name);
        }
    }

    public void Reduce(int damage)
    {
        if (currentHealth == null) return;

        currentHealth.Value -= damage / maxHealth;

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    public void AddHealth(int healthBoost)
    {
        if (currentHealth == null) return;

        float healthValue = currentHealth.Value * maxHealth;
        healthValue += healthBoost;
        currentHealth.Value = Mathf.Clamp(healthValue, 0, maxHealth) / maxHealth;
    }

    public void RestoreFullHealth()
    {
        if (currentHealth != null)
        {
            currentHealth.Value = 1; // 100%
        }
    }

    private void Die()
    {
        // Llamar al sistema de respawn
        RespawnController.Instance.StartCoroutine(RespawnController.Instance.RespawnPlayer(gameObject));
    }
}