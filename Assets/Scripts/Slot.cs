using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public GameObject item;
    public int ID;
    public string tipo;
    public string descripcion;
    public bool empty;
    public Sprite icono;
    private Transform slotIconoGameObject;

    private void Start()
    {
        InicializarSlot();
    }

    // Método para asegurar que slotIconoGameObject esté inicializado
    private void InicializarSlot()
    {
        if (slotIconoGameObject == null && transform.childCount > 0)
        {
            slotIconoGameObject = transform.GetChild(0);
        }
    }

    public void UpdateSlot()
    {
        // Asegurar que esté inicializado antes de actualizar
        InicializarSlot();

        if (slotIconoGameObject == null)
        {
            Debug.LogError("No se encontró el hijo del slot (icono) en: " + gameObject.name);
            return;
        }

        Image iconImage = slotIconoGameObject.GetComponent<Image>();

        if (iconImage == null)
        {
            Debug.LogError("El hijo del slot no tiene componente Image en: " + gameObject.name);
            return;
        }

        if (empty || icono == null)
        {
            // Slot vacío - hacer el icono transparente
            iconImage.sprite = null;
            iconImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            // Slot con item - mostrar icono
            iconImage.sprite = icono;
            iconImage.color = new Color(1, 1, 1, 1);
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.GetComponent<Item>().ItemUsage();
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        UseItem();
    }
}