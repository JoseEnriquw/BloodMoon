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
    // Start is called before the first frame update
    private void Awake()
    {
        if(gameManager == null)
        {
            DontDestroyOnLoad(gameManager);
            gameManager = this;
        }else if(gameManager != this)
        {
            Destroy(gameManager);   
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
