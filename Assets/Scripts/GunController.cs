using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float fireRate = 0.5f;
    public int maxAmmo = 12;
    public float reloadTime = 1.5f;

    [Header("Projectile Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 50f;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public Inventario inventario;

    [Header("Pickup System")]
    public bool tieneArma = false;

    [Header("Input Settings")]
    public bool usarRatonParaDisparar = true; // TRUE = solo clic izquierdo del ratón
    public KeyCode teclaDiparoAlternativa = KeyCode.None; // Opcional: tecla adicional para disparar

    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;

    void Start()
    {
        Debug.Log("=== GUN CONTROLLER INICIADO (MODO PROYECTIL) ===");

        currentAmmo = maxAmmo;

        if (fpsCam == null)
        {
            fpsCam = Camera.main;
            if (fpsCam == null)
            {
                fpsCam = FindObjectOfType<Camera>();
            }
        }

        if (fpsCam != null)
        {
            Debug.Log("Camara encontrada: " + fpsCam.name);
        }
        else
        {
            Debug.LogError("NO SE ENCONTRO CAMARA - El arma no funcionara correctamente");
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("FALTA ASIGNAR EL BULLET PREFAB en el Inspector");
        }

        if (firePoint == null)
        {
            Debug.LogWarning("No hay Fire Point asignado, usando la posicion de la pistola");
            firePoint = transform;
        }

        if (inventario == null)
        {
            inventario = FindObjectOfType<Inventario>();
            if (inventario == null)
            {
                Debug.LogWarning("No se encontró el script Inventario - El arma funcionará normalmente sin restricciones de inventario");
            }
        }

        Debug.Log("Municion inicial: " + currentAmmo);
    }

    void Update()
    {
        if (!tieneArma)
            return;

        if (inventario != null && inventario.InventarioEstaAbierto())
        {
            return;
        }

        if (isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Tecla R presionada - Iniciando recarga");
            StartReload();
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Sin municion - Recargando automaticamente");
            StartReload();
            return;
        }

        // SOLUCIÓN: Usar solo el botón del ratón
        bool disparar = false;

        if (usarRatonParaDisparar)
        {
            // Solo clic izquierdo del ratón (botón 0)
            disparar = Input.GetMouseButtonDown(0);
        }
        else
        {
            // Usar Fire1 (no recomendado si tienes problemas con el control)
            disparar = Input.GetButtonDown("Fire1");
        }

        // Disparo con tecla alternativa (opcional)
        if (teclaDiparoAlternativa != KeyCode.None && Input.GetKeyDown(teclaDiparoAlternativa))
        {
            disparar = true;
        }

        if (disparar && Time.time >= nextTimeToFire)
        {
            Debug.Log("Fire detectado - Llamando a Shoot()");
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("========================================");
        Debug.Log("DISPARANDO PROYECTIL - Balas restantes: " + currentAmmo);

        currentAmmo--;

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (gunAudio != null && shootSound != null)
        {
            gunAudio.PlayOneShot(shootSound);
        }

        if (bulletPrefab == null || fpsCam == null)
        {
            Debug.LogError("Falta bullet prefab o camara");
            return;
        }

        Vector3 spawnPosition = firePoint.position;
        Vector3 shootDirection = fpsCam.transform.forward;
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, shootRotation);

        if (bullet != null)
        {
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.speed = bulletSpeed;
            }
            else
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = shootDirection * bulletSpeed;
                }
            }
        }

        Debug.Log("========================================");
    }

    void StartReload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
            Debug.Log("RECARGANDO - Tiempo de recarga: " + reloadTime + " segundos");

            if (gunAudio != null && reloadSound != null)
            {
                gunAudio.PlayOneShot(reloadSound);
            }

            Invoke("FinishReload", reloadTime);
        }
    }

    void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("RECARGA COMPLETA - Municion restaurada a: " + currentAmmo);
    }

    public void RecogerArma()
    {
        tieneArma = true;
        Debug.Log("Has recogido el arma.");
    }
}