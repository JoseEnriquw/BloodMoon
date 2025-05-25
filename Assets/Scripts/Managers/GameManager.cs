using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    //[SerializeField] TextMeshProUGUI health;
    //[SerializeField] TextMeshProUGUI bullets;
    //[SerializeField] TextMeshProUGUI ruins;
   // private bool ExecutedCoroutine;
    [SerializeField] PlayerData playerData;
    GameInfo gameInfo;
    private string savePath;

    float _healthTemp= 0f;
    int _bulletTemp= 0;
    int _ruinsTemp= 0;
    // Start is called before the first frame update
    private void Awake()
    {
       
        savePath = Application.persistentDataPath + "/GameInfo.dat";
        if (gameManager != null && gameManager != this)
        {
            Destroy(this.gameObject);
            return;
        }

        gameManager = this;
        DontDestroyOnLoad(this.gameObject);

    }
    void Start()
    {
        // UpdateUi();
        SceneManager.sceneLoaded += OnSceneLoaded;
        _healthTemp = playerData.Health;
        _bulletTemp = playerData.Bullets;
        _ruinsTemp = playerData.Runes;
        
        LoadData();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_healthTemp != playerData.Health ||
            _bulletTemp != playerData.Bullets ||
            _ruinsTemp != playerData.Runes)
        {
            _healthTemp = playerData.Health;
            _bulletTemp = playerData.Bullets;
            _ruinsTemp = playerData.Runes;

            UIManager.Instance?.UpdateHUD(_healthTemp, _bulletTemp, _ruinsTemp);
        }
       
    }

   

    public void SaveData(string sceneName = null)
    {
        string path = Application.persistentDataPath + "/GameInfo.dat";

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (FileStream fileStream = File.Create(path))
            {
                GameInfo gameInfo = new GameInfo
                {
                    Health = playerData.Health,
                    Bullets = playerData.Bullets,
                    Runes = playerData.Runes,
                    LastScene = sceneName ?? SceneManager.GetActiveScene().name
                };

                bf.Serialize(fileStream, gameInfo);
                
                fileStream.Flush();
            }

            Debug.Log("Datos guardados correctamente en: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al guardar datos: " + e.Message);
        }
    }


    public void LoadData()
    {
        string path = Application.persistentDataPath + "/GameInfo.dat";
        if (File.Exists(path))
        {
            Debug.Log(Application.persistentDataPath);

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length > 0)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    using (FileStream fileStream = File.Open(path, FileMode.Open))
                    {
                        gameInfo = (GameInfo)bf.Deserialize(fileStream);
                        playerData.Health = gameInfo.Health;
                        playerData.Bullets = gameInfo.Bullets;
                        playerData.Runes = gameInfo.Runes;
                    }
                    if (!string.IsNullOrEmpty(gameInfo.LastScene) && gameInfo.LastScene != SceneManager.GetActiveScene().name)
                    {
                        SceneManager.LoadScene(gameInfo.LastScene);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error al cargar datos: " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("El archivo existe pero está vacío.");
            }
        }
        else
        {
            Debug.Log("No se encontró el archivo de guardado.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

   
    public void ChangeScene(string sceneName)
    {
        SaveData(sceneName);
        SceneManager.LoadScene(sceneName);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(UpdateHUDNextFrame());
        // UIManager.Instance?.UpdateHUD(playerData.Health, playerData.Bullets, playerData.Runes);
    }
    private IEnumerator UpdateHUDNextFrame()
    {
        while (UIManager.Instance == null)
        {
            yield return null; // espera 1 frame
        }

        UIManager.Instance?.UpdateHUD(playerData.Health, playerData.Bullets, playerData.Runes);
    }
}
