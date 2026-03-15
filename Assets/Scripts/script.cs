using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Desbloquear y mostrar el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cargar la escena del menú principal (índice 0)
        SceneManager.LoadScene(0);
    }
}