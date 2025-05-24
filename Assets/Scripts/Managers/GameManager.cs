using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI bullets;
    [SerializeField] TextMeshProUGUI ruins;

    [SerializeField] PlayerData playerData;

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
}
