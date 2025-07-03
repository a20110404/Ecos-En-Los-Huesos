using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private UIInventoryItem item;

    private bool isOverInventory;

    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        if (item == null)
        {
            item = GetComponentInChildren<UIInventoryItem>(true);
        }
    }

    public void SetData(Sprite sprite, int quantity)
    {
        if(item != null)
        {
            item.SetData(sprite, quantity);
            // Asegurarnos que el item está visible
            item.gameObject.SetActive(true);

            // Actualizar color inmediatamente
            UpdateItemColor();
        }
    }

    private void Update()
    {
        transform.position = Input.mousePosition;

        // Verificar si estamos sobre el inventario
        isOverInventory = IsPointerOverInventory();

        // Cambiar color si está fuera del inventario
        UpdateItemColor();
    }

    private void UpdateItemColor()
    {
        if (item != null)
        {
            // Obtener la imagen principal del item
            Image itemImage = item.GetComponentInChildren<Image>();
            if (itemImage != null)
            {
                itemImage.color = isOverInventory ? Color.white : new Color(1, 0.5f, 0.5f, 0.8f);
            }
        }
    }


    public void Toggle(bool val)
    {
        Debug.Log($"Itme toggled {val}");
        gameObject.SetActive(val);

        if (val && item != null)
        {
            // Restablecer color al mostrar
            Image itemImage = item.GetComponentInChildren<Image>();
            if (itemImage != null)
            {
                itemImage.color = Color.white;
            }
        }
    }

    // Método para verificar si el cursor está sobre el inventario
    private bool IsPointerOverInventory()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            // Buscar cualquier objeto con el tag "Inventory"
            if (result.gameObject.CompareTag("Inventory") ||
                (result.gameObject.transform.parent != null &&
                 result.gameObject.transform.parent.CompareTag("Inventory")))
            {
                return true;
            }
        }
        return false;
    }

    // Nuevo método para verificar el estado
    public bool IsOverInventory() => isOverInventory;
}
