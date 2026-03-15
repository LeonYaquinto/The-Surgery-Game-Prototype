using UnityEngine;

public class LinternaController : MonoBehaviour
{
    [Header("Referencia a la linterna en mano")]
    public GameObject linternaEnMano;

    [Header("Sonidos de la linterna")]
    public AudioClip sonidoEncender;
    public AudioClip sonidoApagar;

    [Header("Estado inicial")]
    public bool empiezaApagada = true; // Configurar si empieza apagada o encendida

    private Light luzLinterna;
    private AudioSource audioSource;
    private bool tieneLinterna = false;
    private bool encendida = false;

    void Start()
    {
        // Obtener o agregar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (linternaEnMano != null)
        {
            luzLinterna = linternaEnMano.GetComponentInChildren<Light>();

            // Mantener el objeto activo pero apagar la luz
            linternaEnMano.SetActive(false);

            if (luzLinterna != null)
            {
                luzLinterna.enabled = false; // Asegurar que la luz empiece apagada
            }
        }
    }

    void Update()
    {
        if (tieneLinterna && Input.GetKeyDown(KeyCode.F))
        {
            encendida = !encendida;

            if (luzLinterna != null)
            {
                luzLinterna.enabled = encendida;
            }

            // Reproducir sonido correspondiente
            if (encendida)
            {
                ReproducirSonido(sonidoEncender);
                Debug.Log("Linterna encendida");
            }
            else
            {
                ReproducirSonido(sonidoApagar);
                Debug.Log("Linterna apagada");
            }
        }
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void RecogerLinterna()
    {
        tieneLinterna = true;
        linternaEnMano.SetActive(true);

        // Configurar el estado inicial correctamente
        if (empiezaApagada)
        {
            encendida = false;
            if (luzLinterna != null)
            {
                luzLinterna.enabled = false;
            }
        }
        else
        {
            encendida = true;
            if (luzLinterna != null)
            {
                luzLinterna.enabled = true;
            }
        }

        Debug.Log("Has recogido la linterna. Estado: " + (encendida ? "Encendida" : "Apagada"));
    }
}