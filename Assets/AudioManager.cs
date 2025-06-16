using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer audioMixer;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void SetVolumenGeneral(float valor)
    {
        audioMixer.SetFloat("VolumenGeneral", Mathf.Log10(valor) * 20);
    }

    public void SetVolumenMusica(float valor)
    {
        audioMixer.SetFloat("VolumenMusica", Mathf.Log10(valor) * 20);
    }

    public void SetVolumenSFX(float valor)
    {
        audioMixer.SetFloat("VolumenSFX", Mathf.Log10(valor) * 20);
    }
}
