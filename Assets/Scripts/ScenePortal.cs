using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] int cantRuna;
    [SerializeField] PlayerData playerData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cantRuna == playerData.Runes)
            {
                GameManager.gameManager.ChangeScene(sceneToLoad);
            }
            else
            {
                UIManager.Instance.FaltaRuna();
            }
        }
    }
}
