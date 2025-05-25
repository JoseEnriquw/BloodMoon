using UnityEngine;

public class ExitButtonHandler : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");

        // Guardado manual (si quer�s)
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.SaveData();
        }

        Application.Quit();
    }
}
