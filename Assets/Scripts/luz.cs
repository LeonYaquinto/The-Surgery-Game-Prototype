using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light flickerLight;
    private AudioSource audioSource;

    [Header("Parámetros del parpadeo")]
    public float minTime = 0.05f;   // tiempo mínimo entre cambios
    public float maxTime = 0.2f;    // tiempo máximo entre cambios
    public float minIntensity = 0.5f; // brillo mínimo
    public float maxIntensity = 2f;   // brillo máximo

    [Header("Sonido del parpadeo")]
    public AudioClip sonidoParpadeo;
    [Range(0f, 1f)]
    public float volumenSonido = 0.3f;  // Volumen del sonido (ajustable)
    public bool reproducirSonido = true; // Activar/desactivar sonido

    private float timer;

    void Start()
    {
        flickerLight = GetComponent<Light>();
        if (flickerLight == null)
        {
            Debug.LogError("⚠️ No hay Light en este objeto. Agregá un componente Light.");
            enabled = false;
            return;
        }

        // Obtener o agregar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar AudioSource para sonidos ambientales
        audioSource.spatialBlend = 1f; // Sonido 3D
        audioSource.volume = volumenSonido;

        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Cambiar la intensidad aleatoriamente
            flickerLight.intensity = Random.Range(minIntensity, maxIntensity);

            // Reproducir sonido ocasionalmente (no en cada parpadeo)
            if (reproducirSonido && sonidoParpadeo != null && Random.value > 0.7f)
            {
                audioSource.PlayOneShot(sonidoParpadeo, volumenSonido);
            }

            // Resetear el temporizador
            timer = Random.Range(minTime, maxTime);
        }
    }
}