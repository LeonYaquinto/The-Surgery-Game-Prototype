using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CajaMortal : MonoBehaviour
{
    [Header("Configuración")]
    public string tagJugador = "Player";
    public bool usarTransicion = true; // Usar el sistema TransicionMuerte

    [Header("Audio")]
    public AudioClip sonidoMuerte;
    private AudioSource audioSource;

    [Header("Tiempo")]
    public float esperaAntesDeFade = 0.2f; // Pequeña pausa antes del fade

    private bool yaActivado = false;

    void Start()
    {
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Sonido 2D para que se escuche siempre
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador) && !yaActivado)
        {
            yaActivado = true;
            StartCoroutine(ProcesoMuerte());
        }
    }

    IEnumerator ProcesoMuerte()
    {
        Debug.Log("¡Jugador muerto por la caja!");

        // Reproducir sonido
        if (sonidoMuerte != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }

        // Pequeña espera antes de iniciar el fade
        yield return new WaitForSeconds(esperaAntesDeFade);

        // Usar TransicionMuerte que ya funciona
        if (usarTransicion && TransicionMuerte.instance != null)
        {
            TransicionMuerte.instance.MorirYCambiarEscena(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Reiniciar directamente si no hay transición
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}