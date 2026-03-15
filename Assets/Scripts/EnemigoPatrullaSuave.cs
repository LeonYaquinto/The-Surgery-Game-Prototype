using System.Collections;
using UnityEngine;

public class EnemigoIdleAleatorio : MonoBehaviour
{
    [Header("Animación")]
    public Animator ani;
    public string nombreTriggerIdle = "idle"; // Nombre del trigger en el Animator

    [Header("Configuración Tiempo")]
    public float tiempoMinEntreIdles = 5f;  // Mínimo tiempo entre idles
    public float tiempoMaxEntreIdles = 15f; // Máximo tiempo entre idles

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoIdle;

    [Range(0f, 1f)]
    public float volumenIdle = 0.8f;

    void Start()
    {
        ani = GetComponent<Animator>();

        // Configurar AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.spatialBlend = 1f; // Audio 3D
        audioSource.maxDistance = 20f;

        // Iniciar el ciclo de idles aleatorios
        StartCoroutine(IdleAleatorio());
    }

    IEnumerator IdleAleatorio()
    {
        while (true)
        {
            // Esperar un tiempo aleatorio
            float tiempoEspera = Random.Range(tiempoMinEntreIdles, tiempoMaxEntreIdles);
            yield return new WaitForSeconds(tiempoEspera);

            // Activar idle
            if (ani != null)
            {
                ani.SetTrigger(nombreTriggerIdle);
                Debug.Log($"{name} hace idle");
            }

            // Reproducir sonido al INICIO de la animación
            if (sonidoIdle != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoIdle, volumenIdle);
            }

            // Opcional: Esperar un poco para que no se solapen idles
            // (la duración aproximada de tu animación)
            yield return new WaitForSeconds(2f);
        }
    }

    // Método para llamar desde otros scripts si lo necesitas
    public void HacerIdle()
    {
        if (ani != null)
        {
            ani.SetTrigger(nombreTriggerIdle);
        }

        if (sonidoIdle != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoIdle, volumenIdle);
        }
    }
}