using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReproducirCinematicas : MonoBehaviour
{

    public float duracionFadeOut = 2f;
    public Image pantallaNegra;
    public float duracionFadePantalla = 2f;
    public AudioSource desplegarpestañas;



    public void CinematicaGanar()
    {
      
        ReproducirCinematica("CinematicaGanar");
    }


  public void CinematicaPerdiste()
    {
       
        ReproducirCinematica("CinematicaPerdiste");
    }

  public void CinematicaPerderDisturbios()
    {
      
        ReproducirCinematica("CinematicaPerderDisturbios");
    }


  public void CinematicaRenunciar()
    {
        
        ReproducirCinematica("CinematicaRenunciar");
    }

    public void ReproducirCinematica(string nombreCinematica)
    {
        desplegarpestañas.Play();
        StartCoroutine(CerrarCinematicaCoroutine(nombreCinematica));
    }

    private IEnumerator CerrarCinematicaCoroutine(string nombreCinematica)
    {
        yield return StartCoroutine(FadeOutPantalla());
        ChangeScene(nombreCinematica);
        GameData.NivelActual = 1; 
    }

    private IEnumerator FadeOutPantalla()
    {
        pantallaNegra.gameObject.SetActive(true);

        Color colorFinal = new Color(0, 0, 0, 1);
        Color colorInicial = new Color(0, 0, 0, 0);

        pantallaNegra.color = colorInicial;

        for (float t = 0; t < duracionFadePantalla; t += Time.deltaTime)
        {
            pantallaNegra.color = Color.Lerp(colorInicial, colorFinal, t / duracionFadePantalla);
            yield return null;
        }

        pantallaNegra.color = colorFinal;
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
