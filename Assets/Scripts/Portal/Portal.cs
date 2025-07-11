using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string nombreEscenaDestino;
    public void Interactuar()
    {
        // Valida que la escena destino no esté vacía
        if (!string.IsNullOrEmpty(nombreEscenaDestino))
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.LogWarning("No se ha asignado una escena de destino en el portal.");
        }
    }
}
