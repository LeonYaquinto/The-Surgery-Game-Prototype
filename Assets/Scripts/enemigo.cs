using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemigo : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator ani;
    public Quaternion angulo;
    public float grado;
    public GameObject target;
    public bool atacando;
    public int damage = 1;
    private bool puedeAtacar = true;
    private bool jugadorMuerto = false;

    [Header("Health Settings")]
    public float maxHealth = 1f;
    private float currentHealth;

    [Header("Death Effects")]
    public GameObject bloodEffect;
    public GameObject deathEffect;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip detectionSound; // Sonido al detectar jugador

    [Range(0f, 1f)]
    public float walkVolume = 0.5f;
    [Range(0f, 1f)]
    public float runVolume = 0.6f;
    [Range(0f, 1f)]
    public float attackVolume = 0.8f;
    [Range(0f, 1f)]
    public float deathVolume = 1f;
    [Range(0f, 1f)]
    public float detectionVolume = 0.9f;

    private bool muerto = false;
    private bool isPlayingWalkSound = false;
    private bool isPlayingRunSound = false;
    private bool jugadorDetectado = false; // Para reproducir sonido solo una vez

    void Start()
    {
        ani = GetComponent<Animator>();
        target = GameObject.Find("First Person Controller Variant");
        currentHealth = maxHealth;

        // Configurar AudioSource si no existe
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        audioSource.spatialBlend = 1f; // Audio 3D
        audioSource.maxDistance = 20f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    public void TakeDamage(float amount)
    {
        if (muerto) return;

        currentHealth -= amount;

        if (bloodEffect != null)
        {
            GameObject blood = Instantiate(bloodEffect, transform.position + Vector3.up, Quaternion.identity);
            Destroy(blood, 1f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        muerto = true;
        Debug.Log($"{name} murió y reproduce animación de caída");

        // Detener sonidos de movimiento
        StopMovementSounds();

        // Reproducir sonido de muerte con AudioSource separado
        if (deathSound != null)
        {
            // Crear un objeto temporal para el sonido de muerte
            GameObject audioObject = new GameObject("DeathSound");
            audioObject.transform.position = transform.position;
            AudioSource tempAudioSource = audioObject.AddComponent<AudioSource>();
            tempAudioSource.clip = deathSound;
            tempAudioSource.volume = deathVolume;
            tempAudioSource.spatialBlend = 1f; // Audio 3D
            tempAudioSource.maxDistance = 20f;
            tempAudioSource.Play();

            // Destruir el objeto de audio después de que termine
            Destroy(audioObject, deathSound.length);

            Debug.Log("Reproduciendo sonido de muerte");
        }

        if (ani != null)
        {
            ani.SetBool("walk", false);
            ani.SetBool("run", false);
            ani.SetBool("attack", false);
            ani.SetBool("fall", true);
        }

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 5f);
    }

    public void Comportamiento_Enemigo()
    {
        if (jugadorMuerto || muerto) return;

        if (Vector3.Distance(transform.position, target.transform.position) > 5)
        {
            // Si el jugador se alejó, resetear la detección
            jugadorDetectado = false;

            ani.SetBool("run", false);
            StopRunSound();

            cronometro += 1 * Time.deltaTime;
            if (cronometro >= 1.5)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }
            switch (rutina)
            {
                case 0:
                    ani.SetBool("walk", false);
                    StopWalkSound();
                    break;
                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;
                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    ani.SetBool("walk", true);
                    PlayWalkSound();
                    break;
            }
        }
        else
        {
            // Reproducir sonido de detección la primera vez
            if (!jugadorDetectado)
            {
                PlaySound(detectionSound, detectionVolume);
                jugadorDetectado = true;
                Debug.Log($"{name} detectó al jugador!");
            }

            if (Vector3.Distance(transform.position, target.transform.position) > 1 && !atacando)
            {
                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                ani.SetBool("walk", false);
                ani.SetBool("run", true);
                StopWalkSound();
                PlayRunSound();
                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
                ani.SetBool("attack", false);
            }
            else
            {
                ani.SetBool("walk", false);
                ani.SetBool("run", false);
                ani.SetBool("attack", true);
                StopMovementSounds();
                atacando = true;

                // Reproducir sonido de ataque
                PlaySound(attackSound, attackVolume);
            }
        }
    }

    void PlayWalkSound()
    {
        if (walkSound != null && !isPlayingWalkSound)
        {
            audioSource.clip = walkSound;
            audioSource.loop = true;
            audioSource.volume = walkVolume;
            audioSource.Play();
            isPlayingWalkSound = true;
        }
    }

    void StopWalkSound()
    {
        if (isPlayingWalkSound)
        {
            audioSource.Stop();
            isPlayingWalkSound = false;
        }
    }

    void PlayRunSound()
    {
        if (runSound != null && !isPlayingRunSound)
        {
            audioSource.clip = runSound;
            audioSource.loop = true;
            audioSource.volume = runVolume;
            audioSource.Play();
            isPlayingRunSound = true;
        }
    }

    void StopRunSound()
    {
        if (isPlayingRunSound)
        {
            audioSource.Stop();
            isPlayingRunSound = false;
        }
    }

    void StopMovementSounds()
    {
        StopWalkSound();
        StopRunSound();
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void Final_Ani()
    {
        ani.SetBool("attack", false);
        atacando = false;
        puedeAtacar = true;

        if (!jugadorMuerto)
        {
            jugadorMuerto = true;
            AtacarJugador();
        }
    }

    void AtacarJugador()
    {
        Time.timeScale = 0;
        if (TransicionMuerte.instance != null)
        {
            TransicionMuerte.instance.MorirYCambiarEscena("level 2");
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }

    void Update()
    {
        Comportamiento_Enemigo();
    }
}