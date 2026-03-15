using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerNext : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entra es el jugador
        if (other.CompareTag("Player"))
        {
            // Obtiene el índice de la escena actual
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Calcula el índice de la siguiente escena
            int nextSceneIndex = currentSceneIndex + 1;

            // Verifica si la siguiente escena existe en los Build Settings
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No hay una escena siguiente configurada en los Build Settings.");
            }
        }
    }
}
