using UnityEngine;

public class InteractuarPortal : MonoBehaviour
{
    [SerializeField] private Transform controladorInteractuar;
    [SerializeField] private Vector2 dimensionesCaja;
    [SerializeField] private LayerMask capasInteractuables;
    private void Update()
    {
        if (Input.GetButtonDown("Interactuar"))
        {
            Interactuar();
        }    
    }

    private void Interactuar()
    {
       Collider2D[] objetosTocados = Physics2D.OverlapBoxAll(controladorInteractuar.position, dimensionesCaja, 0f, capasInteractuables);

        foreach (Collider2D objeto in objetosTocados)
        {
            Debug.Log(objeto.name);

            if (objeto.TryGetComponent(out Portal portal))
            {
                portal.Interactuar();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(controladorInteractuar.position, dimensionesCaja);
    }
}
