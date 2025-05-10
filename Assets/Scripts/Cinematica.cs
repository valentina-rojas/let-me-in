using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Analytics;
using static EventManager;


public class Cinematica : MonoBehaviour
{
    public Image imagen;
    public TextMeshProUGUI texto;
    public Sprite[] imagenes;
    public string[] textos;
    public float duracionPorImagen = 5f;
    public float velocidadDeTipeo = 0.05f;
    public AudioSource musica;
    public float duracionFadeOut = 2f;
    public Image pantallaNegra;
    public float duracionFadePantalla = 2f;


    public AudioClip[] sonidos;
    public AudioSource audioSource;
    public AudioSource sonidoCerrar;

    private bool cinematicaCerrada = false;


    private void Start()
    {
        StartCoroutine(ReproducirCinematica());
    }

    private IEnumerator ReproducirCinematica()
    {
        if (imagenes.Length != textos.Length)
        {
            Debug.LogError("El número de imágenes y textos no coincide.");
            yield break;
        }

        for (int i = 0; i < imagenes.Length; i++)
        {
            // Cambia la imagen
            imagen.sprite = imagenes[i];


            // Si hay un sonido asociado a la imagen, lo reproduce
            if (i < sonidos.Length && sonidos[i] != null)
            {
                audioSource.clip = sonidos[i];
                audioSource.Play();
            }


            yield return StartCoroutine(TipearTexto(textos[i]));

            yield return new WaitForSeconds(duracionPorImagen);
        }

        CerrarCinematica();
    }

    private IEnumerator TipearTexto(string textoCompleto)
    {
        texto.text = "";
        foreach (char letra in textoCompleto.ToCharArray())
        {
            texto.text += letra;
            yield return new WaitForSeconds(velocidadDeTipeo);
        }
    }

    private IEnumerator FadeOutMusica()
    {
        float inicioVolumen = musica.volume;

        for (float t = 0; t < duracionFadeOut; t += Time.deltaTime)
        {
            musica.volume = Mathf.Lerp(inicioVolumen, 0, t / duracionFadeOut);
            yield return null;
        }

        musica.volume = 0;
        musica.Stop();
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

    public void OmitirCinematica()
    {
        if (cinematicaCerrada) return;
        cinematicaCerrada = true;

        RegisterEndCinEvent(true);
        sonidoCerrar.Play();
        StartCoroutine(CerrarCinematicaCoroutine());
    }

    public void CerrarCinematica()
    {
        if (cinematicaCerrada) return;
        cinematicaCerrada = true;

        RegisterEndCinEvent(false);
        sonidoCerrar.Play();
        StartCoroutine(CerrarCinematicaCoroutine());
    }



    private void RegisterEndCinEvent(bool cinSkipped)
    {
        Debug.Log("EndCinEvent");

        EndCinEvent endCin = new EndCinEvent();
        endCin.cinSkipped = cinSkipped;

#if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(endCin);
#else
        Debug.Log($"[ANALYTICS] Evento EndCinEvent registrado con cinSkipped = {cinSkipped}");
#endif
    }


    private IEnumerator CerrarCinematicaCoroutine()
    {
        yield return StartCoroutine(FadeOutMusica());
        yield return StartCoroutine(FadeOutPantalla());
        ChangeScene("Gameplay");
    }



    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }


}
