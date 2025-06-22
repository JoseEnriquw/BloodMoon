using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    
    [SerializeField] int cantRuna;
    [SerializeField] PlayerData playerData;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
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
