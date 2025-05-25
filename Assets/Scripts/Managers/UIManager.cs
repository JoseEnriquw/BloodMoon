using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI bulletsText;
    [SerializeField] private TextMeshProUGUI ruinsText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdateHUD(float health, int bullets, int ruins)
    {
        if (healthText != null) healthText.text = health.ToString();
        if (bulletsText != null) bulletsText.text = bullets.ToString();
        if (ruinsText != null) ruinsText.text = ruins.ToString();
    }
}
