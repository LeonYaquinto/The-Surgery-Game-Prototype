using System.Collections;
using UnityEngine;
using TMPro;

public class DoorUIManager : MonoBehaviour
{
    public static DoorUIManager Instance;

    [Header("Referencias UI")]
    public GameObject panelMensaje;
    public TextMeshProUGUI textoMensaje; // Cambiado a TextMeshPro

    [Header("Configuración Visual")]
    public Color colorTexto = Color.white;
    public int tamañoFuente = 24;

    private Coroutine coroutineActual;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ocultar panel al inicio
        if (panelMensaje != null)
        {
            panelMensaje.SetActive(false);
        }

        // Configurar texto
        if (textoMensaje != null)
        {
            textoMensaje.color = colorTexto;
            textoMensaje.fontSize = tamañoFuente;
        }
    }

    public void MostrarMensaje(string mensaje, float duracion = 2f)
    {
        if (panelMensaje == null || textoMensaje == null)
        {
            Debug.LogWarning("No hay referencias UI asignadas en DoorUIManager");
            return;
        }

        // Detener coroutine anterior si existe
        if (coroutineActual != null)
        {
            StopCoroutine(coroutineActual);
        }

        textoMensaje.text = mensaje;
        panelMensaje.SetActive(true);

        // Iniciar coroutine para ocultar después de la duración
        coroutineActual = StartCoroutine(OcultarDespuesDe(duracion));
    }

    public void OcultarMensaje()
    {
        if (coroutineActual != null)
        {
            StopCoroutine(coroutineActual);
            coroutineActual = null;
        }

        if (panelMensaje != null)
        {
            panelMensaje.SetActive(false);
        }
    }

    private IEnumerator OcultarDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        OcultarMensaje();
    }
}