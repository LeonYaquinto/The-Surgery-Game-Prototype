using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public GameObject armaEnMano; // El arma que aparecerá en la mano del jugador

    [Header("Sonidos")]
    public AudioClip sonidoRecoger; // Sonido al recoger el arma del suelo
    public AudioClip sonidoEquipar; // Sonido al equipar el arma en la mano

    private AudioSource audioSource;
    private bool yaRecogida = false;

    void Start()
    {
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Verificar que el arma en mano esté desactivada al inicio
        if (armaEnMano != null)
        {
            armaEnMano.SetActive(false);
        }
        else
        {
            Debug.LogError("GunPickup: Falta asignar el 'armaEnMano' en el Inspector");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (yaRecogida) return;

        if (other.CompareTag("Player"))
        {
            RecogerArma();
        }
    }

    void RecogerArma()
    {
        yaRecogida = true;

        // Reproducir sonido de recoger
        if (sonidoRecoger != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
        }

        // Activar el arma en la mano
        if (armaEnMano != null)
        {
            armaEnMano.SetActive(true);

            // Reproducir sonido de equipar
            if (sonidoEquipar != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoEquipar);
            }

            // Notificar al GunController que tiene el arma
            GunController gunController = armaEnMano.GetComponent<GunController>();
            if (gunController != null)
            {
                gunController.RecogerArma();
            }
        }

        // Destruir el arma del suelo
        Destroy(gameObject);
    }
}