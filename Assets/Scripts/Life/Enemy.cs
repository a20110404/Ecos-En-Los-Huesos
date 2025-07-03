using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageCooldown = 1f;
    [SerializeField] private float pushForce = 15f; // Aumentado para mejor efecto

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";

    private bool canDamage = true;
    private Health playerHealth;

    // Auto-asignar componentes si no están en el inspector
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Animator no encontrado en " + gameObject.name);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canDamage)
        {
            if (playerHealth == null)
            {
                playerHealth = other.GetComponent<Health>();
            }

            if (playerHealth != null && playerHealth.HealthSO != null)
            {
                // Activar animación solo si tenemos Animator
                if (animator != null)
                {
                    animator.SetTrigger(attackTrigger);
                }

                float healthBefore = playerHealth.CurrentHealthPercent;
                playerHealth.Reduce(contactDamage);
                float healthAfter = playerHealth.CurrentHealthPercent;

                PushPlayer(other.transform);

                Debug.Log($"Vida del jugador: {healthAfter:F1}%");
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private void PushPlayer(Transform playerTransform)
    {
        PlayerController playerController = playerTransform.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Vector2 pushDirection = (playerTransform.position - transform.position).normalized;

            // Añadir componente vertical para mejor efecto
            pushDirection.y += 0.3f;
            pushDirection = pushDirection.normalized;

            playerController.ApplyKnockback(pushDirection * pushForce);
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}