using UnityEngine;

public class UnlockCursor : MonoBehaviour
{
    void Start()
    {
        // Desbloquear y mostrar el cursor al cargar el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}