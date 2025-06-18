using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelConfig;
    public GameObject panelCreditos;


    public void MostrarConfig()
    {
        
        panelMenu.SetActive(false);
        panelConfig.SetActive(true);
    }
    public void MostrarCreditos()
    {
        
        panelMenu.SetActive(false);
        panelCreditos.SetActive(true);
    }
    public void BotonVolver()
    {
        panelCreditos.SetActive(false);
        panelConfig.SetActive(false);
        panelMenu.SetActive(true);
        
    }
}
