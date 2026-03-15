using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDoor : MonoBehaviour
{
    public bool doorOpen = false;
    public float doorOpenAngle = 95f;
    public float doorCloseAngle = 0f;
    public float smooth = 3f;

    [Header("Sistema de Llave")]
    public bool requiresKey = false;
    public int requiredKeyID = 0;
    public bool consumeKey = false;
    private bool wasUnlocked = false;

    [Header("Sonidos")]
    public AudioClip sonidoAbrir;
    public AudioClip sonidoCerrar;
    public AudioClip sonidoBloqueo;
    public AudioClip sonidoSinLlave;
    private AudioSource audioSource;

    private GameObject player;

    [Header("Indicadores Visuales")]
    public bool usarIndicadoresVisuales = false; // Desactivado por defecto
    public Renderer puertaRenderer;
    public Color colorBloqueado = Color.red;
    public Color colorDesbloqueado = Color.grey;
    public Color colorNormal = Color.white;

    [Header("Mensajes UI")]
    public string mensajeSinLlave = "Necesitas una llave";
    public float tiempoMensaje = 2f;

    private bool isBeingLookedAt = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        if (puertaRenderer == null)
            puertaRenderer = GetComponentInChildren<Renderer>();

        CambiarColor(colorNormal);
    }

    public void OnLookStart()
    {
        isBeingLookedAt = true;
        if (usarIndicadoresVisuales)
            ActualizarColorSegunEstado();
    }

    public void OnLookEnd()
    {
        isBeingLookedAt = false;

        if (usarIndicadoresVisuales)
        {
            // Si fue desbloqueada, mantener color gris siempre
            if (wasUnlocked)
            {
                CambiarColor(colorDesbloqueado);
            }
            else
            {
                CambiarColor(colorNormal);
            }
        }
    }

    private void ActualizarColorSegunEstado()
    {
        if (!usarIndicadoresVisuales) return;

        // Si ya fue desbloqueada, SIEMPRE gris (incluso si se cierra)
        if (wasUnlocked)
        {
            CambiarColor(colorDesbloqueado);
            return;
        }

        // Si no está mirando, color normal
        if (!isBeingLookedAt)
        {
            CambiarColor(colorNormal);
            return;
        }

        // Solo aplicar colores de bloqueo si NO ha sido desbloqueada
        if (requiresKey)
        {
            if (HasRequiredKey())
            {
                CambiarColor(colorDesbloqueado);
            }
            else
            {
                CambiarColor(colorBloqueado);
            }
        }
        else
        {
            CambiarColor(colorNormal);
        }
    }

    public void ChangeDoorState()
    {
        if (requiresKey && !wasUnlocked && !doorOpen)
        {
            if (HasRequiredKey())
            {
                wasUnlocked = true;

                if (consumeKey)
                {
                    RemoveKeyFromInventory();
                    Debug.Log("Llave consumida");
                }

                doorOpen = true;
                ReproducirSonido(sonidoAbrir);
                if (usarIndicadoresVisuales)
                    ActualizarColorSegunEstado();

                Debug.Log("Puerta desbloqueada y abierta con llave");
            }
            else
            {
                ReproducirSonido(sonidoSinLlave != null ? sonidoSinLlave : sonidoBloqueo);

                // Mostrar mensaje al intentar abrir sin llave
                if (DoorUIManager.Instance != null)
                {
                    DoorUIManager.Instance.MostrarMensaje(mensajeSinLlave, tiempoMensaje);
                }

                Debug.Log("No tienes la llave con ID: " + requiredKeyID);
            }
        }
        else
        {
            doorOpen = !doorOpen;

            if (doorOpen)
            {
                ReproducirSonido(sonidoAbrir);
                Debug.Log("Puerta abierta");
            }
            else
            {
                ReproducirSonido(sonidoCerrar);
                Debug.Log("Puerta cerrada");
            }

            if (usarIndicadoresVisuales)
                ActualizarColorSegunEstado();
        }
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void CambiarColor(Color color)
    {
        if (usarIndicadoresVisuales && puertaRenderer != null)
            puertaRenderer.material.color = color;
    }

    bool HasRequiredKey()
    {
        if (player == null) return false;

        Inventario inventario = player.GetComponent<Inventario>();
        if (inventario == null) return false;

        return inventario.HasItem(requiredKeyID);
    }

    void RemoveKeyFromInventory()
    {
        if (player == null) return;

        Inventario inventario = player.GetComponent<Inventario>();
        if (inventario != null)
            inventario.RemoveItem(requiredKeyID);
    }

    void Update()
    {
        Quaternion targetRotation = Quaternion.Euler(0, doorOpen ? doorOpenAngle : doorCloseAngle, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}