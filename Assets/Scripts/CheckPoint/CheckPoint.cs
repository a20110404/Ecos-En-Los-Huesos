using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoxCollider2D trigger;
    [SerializeField] private Animator checkpointAnimator;

    [Header("Configuración")]
    [SerializeField] private string activateAnimation = "Activate";

    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            // Solo actualizamos el punto de respawn, NO restauramos salud
            RespawnController.Instance.respawnPoint = transform;

            // Activar animación
            if (checkpointAnimator != null)
            {
                checkpointAnimator.SetTrigger(activateAnimation);
            }

            // Desactivar el trigger para evitar reactivaciones
            trigger.enabled = false;
            isActive = true;
        }
    }
}