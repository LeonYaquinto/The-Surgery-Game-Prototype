using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathTrigger : MonoBehaviour
{
    [Header("Configuración")]
    public string tagObjetoPeligroso = "Death"; // Tag del objeto que causa la muerte
    public float tiempoFade = 2f; // Duración del fade a negro
    public float tiempoAntesDeReiniciar = 1f; // Tiempo extra después del fade antes de reiniciar

    [Header("Audio")]
    public AudioClip sonidoMuerte; // Arrastra aquí el sonido de muerte
    private AudioSource audioSource;

    [Header("UI Fade")]
    public Image panelFade; // Imagen negra para el fade

    private bool estaMuriendo = false;

    void Start()
    {
        // Crear o configurar el AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // Verificar que tenemos el panel de fade
        if (panelFade == null)
        {
            Debug.LogError("¡No hay Panel de Fade asignado! Asigna una imagen negra en el Inspector.");
        }
        else
        {
            // Asegurar que el panel empiece transparente
            Color color = panelFade.color;
            color.a = 0f;
            panelFade.color = color;
            panelFade.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si colisionó con un objeto peligroso y si no está ya muriendo
        if (other.CompareTag(tagObjetoPeligroso) && !estaMuriendo)
        {
            estaMuriendo = true;
            StartCoroutine(MuerteYReinicio());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // También detectar colisiones normales (por si no es trigger)
        if (collision.collider.CompareTag(tagObjetoPeligroso) && !estaMuriendo)
        {
            estaMuriendo = true;
            StartCoroutine(MuerteYReinicio());
        }
    }

    IEnumerator MuerteYReinicio()
    {
        // Reproducir sonido de muerte
        if (sonidoMuerte != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }

        // Fade a negro
        if (panelFade != null)
        {
            float tiempoTranscurrido = 0f;
            Color color = panelFade.color;

            while (tiempoTranscurrido < tiempoFade)
            {
                tiempoTranscurrido += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, tiempoTranscurrido / tiempoFade);
                panelFade.color = color;
                yield return null;
            }

            // Asegurar que termine completamente negro
            color.a = 1f;
            panelFade.color = color;
        }

        // Esperar un tiempo adicional antes de reiniciar
        yield return new WaitForSeconds(tiempoAntesDeReiniciar);

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}