using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBoss : MonoBehaviour
{
    public GameObject portal; // asigna tu particula de portal en el inspector
    public PlayerData playerData; // referencia al ScriptableObject
    public string finalSceneName = "NombreDeTuEscenaFinal";

    //private void Start()
    //{
    //    portal.SetActive(false); // asegurarse de que el portal está desactivado al inicio
    //}

    //private void Update()
    //{
    //    if (playerData.Runes >= 3 && !portal.activeSelf)
    //    {
    //        portal.SetActive(true); // activamos el portal
    //        Debug.Log("Portal activado!");
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player") && portal.activeSelf)
    //    {
    //        Debug.Log("Entrando al portal...");
    //        SceneManager.LoadScene(finalSceneName); // carga la escena final
    //    }
    //}
}
