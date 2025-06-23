using UnityEngine;
// inventario
public class ObjectController : MonoBehaviour
{
    public GameObject obj;
    public int cant = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryController inv = GameObject.FindGameObjectWithTag("general_events").GetComponent<InventoryController>();
            inv.AddItem(obj, cant);
            Destroy(gameObject);
        }
    }
}
