using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{

    public RectTransform panelCreditos;
    public RectTransform panelAyuda;
    public AudioSource sonidoCerrar;

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

}
