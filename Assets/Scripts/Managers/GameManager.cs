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
    [SerializeField] PlayerData playerData;
    GameInfo gameInfo;
    private string savePath;

    float _healthTemp= 0f;
    int _bulletTemp= 0;
    int _ruinsTemp= 0;
    bool juegoTerminado = false;

    public static GameManager Instance { get; private set; }
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
        if (Instance == null)
        {
            Instance = this;
            
        }

    }
    void Start()
    {
        // UpdateUi();
        //SceneManager.sceneLoaded += OnSceneLoaded;
        _healthTemp = playerData.Health;
        _bulletTemp = playerData.Bullets;
        _ruinsTemp = playerData.Runes;      
        
        
    }

   
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

   

    public void SaveData(int sceneIndex = -1)
    {
        string path = Application.persistentDataPath + "/GameInfo.dat";

        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            if (gameInfo == null)
                gameInfo = new GameInfo(); 

            
            gameInfo.Health = playerData.Health;
            gameInfo.Bullets = playerData.Bullets;
            gameInfo.Runes = playerData.Runes;
            gameInfo.LastSceneIndex = sceneIndex >= 0 ? sceneIndex : SceneManager.GetActiveScene().buildIndex;

           
            if (gameInfo.LevelsWithRune == null)
                gameInfo.LevelsWithRune = new List<int>();

            using (FileStream fileStream = File.Create(path))
            {               

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

                        if (gameInfo.LevelsWithRune == null)
                            gameInfo.LevelsWithRune = new List<int>();
                    }
                    int currentIndex = SceneManager.GetActiveScene().buildIndex;
                    //if (gameInfo.LastSceneIndex != currentIndex)
                    //{
                    //    OnSceneLoaded();
                    //    GameSceneManager.Instance.LoadSceneByIndex(gameInfo.LastSceneIndex);
                    //}
                    //else
                    //{
                    //    OnSceneLoaded();
                    //    GameSceneManager.Instance.LoadSceneByIndex(1);
                    //}
                    OnSceneLoaded();
                    GameSceneManager.Instance.LoadSceneByIndex(gameInfo.LastSceneIndex);
                
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error al cargar datos: " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("El archivo existe pero est� vac�o.");
            }
        }
        else
        {
            Debug.Log("No se encontr� el archivo de guardado.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void OnSceneLoaded()
    {
        StartCoroutine(UpdateHUDNextFrame());
       
    }
    private IEnumerator UpdateHUDNextFrame()
    {
        while (UIManager.Instance == null)
        {
            yield return null; 
        }

        UIManager.Instance?.UpdateHUD(playerData.Health, playerData.Bullets, playerData.Runes);
    }
    public void ResetData()
    {
        string path = Application.persistentDataPath + "/GameInfo.dat";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("🗑 Archivo de guardado eliminado.");
        }
        
        playerData.Health = 100f;
        playerData.Bullets = 0;
        playerData.Runes = 0;
    }

    public bool HasRuneForLevel(int sceneIndex)
    {
        return gameInfo != null && gameInfo.LevelsWithRune.Contains(sceneIndex);
    }

    public void MarkRuneAsCollected(int sceneIndex)
    {
        if (gameInfo == null)
            gameInfo = new GameInfo();

        if (gameInfo.LevelsWithRune == null)
            gameInfo.LevelsWithRune = new List<int>();

        if (!gameInfo.LevelsWithRune.Contains(sceneIndex))
        {
            gameInfo.LevelsWithRune.Add(sceneIndex);
            SaveData(); 
        }
    }

    public void Perder()
    {
        //if (juegoTerminado) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //juegoTerminado = true;
        Debug.Log("hola");
        Time.timeScale = 0;
        UIManager.Instance.MostrarDerrota();
    }
    public void Reiniciarnivel()
    {
        SaveData();

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
                        playerData.Health = 100;
                        playerData.Bullets = 0;
                        playerData.Runes = gameInfo.Runes;

                        if (gameInfo.LevelsWithRune == null)
                            gameInfo.LevelsWithRune = new List<int>();
                    }

                    OnSceneLoaded();
                    GameSceneManager.Instance.LoadSceneByIndex(gameInfo.LastSceneIndex);
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

    
}
