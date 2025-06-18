using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    [SerializeField] private GameObject hud;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI bulletsText;
    [SerializeField] private TextMeshProUGUI ruinsText;
    [SerializeField] private Image runa1;
    [SerializeField] private Image runa2;
    [SerializeField] private Image runa3;
    public GameObject cartelRunaFaltante;
    public GameObject panelVictoria;
    public GameObject panelDerrota;
    public GameObject panelPausa;
    private bool juegoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }
    
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
        if (!hud.activeSelf)
            hud.SetActive(true);

        if (healthText != null) healthText.text = health.ToString();
        if (healthSlider != null) healthSlider.value = health;

        if (bulletsText != null) bulletsText.text = bullets.ToString();
        if (ruinsText != null) ruinsText.text = ruins.ToString();
        if (ruins == 1) 
        {
            runa1.gameObject.SetActive(true);
        }
        if (ruins == 2)
        {
            runa2.gameObject.SetActive(true);
        }
        if (ruins == 3)
        {
            runa3.gameObject.SetActive(true);
        }
    }

    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;
        panelPausa.SetActive(juegoPausado);
        hud.SetActive(!juegoPausado);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !juegoPausado;
        Time.timeScale = juegoPausado ? 0 : 1;
    }
    public void ReanudarJuego()
    {
        juegoPausado = !juegoPausado;
        panelPausa.SetActive(juegoPausado);
        hud.SetActive(!juegoPausado);
        Time.timeScale = juegoPausado ? 0 : 1;
    }
    public void MostrarVictoria()
    {
        Time.timeScale = 0;
        hud.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelVictoria.SetActive(true);
    }
    public void BactoMenu()
    {
        panelVictoria.SetActive(false);
        GameSceneManager.Instance.LoadSceneByIndex(0);
    }
    public void MostrarDerrota()
    {
        Time.timeScale = 0;
        hud.SetActive(false);
        InteractionUIManager.Instance.HideInteraction();
        panelDerrota.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       
    }
    public void FaltaRuna()
    {
        StartCoroutine(MostrarTemporal(cartelRunaFaltante, 5f));
    }

    private System.Collections.IEnumerator MostrarTemporal(GameObject cartel, float duracion)
    {
        cartel.SetActive(true);
        yield return new WaitForSecondsRealtime(duracion);
        cartel.SetActive(false);
    }

    public bool IsHudActive()
    {
        return hud != null && hud.activeSelf;
    }
    public void ReintentarPartida()
    {
        if (juegoPausado)
        {
            TogglePausa();
        }
        panelDerrota.SetActive(false); // ocultar el panel de derrota
        Time.timeScale = 1f; // asegurar que el juego siga

        GameManager.gameManager.Reiniciarnivel(); // cargar partida
    }

    public void Guardar()
    {
        GameManager.gameManager.SaveData();
    }

}
