using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    private bool inventarioActivo = false;
    public GameObject inventario;
    private int allSlots;
    private GameObject[] slot;
    public GameObject slotholder;

    void Start()
    {
        allSlots = slotholder.transform.childCount;
        slot = new GameObject[allSlots];

        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotholder.transform.GetChild(i).gameObject;
            if (slot[i].GetComponent<Slot>().item == null)
            {
                slot[i].GetComponent<Slot>().empty = true;
            }
        }

        // Asegurar que el inventario empiece cerrado
        if (inventario != null)
            inventario.SetActive(false);

        ActualizarCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventarioActivo = !inventarioActivo;
            ActualizarCursor();
        }

        // Sincronizar el estado visual del inventario
        if (inventario != null)
        {
            inventario.SetActive(inventarioActivo);
        }
    }

    private void ActualizarCursor()
    {
        if (inventarioActivo)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            GameObject itemPickedUp = other.gameObject;
            Item item = itemPickedUp.GetComponent<Item>();

            // Reproducir sonido de recoger
            if (item != null)
                item.ReproducirSonidoRecoger();

            AddItem(itemPickedUp, item.ID, item.tipo, item.descripcion, item.icono);
        }
    }

    public void AddItem(GameObject itemObject, int itemID, string itemTipo, string itemdescripcion, Sprite icono)
    {
        for (int i = 0; i < allSlots; i++)
        {
            Slot slotScript = slot[i].GetComponent<Slot>();
            if (slotScript.empty)
            {
                itemObject.GetComponent<Item>().pickedUp = true;
                slotScript.item = itemObject;
                slotScript.ID = itemID;
                slotScript.tipo = itemTipo;
                slotScript.descripcion = itemdescripcion;
                slotScript.icono = icono;
                itemObject.transform.parent = slot[i].transform;
                itemObject.SetActive(false);
                slotScript.empty = false;
                slotScript.UpdateSlot();
                return;
            }
        }
    }

    public bool HasItem(int itemID)
    {
        for (int i = 0; i < allSlots; i++)
        {
            Slot slotScript = slot[i].GetComponent<Slot>();
            if (!slotScript.empty && slotScript.ID == itemID)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int itemID)
    {
        for (int i = 0; i < allSlots; i++)
        {
            Slot slotScript = slot[i].GetComponent<Slot>();
            if (!slotScript.empty && slotScript.ID == itemID)
            {
                if (slotScript.item != null)
                {
                    Destroy(slotScript.item);
                }
                slotScript.item = null;
                slotScript.ID = 0;
                slotScript.tipo = "";
                slotScript.descripcion = "";
                slotScript.icono = null;
                slotScript.empty = true;
                slotScript.UpdateSlot();
                return;
            }
        }
    }

    // Método público para que otros scripts verifiquen si el inventario está abierto
    public bool InventarioEstaAbierto()
    {
        return inventarioActivo;
    }
}