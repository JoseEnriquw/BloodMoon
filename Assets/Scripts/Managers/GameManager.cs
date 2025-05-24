using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI bullets;
    [SerializeField] TextMeshProUGUI ruins;
    private bool ExecutedCoroutine;
    [SerializeField] PlayerData playerData;
    GameInfo gameInfo;

    float _healthTemp= 0f;
    int _bulletTemp= 0;
    int _ruinsTemp= 0;
    // Start is called before the first frame update
    private void Awake()
    {
        if(gameManager == null)
        {
            DontDestroyOnLoad(gameObject);
            gameManager = this;
        }
        else if(gameManager != this)
        {
            Destroy(gameManager);   
        }
    }
    void Start()
    {
        UpdateUi();

        _healthTemp = playerData.Health;
        _bulletTemp = playerData.Bullets;
        _ruinsTemp = playerData.Runes;

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        if (_healthTemp != playerData.Health)            
        {
            UpdateUi();
        }
        if (_bulletTemp != playerData.Bullets)
        {
            UpdateUi();
        }
        if (_ruinsTemp != playerData.Runes)
        {
            UpdateUi();
        }
    }

    private void UpdateUi()
    {
        health.text = playerData.Health.ToString();
        bullets.text = playerData.Bullets.ToString();
        ruins.text = playerData.Runes.ToString();
    }

    public void SaveData()
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
                    Runes = playerData.Runes
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
                        GameInfo gameInfo = (GameInfo)bf.Deserialize(fileStream);
                        playerData.Health = gameInfo.Health;
                        playerData.Bullets = gameInfo.Bullets;
                        playerData.Runes = gameInfo.Runes;
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

    public void NextScene()
    {
        var sceneNumber = SceneManager.GetActiveScene().buildIndex;
        var nextScene = sceneNumber + 1;
        if (sceneNumber != 4)
        {
            if (ExecutedCoroutine) return; // Evita llamadas repetidas
            ExecutedCoroutine = true;
            //UIManager.GetUIManager().ChangeLoadingBackGround(sceneNumber);

            // Suscribirse al evento una sola vez
            SceneManager.sceneLoaded += (scene, loadMode) => { UIManager.GetUIManager(); };

            // Inicia la carga de la escena con un delay
            StartCoroutine(EjecutarConDelay(5f, () =>
            {
                SceneManager.LoadScene(nextScene);
            }));
        }
        else
        {
           // UIManager.GetUIManager().HideTaskPanel();
            SceneManager.LoadScene(nextScene);
        }
    }
    IEnumerator EjecutarConDelay(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);

        // Código que se ejecuta después del delay
        Debug.Log($"Han pasado {seconds} segundos");
        action?.Invoke();
        ExecutedCoroutine = false;
    }
}
