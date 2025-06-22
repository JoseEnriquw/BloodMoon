using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    
    [SerializeField] int cantRuna;
    [SerializeField] PlayerData playerData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (cantRuna == playerData.Runes)
            {
                GameSceneManager.Instance.LoadNextScene();
            }
            else
            {
                UIManager.Instance.FaltaRuna();
            }
        }
    }

}
