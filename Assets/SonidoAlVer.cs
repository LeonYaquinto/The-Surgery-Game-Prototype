using UnityEngine;

public class SonidoAlVer : MonoBehaviour
{
    [Header("Configuración")]
    public AudioClip sonidoAlVer;
    public string tagCamara = "MainCamera";

    [Header("Opciones Avanzadas")]
    [Range(0f, 1f)]
    public float volumen = 1f;
    public bool requiereLineaDeVista = true; // Manténlo en true para evitar paredes

    private AudioSource audioSource;
    private bool yaReproducido = false;
    private Camera camaraPrincipal;

    void Start()
    {
        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = sonidoAlVer;
        audioSource.playOnAwake = false;
        audioSource.volume = volumen;
        audioSource.spatialBlend = 0f;

        // Buscar cámara
        GameObject camObj = GameObject.FindGameObjectWithTag(tagCamara);
        camaraPrincipal = camObj ? camObj.GetComponent<Camera>() : Camera.main;

        if (camaraPrincipal == null)
            Debug.LogError("No se encontró la cámara principal con el tag 'MainCamera'.");
    }

    void Update()
    {
        if (yaReproducido || camaraPrincipal == null)
            return;

        if (EstaVisibleEnPantalla() && TieneLineaDeVista())
        {
            ReproducirSonido();
        }
    }

    bool EstaVisibleEnPantalla()
    {
        Vector3 punto = camaraPrincipal.WorldToViewportPoint(transform.position);

        // Solo si está dentro de la pantalla y frente a la cámara
        return (punto.z > 0 && punto.x > 0 && punto.x < 1 && punto.y > 0 && punto.y < 1);
    }

    bool TieneLineaDeVista()
    {
        if (!requiereLineaDeVista) return true;

        Vector3 direccion = transform.position - camaraPrincipal.transform.position;
        float distancia = direccion.magnitude;
        direccion.Normalize();

        // Si el primer objeto que toca el rayo es ESTE, entonces hay línea de vista
        if (Physics.Raycast(camaraPrincipal.transform.position, direccion, out RaycastHit hit, distancia))
        {
            return hit.collider.gameObject == gameObject ||
                   hit.collider.transform.IsChildOf(transform);
        }

        return false;
    }

    void ReproducirSonido()
    {
        if (audioSource != null && sonidoAlVer != null)
        {
            audioSource.Play();
            yaReproducido = true;
            Debug.Log("?? Sonido reproducido al ver: " + gameObject.name);
        }
    }

    public void ResetearSonido()
    {
        yaReproducido = false;
    }
}
