using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

public class UI_Manager : MonoBehaviour
{

    public RectTransform panelInicioDia;
    public TextMeshProUGUI textoInicioDia;
    public float velocidadTexto = 0.1f;
    public float duracionPanel = 1.0f;
    public float intervaloCursor = 0.5f;

    public RectTransform panelReporte;
    public RectTransform panelPerdiste;
    public TextMeshProUGUI mensajeReporte;
    public TextMeshProUGUI reporteText;

    public RectTransform indicaciones1;
    public RectTransform indicaciones2;
    public RectTransform indicaciones3;
    public RectTransform indicaciones4;
    public RectTransform indicaciones5;
    public TextMeshProUGUI omitirIndicaciones;

    public float duracionIndicaciones = 4f;

    public Button botonSiguienteNivel;
    public Button botonGanaste;
    public Button botonPerdiste;
    public Button botonPerdisteDisturbios;

    public event Action PanelInicioDesactivado;

    public DialogueManager dialogueManager;
    public s_GameManager gameManager;
    public CharactersManager charactersManager;
    public OptionsManager optionsManager;
    public InteractableObjects interactableObjects;

    public AudioSource audioTecleo;

    private Coroutine panelInicioDiaCoroutine;
    private bool cursorVisible = true;
    private float tiempoUltimaActualizacion;

    public Image imagenInicioDia;      // Referencia al componente Image en el panel de inicio del día
    public Sprite[] imagenesNiveles;   // Array de imágenes para los niveles



    public void MostrarInicioDia(string mensaje)
    {
        dialogueManager.panelDialogo.gameObject.SetActive(false);
        dialogueManager.panelRespuestas.gameObject.SetActive(false);

        interactableObjects.DesactivarEventTriggers();

        int nivelActual = gameManager.NivelActual - 1; // Si el nivel empieza en 1, resta 1 para usarlo como índice

        if (nivelActual >= 0 && nivelActual < imagenesNiveles.Length)
        {
            imagenInicioDia.sprite = imagenesNiveles[nivelActual];  // Asigna la imagen correspondiente al nivel actual
        }
        else
        {
            Debug.LogWarning("No hay imagen asignada para este nivel.");
        }

        panelReporte.gameObject.SetActive(false);
        // panelPerdiste.gameObject.SetActive(false);
        botonSiguienteNivel.gameObject.SetActive(false);

        panelInicioDia.gameObject.SetActive(true);
        int diaActual = gameManager.NivelActual;
        string titulo = $"Día {diaActual}\n\n";

        panelInicioDiaCoroutine = StartCoroutine(MostrarPanelInicioDiaCoroutine(titulo + mensaje));

        PanelInicioDesactivado += MostrarPanelIndicaciones;
    }

    public void MostrarPanelIndicaciones()
    {

        if (gameManager.NivelActual == 1)
        {
            StartCoroutine(MostrarPanelIndicacionesCoroutine());
            PanelInicioDesactivado -= MostrarPanelIndicaciones;
        }
        else
        {
            IniciarJuego();
            interactableObjects.ActivarEventTriggers();
        }
    }

  public IEnumerator MostrarPanelInicioDiaCoroutine(string mensaje)
{
    textoInicioDia.text = "";
    audioTecleo.Play();
    tiempoUltimaActualizacion = Time.time;

    // Mostrar el texto letra por letra
    for (int i = 0; i < mensaje.Length; i++)
    {
        textoInicioDia.text += mensaje[i]; // Añadir letra
        textoInicioDia.text += "_"; // Añadir el cursor

        yield return new WaitForSeconds(velocidadTexto);
        textoInicioDia.text = textoInicioDia.text.TrimEnd('_'); // Quitar el cursor en la siguiente iteración
    }

    // Finalizar texto
    textoInicioDia.text = mensaje;

    audioTecleo.Stop();

    // Mostrar el texto completo con el cursor titilante
    while (true)
    {
        if (Time.time - tiempoUltimaActualizacion >= intervaloCursor)
        {
            cursorVisible = !cursorVisible;
            tiempoUltimaActualizacion = Time.time;
        }

        string textoConCursorTitilante = mensaje;
        if (cursorVisible)
        {
            textoConCursorTitilante += "_";
        }
        textoInicioDia.text = textoConCursorTitilante;

        yield return null; // Esperar al siguiente frame

        // Salir del bucle cuando se haga clic
        if (Input.GetMouseButtonDown(0))
        {
            break;
        }
    }

    // Esperar antes de desactivar el panel
    yield return new WaitForSeconds(duracionPanel);
    panelInicioDia.gameObject.SetActive(false);

    PanelInicioDesactivado?.Invoke();
}



    public void CerrarPanelInicioDia()
    {
        if (panelInicioDiaCoroutine != null)
        {
            StopCoroutine(panelInicioDiaCoroutine);
            panelInicioDiaCoroutine = null;
        }

        audioTecleo.Stop();
        panelInicioDia.gameObject.SetActive(false);

        optionsManager.ActivarBotonesVentanas();

        //llamar iniico juego
        PanelInicioDesactivado?.Invoke();
    }


    private IEnumerator MostrarPanelIndicacionesCoroutine()
    {
        yield return StartCoroutine(MostrarIndicacionesSecuencia());
        PanelInicioDesactivado -= MostrarPanelIndicaciones;
        yield break;
    }


    private IEnumerator MostrarIndicacionesSecuencia()
    {

        optionsManager.panelAyuda.gameObject.SetActive(true);

        /*  RectTransform[] indicaciones = { indicaciones1, indicaciones2, indicaciones3, indicaciones4, indicaciones5 };

          foreach (var indicacion in indicaciones)
          {
              indicacion.gameObject.SetActive(true);
              float tiempoRestante = duracionIndicaciones;

              omitirIndicaciones.gameObject.SetActive(true);

              while (tiempoRestante > 0)
              {
                  if (Input.GetKeyDown(KeyCode.Space))
                  {
                      break; // Saltar a la siguiente indicación
                  }
                  tiempoRestante -= Time.deltaTime;
                  yield return null;
              }

              indicacion.gameObject.SetActive(false);
              omitirIndicaciones.gameObject.SetActive(false);
          }*/

         yield return new WaitUntil(() => !optionsManager.panelAyuda.gameObject.activeSelf);


        // Iniciar el juego después de mostrar todas las indicaciones
        IniciarJuego();
        interactableObjects.ActivarEventTriggers();

        yield break;
    }



    public void ActualizarPanelReporte(int sanosIngresados, int enfermosIngresados, int sanosRechazados, int enfermosRechazados)
    {
        panelReporte.gameObject.SetActive(true);
        // botonSiguienteNivel.gameObject.SetActive(true);

        optionsManager.DesactivarBotonesVentanas();
        interactableObjects.DesactivarEventTriggers();

        int diaActual = gameManager.NivelActual;
        string tituloReporte = $"Reporte Día {diaActual}\n";
        reporteText.text = $"{tituloReporte}" +
                        $"Sanos ingresados: {sanosIngresados}\n" +
                        $"Enfermos ingresados: {enfermosIngresados}\n" +
                        $"Sanos rechazados: {sanosRechazados}\n" +
                        $"Enfermos rechazados: {enfermosRechazados}";
        gameManager.MostrarMensaje();
    }

    public void PanelReporte()
    {
        //panelPerdiste.gameObject.SetActive(true);
    }


    public void IniciarJuego()
    {
        charactersManager.AparecerSiguientePersonaje();
    }
}