using UnityEngine;

public class PulsarLuz : MonoBehaviour
{
    [Header("Configuración del pulso")]
    public float intensidadMin = 1f;     // Intensidad mínima
    public float intensidadMax = 3f;     // Intensidad máxima
    public float velocidad = 2f;         // Velocidad del cambio

    private Light luz;
    private float fase;

    void Start()
    {
        luz = GetComponent<Light>();
        if (luz == null)
        {
            Debug.LogWarning(" No se encontró un componente Light en " + gameObject.name);
        }

        // Arranca desde una fase aleatoria para evitar sincronizar luces iguales
        fase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (luz == null) return;

        // Oscila suavemente entre intensidadMin e intensidadMax
        float t = (Mathf.Sin(Time.time * velocidad + fase) + 1f) / 2f;
        luz.intensity = Mathf.Lerp(intensidadMin, intensidadMax, t);
    }
}
