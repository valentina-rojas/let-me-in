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
    public RectTransform panelAyuda;
    public RectTransform panelSintomas;

    public RectTransform panelSintomasNivel1;
    public RectTransform panelSintomasNivel2;

    public Button botonOpciones;
    public Button botonAyuda;
    public Button botonSintomas;


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
        CerrarTodosLosPaneles();
        panelOpciones.gameObject.SetActive(true);
        //  Time.timeScale = 0f;
    }

    public void CerrarOpciones()
    {
        panelOpciones.gameObject.SetActive(false);
        //Time.timeScale = 1f;
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

    public void AbrirAyuda()
    {
        CerrarTodosLosPaneles();
        panelAyuda.gameObject.SetActive(true);
    }

    public void CerrarAyuda()
    {
        panelAyuda.gameObject.SetActive(false);

    }

    public void AbrirSintomas()
    {
        CerrarTodosLosPaneles();
        panelSintomas.gameObject.SetActive(true);

        if (GameData.NivelActual == 1)
        {
            panelSintomasNivel2.gameObject.SetActive(false);
            panelSintomasNivel1.gameObject.SetActive(true);
        }

        if (GameData.NivelActual == 2)
        {
            panelSintomasNivel1.gameObject.SetActive(false);
            panelSintomasNivel2.gameObject.SetActive(true);
        }

    }

    public void CerrarSintomas()
    {
        panelSintomas.gameObject.SetActive(false);
    }

    // Método para cerrar todos los paneles
    public void CerrarTodosLosPaneles()
    {
        panelOpciones.gameObject.SetActive(false);
        panelAyuda.gameObject.SetActive(false);
        panelSintomas.gameObject.SetActive(false);
    }

    public void DesactivarBotonesVentanas()
    {

        botonAyuda.interactable = false;
        botonOpciones.interactable = false;
        botonSintomas.interactable = false;
    }

    public void ActivarBotonesVentanas()
    {

        botonAyuda.interactable = true;
        botonOpciones.interactable = true;
        botonSintomas.interactable = true;
    }

}
