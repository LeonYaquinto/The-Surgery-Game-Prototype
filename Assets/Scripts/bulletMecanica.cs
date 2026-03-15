using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 50f;
    public float damage = 25f;
    public float lifetime = 5f;

    [Header("Sounds")]
    public AudioClip enemyImpactSound;  // 💥 Sonido al impactar enemigo
    public AudioClip objectImpactSound; // 💢 Sonido al impactar cualquier otra cosa

    [Header("Effects")]
    public GameObject impactEffect;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Dar velocidad a la bala
        rb.velocity = transform.forward * speed;

        // Destruir la bala después de X segundos si no golpea nada
        Destroy(gameObject, lifetime);

        Debug.Log("Bala disparada a velocidad: " + speed);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bala golpeó: " + other.name);

        bool hitEnemy = false;

        // Verificar si golpeó un zombie
        Enemigo zombie = other.GetComponent<Enemigo>();
        if (zombie != null)
        {
            Debug.Log("¡Bala golpeó al zombie! Aplicando " + damage + " de daño");
            zombie.TakeDamage(damage);
        }

        // Crear efecto de impacto
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(impact, 2f);
        }

        // 🔊 Reproducir sonido según el tipo de impacto
        if (hitEnemy && enemyImpactSound != null)
        {
            AudioSource.PlayClipAtPoint(enemyImpactSound, transform.position);
        }
        else if (!hitEnemy && objectImpactSound != null)
        {
            AudioSource.PlayClipAtPoint(objectImpactSound, transform.position);
        }

        // Destruir la bala
        Destroy(gameObject);
    }
}