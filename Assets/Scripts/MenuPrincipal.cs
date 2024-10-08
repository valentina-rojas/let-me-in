using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{

    public RectTransform panelCreditos;
    public RectTransform panelAyuda;

    public void AbrirCreditos()
    {
        panelCreditos.gameObject.SetActive(true);
    }

    public void CerrarCreditos()
    {
        panelCreditos.gameObject.SetActive(false);
    }


    public void AbrirAyuda()
    {
        panelAyuda.gameObject.SetActive(true);
    }

    public void CerrarAyuda()
    {
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
