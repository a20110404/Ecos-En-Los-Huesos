using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The target to follow
    public Transform skyBackground, cloudsBackgroud;
    public Transform middleBackground, closeBackground; // Background layers

    public float minHeight, maxHeight; // Limites de la camara en y

    private Vector2 lastXPosition;

    void Start()
    {
        lastXPosition = transform.position;
    }

    void Update()
    {
        // transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        transform.position = new Vector3(target.position.x, Mathf.Clamp(target.position.y, minHeight, maxHeight), transform.position.z);

        Vector2 amountToMove = new Vector2(transform.position.x - lastXPosition.x, transform.position.y - lastXPosition.y);
        skyBackground.position = skyBackground.position + new Vector3(amountToMove.x, amountToMove.y, 0f);
        cloudsBackgroud.position += new Vector3(amountToMove.x, amountToMove.y, 0f) * 0.95f;
        middleBackground.position += new Vector3(amountToMove.x, amountToMove.y, 0f) * 0.60f;
        closeBackground.position += new Vector3(amountToMove.x, amountToMove.y, 0f) * 0.30f;
        lastXPosition = transform.position;
    }
}
