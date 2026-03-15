using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransicionMuerte : MonoBehaviour
{
    public static TransicionMuerte instance;
    public Image panelNegro;
    public float duracionFade = 2f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // AGREGAR ESTO: Asegurar que el panel también persista
            if (panelNegro != null)
            {
                DontDestroyOnLoad(panelNegro.canvas.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MorirYCambiarEscena(string escenaIndex)
    {
        StartCoroutine(FadeYCambiarEscena(escenaIndex));
    }

    IEnumerator FadeYCambiarEscena(string escenaIndex)
    {
        // Verificar que el panel existe
        if (panelNegro == null)
        {
            SceneManager.LoadScene(escenaIndex);
            yield break;
        }

        // Hacer visible el panel
        panelNegro.gameObject.SetActive(true);
        panelNegro.raycastTarget = false;

        // Fade a negro
        float tiempo = 0;
        Color color = panelNegro.color;
        color.a = 0;
        panelNegro.color = color;

        while (tiempo < duracionFade)
        {
            tiempo += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0, 1, tiempo / duracionFade);
            panelNegro.color = color;
            yield return null;
        }

        // Asegurar que quedó completamente negro
        color.a = 1;
        panelNegro.color = color;

        // Esperar medio segundo en negro
        yield return new WaitForSecondsRealtime(0.5f);

        // Cambiar escena
        SceneManager.LoadScene(escenaIndex);

        // ? AGREGAR ESTO: Esperar a que cargue la escena
        yield return new WaitForSecondsRealtime(0.2f);

        // ? AGREGAR ESTO: Hacer fade out (volver transparente)
        tiempo = 0;
        while (tiempo < 1f)
        {
            tiempo += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1, 0, tiempo / 1f);
            panelNegro.color = color;
            yield return null;
        }

        // ? AGREGAR ESTO: Ocultar el panel
        panelNegro.gameObject.SetActive(false);
        Time.timeScale = 1; // Restaurar velocidad del juego
    }
}