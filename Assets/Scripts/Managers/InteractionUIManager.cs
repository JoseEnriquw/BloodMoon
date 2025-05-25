using UnityEngine;
using TMPro;

public class InteractionUIManager : MonoBehaviour
{
    public static InteractionUIManager Instance;

    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        interactionUI.SetActive(false);
    }

    public void ShowInteraction(string message)
    {
        interactionText.text = message;
        interactionUI.SetActive(true);
    }

    public void HideInteraction()
    {
        interactionUI.SetActive(false);
    }
}
