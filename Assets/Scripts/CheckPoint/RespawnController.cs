using UnityEngine;
using System.Collections;

public class RespawnController : MonoBehaviour
{
    public static RespawnController Instance;
    public Transform respawnPoint;

    [Header("Configuración")]
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float fallDeathY = -10f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckForFallDeath();
    }

    private void CheckForFallDeath()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.transform.position.y < fallDeathY)
        {
            StartCoroutine(RespawnPlayer(player));
        }
    }

    public IEnumerator RespawnPlayer(GameObject player)
    {
        // Desactivar temporalmente al jugador
        player.SetActive(false);

        // Esperar el tiempo de respawn
        yield return new WaitForSeconds(respawnDelay);

        // Restaurar salud al 100% solo en respawn
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.RestoreFullHealth();
        }

        // Mover al punto de respawn
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        // Reactivar jugador
        player.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(RespawnPlayer(collision.gameObject));
        }
    }
}