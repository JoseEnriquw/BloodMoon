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
        audioMixer.SetFloat("Volumen", valor);
        audioMixer.SetFloat("VolumenMusica", valor);
        audioMixer.SetFloat("VolumenSFX", valor);
    }

    public void SetVolumenMusica(float valor)
    {
        audioMixer.SetFloat("VolumenMusica", valor);
    }

    public void SetVolumenSFX(float valor)
    {
        audioMixer.SetFloat("VolumenSFX", valor);
    }

    public void StopMusic()
    {
        GetComponent<AudioSource>()?.Stop();
    }

}
