using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voicesSlider;

    public AudioMixer audioMixer;

    public RectTransform panelOpciones;

public Button botonOpciones;
 

    void Start()
    {
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;
        voicesSlider.value = 0.5f;

        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetVoicesVolume(voicesSlider.value);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        voicesSlider.onValueChanged.AddListener(SetVoicesVolume);
    }

    public void AbrirOpciones()
    {
        panelOpciones.gameObject.SetActive(true);
        Time.timeScale = 0f;      
    }

    public void CerrarOpciones()
    {
        panelOpciones.gameObject.SetActive(false);
        Time.timeScale = 1f;  
    }

    public void SetMusicVolume(float volume)
    {
        float minVolume = 0.0001f;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(volume, minVolume)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        float minVolume = 0.0001f;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(volume, minVolume)) * 20);
    }

    public void SetVoicesVolume(float volume)
    {
        float minVolume = 0.0001f;
        audioMixer.SetFloat("VoicesVolume", Mathf.Log10(Mathf.Max(volume, minVolume)) * 20);
    }


}
