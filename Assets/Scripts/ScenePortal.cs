using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.gameManager.ChangeScene(sceneToLoad);
        }
    }
}
