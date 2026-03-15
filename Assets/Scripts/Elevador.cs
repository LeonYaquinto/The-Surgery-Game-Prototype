using System.Collections;
using UnityEngine;

public class Elevador : MonoBehaviour
{
    [Header("Configuración")]
    public Transform puntoDestino; // El punto donde aparecerá el jugador
    public bool usarFade = true; // Si quieres usar un fade a negro
    public float duracionFade = 0.5f;

    [Header("Audio")]
    public AudioClip sonidoElevador; // Sonido cuando se usa el elevador
    private AudioSource audioSource;

    [Header("Desactivar Control")]
    public bool desactivarControlTemporalmente = true; // Desactiva el movimiento durante la transición
    public float tiempoSinControl = 1f;

    void Start()
    {
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    public void Transportar(GameObject jugador)
    {
        Debug.Log("Transportar llamado! Jugador: " + (jugador != null ? jugador.name : "NULL"));

        if (puntoDestino == null)
        {
            Debug.LogError("No hay punto de destino asignado al elevador!");
            return;
        }

        Debug.Log("Iniciando transporte a: " + puntoDestino.position);
        StartCoroutine(ProcesoTransporte(jugador));
    }

    IEnumerator ProcesoTransporte(GameObject jugador)
    {
        // Reproducir sonido
        if (sonidoElevador != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoElevador);
        }

        // Desactivar controles del jugador si está habilitado
        FirstPersonMovement movimiento = null;
        FirstPersonLook camara = null;

        if (desactivarControlTemporalmente)
        {
            movimiento = jugador.GetComponent<FirstPersonMovement>();
            camara = jugador.GetComponentInChildren<FirstPersonLook>();

            if (movimiento != null) movimiento.enabled = false;
            if (camara != null) camara.enabled = false;
        }

        // Fade a negro si está habilitado
        if (usarFade)
        {
            yield return StartCoroutine(FadeNegro(true));
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        // Teletransportar al jugador
        CharacterController controller = jugador.GetComponent<CharacterController>();
        if (controller != null)
        {
            // Desactivar temporalmente para poder mover
            controller.enabled = false;
            jugador.transform.position = puntoDestino.position;
            jugador.transform.rotation = puntoDestino.rotation;
            controller.enabled = true;
        }
        else
        {
            // Si no tiene CharacterController, mover directamente
            jugador.transform.position = puntoDestino.position;
            jugador.transform.rotation = puntoDestino.rotation;
        }

        // Fade desde negro si está habilitado
        if (usarFade)
        {
            yield return StartCoroutine(FadeNegro(false));
        }

        // Esperar antes de reactivar controles
        if (desactivarControlTemporalmente)
        {
            yield return new WaitForSeconds(tiempoSinControl);

            if (movimiento != null) movimiento.enabled = true;
            if (camara != null) camara.enabled = true;
        }
    }

    IEnumerator FadeNegro(bool fadeOut)
    {
        // Buscar el panel de fade (asumiendo que usas el mismo de TransicionMuerte)
        UnityEngine.UI.Image panelFade = null;

        if (TransicionMuerte.instance != null && TransicionMuerte.instance.panelNegro != null)
        {
            panelFade = TransicionMuerte.instance.panelNegro;
        }

        if (panelFade == null)
        {
            yield break; // No hay panel de fade, salir
        }

        panelFade.gameObject.SetActive(true);

        float tiempo = 0;
        Color color = panelFade.color;
        float alphaInicial = fadeOut ? 0f : 1f;
        float alphaFinal = fadeOut ? 1f : 0f;

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            color.a = Mathf.Lerp(alphaInicial, alphaFinal, tiempo / duracionFade);
            panelFade.color = color;
            yield return null;
        }

        color.a = alphaFinal;
        panelFade.color = color;

        if (!fadeOut)
        {
            panelFade.gameObject.SetActive(false);
        }
    }
}