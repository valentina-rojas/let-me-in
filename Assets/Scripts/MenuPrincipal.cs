using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public static MenuPrincipal instance;

    public RectTransform panelCreditos;
    public RectTransform panelAyuda;
    public AudioSource sonidoCerrar;

private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    
    public void AbrirCreditos()
    {
        sonidoCerrar.Play();
        panelCreditos.gameObject.SetActive(true);
    }

    public void CerrarCreditos()
    {
        sonidoCerrar.Play();
        panelCreditos.gameObject.SetActive(false);
    }


    public void AbrirAyuda()
    {
        sonidoCerrar.Play();
        panelAyuda.gameObject.SetActive(true);
    }

    public void CerrarAyuda()
    {
        sonidoCerrar.Play();
        panelAyuda.gameObject.SetActive(false);
    }


    public void CerrarJuego()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }


public void IniciarJuego()
{
    sonidoCerrar.Play();
    ChangeScene("Cinematica"); 
}

public void ChangeScene(string name)
{
    SceneManager.LoadScene(name);
}

}
