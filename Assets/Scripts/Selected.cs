using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InteractableObject
{
    public string tag;
    public string interactionText;
}

public class Selected : MonoBehaviour
{
    [Header("Interacción")]
    public float distancia = 3f;

    [Header("Objetos Interactuables")]
    public List<InteractableObject> objetosInteractuables = new List<InteractableObject>();

    [Header("UI")]
    public Image crosshair;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.green;

    [Header("UI Texto")]
    public TextMeshProUGUI interactionText;

    private GameObject ultimoReconocido;
    private GameObject player;

    void Start()
    {
        if (crosshair != null)
            crosshair.color = normalColor;
        player = GameObject.FindGameObjectWithTag("Player");
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);

        // Configurar objetos por defecto si la lista está vacía
        if (objetosInteractuables.Count == 0)
        {
            objetosInteractuables.Add(new InteractableObject { tag = "Door", interactionText = "[E] Interactuar con puerta" });
            objetosInteractuables.Add(new InteractableObject { tag = "Radio", interactionText = "[E] Encender radio" });
        }
    }

    void Update()
    {
        // Visualizar el raycast en la escena (puedes comentar esta línea después de debuggear)
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distancia, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia))
        {
            // Buscar si el objeto tiene un tag interactuable
            InteractableObject objetoEncontrado = BuscarObjetoInteractuable(hit.collider.tag);

            if (objetoEncontrado != null)
            {
                // Cambiar color de la mira a hover (verde)
                if (crosshair != null)
                    crosshair.color = hoverColor;

                // Mostrar texto de interacción
                if (interactionText != null)
                {
                    // Caso especial para puertas (mostrar abrir/cerrar)
                    if (hit.collider.CompareTag("Door"))
                    {
                        SystemDoor door = hit.collider.GetComponent<SystemDoor>();
                        if (door != null)
                        {
                            if (door.doorOpen)
                            {
                                interactionText.text = "[E] Cerrar puerta";
                            }
                            else
                            {
                                interactionText.text = "[E] Abrir puerta";
                            }
                        }
                        else
                        {
                            interactionText.text = objetoEncontrado.interactionText;
                        }
                    }
                    else
                    {
                        interactionText.text = objetoEncontrado.interactionText;
                    }

                    interactionText.gameObject.SetActive(true);
                }

                // Actualizar último objeto reconocido
                ultimoReconocido = hit.collider.gameObject;

                // Interacción con E
                if (Input.GetKeyDown(KeyCode.E))
                {
                    InteractuarConObjeto(hit.collider);
                }
            }
            else
            {
                RestaurarVista();
            }
        }
        else
        {
            RestaurarVista();
        }
    }

    InteractableObject BuscarObjetoInteractuable(string tag)
    {
        foreach (InteractableObject obj in objetosInteractuables)
        {
            if (obj.tag == tag)
            {
                return obj;
            }
        }
        return null;
    }

    void InteractuarConObjeto(Collider collider)
    {
        Debug.Log("Interactuando con: " + collider.tag);

        if (collider.CompareTag("Door"))
        {
            SystemDoor door = collider.GetComponent<SystemDoor>();
            if (door != null)
            {
                door.ChangeDoorState();
            }
        }
        else if (collider.CompareTag("Radio"))
        {
            AudioSource[] audioSources = collider.GetComponents<AudioSource>();

            if (audioSources.Length >= 2)
            {
                StartCoroutine(ReproducirRadio(audioSources));
            }
            else
            {
                Debug.LogWarning("El objeto Radio necesita al menos 2 AudioSource componentes");
            }
        }
        else if (collider.CompareTag("Elevador"))
        {
            Debug.Log("¡Elevador detectado!");
            Elevador elevador = collider.GetComponent<Elevador>();
            if (elevador != null)
            {
                Debug.Log("Componente Elevador encontrado, transportando...");
                elevador.Transportar(player);
            }
            else
            {
                Debug.LogWarning("El objeto Elevador necesita el componente Elevador");
            }
        }
    }

    IEnumerator ReproducirRadio(AudioSource[] audioSources)
    {
        // Activar ambos AudioSource
        foreach (AudioSource audio in audioSources)
        {
            audio.Play();
        }

        // Esperar 28 segundos
        yield return new WaitForSeconds(28f);

        // Detener ambos AudioSource
        foreach (AudioSource audio in audioSources)
        {
            audio.Stop();
        }
    }

    void RestaurarVista()
    {
        if (crosshair != null)
            crosshair.color = normalColor;
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
        ultimoReconocido = null;
    }
}