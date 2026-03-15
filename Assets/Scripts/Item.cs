using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Información del Item")]
    public int ID;
    public string tipo; // "weapon", "key", "consumable", etc.
    public string descripcion;
    public Sprite icono; // IMPORTANTE: Asigna el sprite aquí en el Inspector

    [Header("Sonido")]
    public AudioClip sonidoRecoger; // Sonido cuando se recoge el item

    [Header("Estado")]
    [HideInInspector]
    public bool pickedUp = false;
    [HideInInspector]
    public bool equipped;

    [Header("Solo para Armas")]
    public bool playersWeapon;
    [HideInInspector]
    public GameObject weaponManager;
    [HideInInspector]
    public GameObject weapon;

    private void Start()
    {
        // Verificar que el icono esté asignado
        if (icono == null)
        {
            Debug.LogWarning("El item '" + descripcion + "' (ID: " + ID + ") no tiene icono asignado.");
        }

        // Solo buscar el weapon manager si es un arma
        if (tipo == "weapon")
        {
            weaponManager = GameObject.FindWithTag("WeaponManager");
            if (!playersWeapon && weaponManager != null)
            {
                int allWeapons = weaponManager.transform.childCount;
                for (int i = 0; i < allWeapons; i++)
                {
                    Item weaponItem = weaponManager.transform.GetChild(i).gameObject.GetComponent<Item>();
                    if (weaponItem != null && weaponItem.ID == ID)
                    {
                        weapon = weaponManager.transform.GetChild(i).gameObject;
                        break;
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Solo para armas equipadas
        if (equipped && tipo == "weapon")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                equipped = false;
                gameObject.SetActive(false);
            }
        }
    }

    // Método para reproducir el sonido al recoger
    public void ReproducirSonidoRecoger()
    {
        if (sonidoRecoger != null)
        {
            // Reproducir el sonido en la posición del item
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
            Debug.Log("Sonido de recoger reproducido para: " + descripcion);
        }
        else
        {
            Debug.LogWarning("No hay sonido de recoger asignado para: " + descripcion);
        }
    }

    public void ItemUsage()
    {
        if (tipo == "weapon")
        {
            UseWeapon();
        }
        else if (tipo == "key")
        {
            UseKey();
        }
        else if (tipo == "consumable")
        {
            UseConsumable();
        }
        else
        {
            Debug.Log("Usando item: " + descripcion);
        }
    }

    private void UseWeapon()
    {
        if (weapon != null)
        {
            weapon.SetActive(true);
            weapon.GetComponent<Item>().equipped = true;
            Debug.Log("Arma equipada: " + descripcion);
        }
        else
        {
            Debug.LogWarning("No se encontro el arma '" + descripcion + "' en el WeaponManager");
        }
    }

    private void UseKey()
    {
        Debug.Log("Llave '" + descripcion + "' seleccionada. Acercate a una puerta cerrada para usarla.");
    }

    private void UseConsumable()
    {
        Debug.Log("Usando consumible: " + descripcion);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Inventario inventario = player.GetComponent<Inventario>();
            if (inventario != null)
            {
                inventario.RemoveItem(ID);
                Debug.Log("Consumible '" + descripcion + "' usado y eliminado del inventario.");
            }
        }
    }
}