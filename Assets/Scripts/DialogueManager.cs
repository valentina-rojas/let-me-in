using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textoDialogo;
    public RectTransform panelDialogo;

    public RectTransform panelRespuestas;
    public TextMeshProUGUI textoRespuesta;

    public Button botonIngreso;
    public Button botonRechazo;
    public RectTransform panelSiguiente;

    public float velocidadTexto = 0.05f;

    private string[] lineas;
    private List<string> respuestasActuales;

    private bool mostrandoRespuestas = false;
    private bool textoCompleto = false;
    private bool esAgresivo;

    private int indexDialogo;
    private int indexRespuestas;

    public s_GameManager gameManager;
    public AudioManager audioManager;

    public AggressiveNPCs aggressiveNPCs;
    public CheckCondition checkCondition;

    public bool medicoUsado = false;

    public float intervaloCursor = 0.5f;
    private bool cursorVisible = true;
    private float tiempoUltimaActualizacion;

    public AudioSource vozGuardia;
    public AudioSource vozPersonaje;

    private bool dialogoVisible = false;  // Verifica si hay un diálogo en pantalla

    public RectTransform areaClickable; // Asigna este panel como área clickable

    public Button nextDialogo;
    public Button nextRespuesta;


    void Start()
    {

    }

    void Update()
    {
        if (dialogoVisible)
        {
            // Detecta si el usuario presiona Espacio para adelantar un diálogo
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AdelantarDialogo();
            }
            // Detecta si el usuario presiona Enter para omitir todos los diálogos
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                SaltarTodosLosDialogos();
            }
        }
    }

    public void AdelantarDialogo()
    {
        textoCompleto = true;

        PausarVoces();
    }

    public void SaltarTodosLosDialogos()
    {
        StopAllCoroutines();
        panelDialogo.gameObject.SetActive(false);
        panelRespuestas.gameObject.SetActive(false);
        dialogoVisible = false;

        PausarVoces();

        if (esAgresivo)
        {
            aggressiveNPCs.MostrarComportamientoAgresivo();
        }
        else
        {
            MostrarBotonSiguiente();
        }

    }

    public void PausarVoces()
    {
        if (vozGuardia != null)
        {
            vozGuardia.Pause();
        }

        if (vozPersonaje != null)
        {
            vozPersonaje.Pause();
        }
    }


    public void ComenzarDialogo(string[] dialogos, List<string> respuestas, bool esAgresivo)
    {
        lineas = dialogos;
        respuestasActuales = respuestas;
        this.esAgresivo = esAgresivo;

        indexDialogo = 0;
        indexRespuestas = 0;

        MostrarPanelDialogo();
    }

    void MostrarPanelRespuestas()
    {
        panelRespuestas.gameObject.SetActive(true);
        panelDialogo.gameObject.SetActive(false);
        dialogoVisible = true;
        StartCoroutine(EscribirRespuestas());
    }

    IEnumerator EscribirRespuestas()
    {
        textoRespuesta.text = "";

        // Iniciar la reproducción del audio pero no detener el flujo del texto
        vozGuardia.Play();
        StartCoroutine(DetenerAudioGuardia(2));


        tiempoUltimaActualizacion = Time.time;

        while (textoRespuesta.text.Length < respuestasActuales[indexRespuestas].Length)
        {
            if (textoCompleto) break;

            if (Time.time - tiempoUltimaActualizacion >= intervaloCursor)
            {
                cursorVisible = !cursorVisible;
                tiempoUltimaActualizacion = Time.time;
            }

            // Construye el texto actual con el cursor
            string textoParcial = respuestasActuales[indexRespuestas].Substring(0, textoRespuesta.text.Length);
            if (cursorVisible)
            {
                textoParcial += "_";
            }
            textoRespuesta.text = textoParcial;

            yield return new WaitForSeconds(velocidadTexto);
        }


        textoRespuesta.text = respuestasActuales[indexRespuestas];
        if (AudioManager.instance != null)
        {
            AudioManager.instance.DetenerHablar();
        }

        while (true)
        {
            if (Time.time - tiempoUltimaActualizacion >= intervaloCursor)
            {
                cursorVisible = !cursorVisible;
                tiempoUltimaActualizacion = Time.time;
            }

            // Construir el texto actual con el cursor titilante
            string textoConCursorTitilante = respuestasActuales[indexRespuestas];
            if (cursorVisible)
            {
                textoConCursorTitilante += "_";
            }
            textoRespuesta.text = textoConCursorTitilante;

            yield return null; // Espera hasta el siguiente frame

            // Salir del bucle cuando se haga clic
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }

            textoCompleto = false;

            nextRespuesta.gameObject.SetActive(true);
        }

    }

    // Función adicional para detener el audio después de 2 segundos
    IEnumerator DetenerAudioGuardia(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        vozGuardia.Pause();
    }


    void MostrarPanelDialogo()
    {
        mostrandoRespuestas = false;
        dialogoVisible = true;
        panelDialogo.gameObject.SetActive(true);
        panelRespuestas.gameObject.SetActive(false);
        ComenzarEscritura();
    }

    void ComenzarEscritura()
    {
        StartCoroutine(EscribirLinea());
    }

    IEnumerator EscribirLinea()
    {
        textoDialogo.text = string.Empty;

        vozPersonaje.Play();
        StartCoroutine(DetenerAudioPersonaje(2));

        tiempoUltimaActualizacion = Time.time; // Inicializar el tiempo del cursor

        while (textoDialogo.text.Length < lineas[indexDialogo].Length)
        {
            if (textoCompleto)
            {
                textoDialogo.text = lineas[indexDialogo]; // Mostrar el texto completo
                break;
            }

            if (Time.time - tiempoUltimaActualizacion >= intervaloCursor)
            {
                cursorVisible = !cursorVisible;
                tiempoUltimaActualizacion = Time.time;
            }

            string textoParcial = lineas[indexDialogo].Substring(0, textoDialogo.text.Length);
            if (cursorVisible)
            {
                textoParcial += "_";
            }
            textoDialogo.text = textoParcial;

            yield return new WaitForSeconds(velocidadTexto);
        }

        // Finalizar diálogo
        textoDialogo.text = lineas[indexDialogo];

        if (AudioManager.instance != null)
        {
            AudioManager.instance.DetenerHablar();
        }


        // Mostrar el texto completo con el cursor titilante
        while (true)
        {
            if (Time.time - tiempoUltimaActualizacion >= intervaloCursor)
            {
                cursorVisible = !cursorVisible;
                tiempoUltimaActualizacion = Time.time;
            }

            string textoConCursorTitilante = lineas[indexDialogo];
            if (cursorVisible)
            {
                textoConCursorTitilante += "_";
            }
            textoDialogo.text = textoConCursorTitilante;

            yield return null; // Esperar al siguiente frame

            // Salir del bucle cuando se haga clic
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }

            textoCompleto = false;
            mostrandoRespuestas = true;
            nextDialogo.gameObject.SetActive(true);
        }

    }

    // Función adicional para detener el audio después de 2 segundos
    IEnumerator DetenerAudioPersonaje(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        vozPersonaje.Pause();
    }

    public void MostrarRespuestas(List<string> respuestas)
    {
        respuestasActuales = respuestas;
        MostrarPanelRespuestas();
    }

    public void PanelRespuestasClick()
    {
        nextRespuesta.gameObject.SetActive(false);
        if (mostrandoRespuestas)
        {
            mostrandoRespuestas = false;
            indexRespuestas++;
            MostrarPanelDialogo();
        }
    }

    public void PanelDialogoClick()
    {

        nextDialogo.gameObject.SetActive(false);

        if (mostrandoRespuestas && indexRespuestas < respuestasActuales.Count)
        {
            indexDialogo++;
            MostrarPanelRespuestas();
        }
        else
        {
            if (indexDialogo == lineas.Length - 1)
            {
                panelDialogo.gameObject.SetActive(false);
                dialogoVisible = false;
                //MostrarBotonSiguiente();
                if (esAgresivo)
                {
                    aggressiveNPCs.MostrarComportamientoAgresivo();
                }
                else
                {
                    MostrarBotonSiguiente();
                }
            }
        }
    }

    void MostrarBotonSiguiente()
    {
        botonIngreso.interactable = true;
        botonRechazo.interactable = true;
        ColisionBotones();
        panelSiguiente.gameObject.SetActive(true);

        if (!medicoUsado)
        {
            checkCondition.botonMedico.interactable = true;
        }

    }

    public void ColisionBotones()
    {
        var imageIngreso = botonIngreso.GetComponent<Image>();
        var imageRechazo = botonRechazo.GetComponent<Image>();


        if (imageIngreso != null)
        {
            imageIngreso.alphaHitTestMinimumThreshold = 0.1f;
        }

        if (imageRechazo != null)
        {
            imageRechazo.alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}

