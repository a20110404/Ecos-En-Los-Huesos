using UnityEngine;

public class AttributesController : MonoBehaviour
{
    public int cantidad;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void setCantidad(int cantidad)
    {
        this.cantidad = cantidad;
    }

    public int getCantidad()
    {
        return this.cantidad;
    }
}
