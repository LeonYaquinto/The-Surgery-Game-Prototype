using UnityEngine;

public class LinternaPickup : MonoBehaviour
{
    [Header("Sonido al recoger")]
    public AudioClip sonidoRecoger;
    [Range(0f, 1f)]
    public float volumen = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Busca el script de control de linterna en el jugador
            LinternaController controller = other.GetComponent<LinternaController>();
            if (controller != null)
            {
                controller.RecogerLinterna();

                // Reproducir sonido antes de destruir
                if (sonidoRecoger != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position, volumen);
                }

                Destroy(gameObject); // elimina la linterna del suelo
            }
        }
    }
}