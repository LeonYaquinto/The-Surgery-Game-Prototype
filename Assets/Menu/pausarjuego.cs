using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Agregar esto al inicio

public class pausarjuego : MonoBehaviour
{
    public GameObject menupausa;
    public bool juegopausado = false;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {

       
         if (juegopausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void VolverMenu()
    {
        // Asegurarse de que el juego vuelva a la velocidad normal
        Time.timeScale = 1;
        // Cargar la escena con build index 0
        SceneManager.LoadScene(0);
    }

    public void Reanudar()
    {
        Debug.Log("Reanudar() llamado");
        menupausa.SetActive(false);
        Time.timeScale = 1;
        juegopausado = false;

        // Bloquea y oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pausar()
    {
        menupausa.SetActive(true);
        Time.timeScale = 0;
        juegopausado = true;
    }

    private void LateUpdate()
    {
        if (juegopausado)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


}

