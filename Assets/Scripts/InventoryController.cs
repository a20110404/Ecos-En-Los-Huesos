using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    // Cambiamos a una clase que almacene tanto el prefab como la cantidad
    [System.Serializable]
    public class InventoryItem
    {
        public GameObject prefab;
        public int cantidad;
        public string itemID; // Identificador único para agrupar items similares
    }

    public InventoryItem[] items;
    private int num_slots_max = 59;

    void Start()
    {
        items = new InventoryItem[num_slots_max];
    }

    public void AddItem(GameObject newItem, int cantidad)
    {
        string itemID = newItem.GetComponent<ItemIdentifier>().itemID;

        // 1. Buscar si ya existe el item
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemID == itemID)
            {
                items[i].cantidad += cantidad;
                return;
            }
        }

        // 2. Buscar primer slot vacío
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = new InventoryItem()
                {
                    prefab = newItem,
                    cantidad = cantidad,
                    itemID = itemID
                };
                return;
            }
        }

        Debug.LogWarning("Inventario lleno!");
    }

    public void showInventory()
    {
        // Limpiar inventario actual
        removeItems();

        // Obtener todos los slots
        List<Transform> emptySlots = new List<Transform>();
        foreach (Transform child in GameObject.FindGameObjectWithTag("inventario").transform)
        {
            if (child.CompareTag("slot"))
            {
                emptySlots.Add(child);
            }
        }

        // Instanciar items en slots vacíos
        int slotIndex = 0;
        for (int i = 0; i < items.Length && slotIndex < emptySlots.Count; i++)
        {
            if (items[i] != null)
            {
                Transform slot = emptySlots[slotIndex];
                GameObject item = Instantiate(items[i].prefab, slot.position, Quaternion.identity, slot);
                item.name = items[i].prefab.name;

                // Actualizar la cantidad en el UI
                Text textComponent = item.GetComponentInChildren<Text>();
                if (textComponent != null)
                {
                    textComponent.text = items[i].cantidad.ToString();
                }

                slotIndex++;
            }
        }
    }

    private void removeItems()
    {
        foreach (Transform slot in GameObject.FindGameObjectWithTag("inventario").transform)
        {
            if (slot.CompareTag("slot") && slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
    }
}