using UnityEngine;

public class ExitButtonHandler : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");

        // Guardado manual (si querés)
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.SaveData();
        }

        Application.Quit();
    }
}
