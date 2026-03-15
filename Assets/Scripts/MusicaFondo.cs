using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    [Header("Configuración de Música")]
    public AudioClip musicaFondo;
    [Range(0f, 1f)]
    public float volumen = 0.5f;
    public bool reproducirAlIniciar = true;
    public bool mantenerEntreEscenas = false; // Música continua entre escenas

    private AudioSource audioSource;
    private static MusicaFondo instance;

    void Awake()
    {
        // Si querés que la música continúe entre escenas
        if (mantenerEntreEscenas)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    void Start()
    {
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = musicaFondo;
        audioSource.volume = volumen;
        audioSource.loop = true;
        audioSource.playOnAwake = reproducirAlIniciar;
        audioSource.spatialBlend = 0f; // Sonido 2D (no espacial)

        if (reproducirAlIniciar && musicaFondo != null)
        {
            audioSource.Play();
        }
    }

    // Métodos para controlar la música desde otros scripts
    public void ReproducirMusica()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PausarMusica()
    {
        audioSource.Pause();
    }

    public void DetenerMusica()
    {
        audioSource.Stop();
    }

    public void CambiarVolumen(float nuevoVolumen)
    {
        audioSource.volume = Mathf.Clamp01(nuevoVolumen);
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.Play();
    }
}