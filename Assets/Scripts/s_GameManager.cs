using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Analytics; 
using static EventManager;

public class s_GameManager : MonoBehaviour
{
    public static s_GameManager Instance { get; private set; }


    public UI_Manager uiManager;
    public CharactersManager charactersManager;
    public DialogueManager dialogueManager;
    public RadioManager radioManager;
    public CheckCondition checkCondition;
    public StressBar stressBar;
    public RechazoBarraManager rechazoBarraManager;

    public string[] mensajesInicioDia;

    public int sanosIngresados;
    public int enfermosIngresados;
    public int sanosRechazados;
    public int enfermosRechazados;
    public int strikes;

    public TextMeshProUGUI textoStrikes;

    public GameObject Musica;
    public AudioSource backgroundMusic;
    public AudioSource sonidoBoton;
    public AudioSource puertaAbriendose;
    public AudioSource ruidoAmbiente;
    public AudioSource ruidoPalanca;

    public GameObject capaPuerta;
    public float alturaMovimiento = 7f;
    public float tiempoMovimiento = 1f;
    public float tiempoEspera = 3f;

    private int totalEnfermos;
    public int NivelActual { get; private set; }

    private float tiempoNivel;

    private float tiempoTotalJuego = 0f;
    private int strikesAcumulados = 0;
    public int dialogosOmitidosTotal = 0;

  void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


   void Start()
    {

        tiempoNivel = 0f;
        strikes = GameData.Faltas;  
        dialogosOmitidosTotal = GameData.DialogosOmitidos;
        tiempoTotalJuego = GameData.TiempoTotal;



        // Llamar al evento LevelStart
        RegisterLevelStartEvent();
        
        NivelActual = GameData.NivelActual;
        ruidoAmbiente.Play();

        if (uiManager != null && charactersManager != null)
        {
            string mensajeInicio = ObtenerMensajeInicioParaNivel(NivelActual);
            uiManager.MostrarInicioDia(mensajeInicio);
        }
        else
        {
            Debug.LogError("UI_Manager o CharactersManager no están asignados en GameController.");
        }
    }
     
     void Update()
    {
        tiempoNivel += Time.deltaTime;
        tiempoTotalJuego += Time.deltaTime;
    }


    private void RegisterLevelStartEvent()
    {
        // Debug para verificar
        Debug.Log($"LevelStart - Nivel: {GameData.NivelActual}");
        
        // Crear y configurar el evento
        LevelStartEvent levelStart = new LevelStartEvent();
        levelStart.level = GameData.NivelActual;
        
        // Grabar el evento 
        #if !UNITY_EDITOR
            AnalyticsService.Instance.RecordEvent(levelStart);
        #else
            Debug.Log("[ANALYTICS] Evento LevelStart registrado");
        #endif
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void NextCharacter()
    {
        charactersManager.AparecerSiguientePersonaje();

        stressBar.ActualizarEstres(1);
    }

    public void OnBotonIngresoClick()
    {
        StartCoroutine(ProcesoIngreso());
    }

    public void OnBotonRechazoClick()
    {
        StartCoroutine(ProcesoRechazo());
    }

    private IEnumerator ProcesoIngreso()
    {
        // sonidoBoton.Play();
        ruidoPalanca.Play();
        checkCondition.botonMedico.interactable = false;

        StartCoroutine(DetenerSonidoPuerta(4f)); // Detener sonido después de 4 segundos
        VerificarEstadoPersonaje(true);

        // Obtener el diálogo de ingreso del personaje actual y mostrarlo
        string[] dialogoIngreso = charactersManager.GetCharacter(charactersManager.CurrentCharacterIndex).dialogosIngreso;

        charactersManager.ActivarReaccionIngreso();

        dialogueManager.ComenzarDialogo(dialogoIngreso, null, false, true);


        yield return new WaitForSeconds(5f); // Esperar 2 segundos

        dialogueManager.OcultarDialogo();

        yield return new WaitUntil(() => dialogueManager.DialogoEstaOculto());

       // charactersManager.TerminoHablar();

        charactersManager.MoverPersonajeAlPunto(charactersManager.exitPoint.position);
        StartCoroutine(AbrirPuerta());

        // Verificar si el personaje ingresado es enfermo
        Character personajeActual = charactersManager.GetCharacter(charactersManager.CurrentCharacterIndex);


        if (personajeActual.estado == CharacterState.Enfermo)
        {
            // Si es enfermo, activar disturbios
            StartCoroutine(ProximoPersonajeTrasDisturbios());
            stressBar.ActualizarEstres(1);
            ActualizarTextoStrikes();
        }
        else
        {
            NextCharacter();
        }


        yield break;
    }

    private IEnumerator ProcesoRechazo()
    {
        // sonidoBoton.Play();
        ruidoPalanca.Play();
        checkCondition.botonMedico.interactable = false;

        VerificarEstadoPersonaje(false);


        // Obtener el diálogo de rechazo del personaje actual y mostrarlo
        string[] dialogoRechazo = charactersManager.GetCharacter(charactersManager.CurrentCharacterIndex).dialogosRechazo;

        charactersManager.ActivarReaccionRechazo();

        dialogueManager.ComenzarDialogo(dialogoRechazo, null, false, true);

        yield return new WaitForSeconds(5f); // Esperar 2 segundos

        dialogueManager.OcultarDialogo();

        yield return new WaitUntil(() => dialogueManager.DialogoEstaOculto());

        charactersManager.MoverPersonajeAlPunto(charactersManager.spawnPoint.position);


        Character personajeActual = charactersManager.GetCharacter(charactersManager.CurrentCharacterIndex);
        if (personajeActual.estado == CharacterState.Sano)
        {
            rechazoBarraManager.RechazarSano();
        }

        NextCharacter();

  

        yield break;
    }

    public IEnumerator AbrirPuerta(float? tiempoEsperaExtendido = null)
    {

        puertaAbriendose.Play();
        Vector3 posicionInicial = capaPuerta.transform.position;
        Vector3 posicionFinal = new Vector3(posicionInicial.x, posicionInicial.y + alturaMovimiento, posicionInicial.z);


        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoMovimiento)
        {
            capaPuerta.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tiempoTranscurrido / tiempoMovimiento);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        capaPuerta.transform.position = posicionFinal;

     
        float tiempoEsperaActual = tiempoEsperaExtendido ?? tiempoEspera;
        yield return new WaitForSeconds(tiempoEsperaActual);

        
        tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoMovimiento)
        {
            capaPuerta.transform.position = Vector3.Lerp(posicionFinal, posicionInicial, tiempoTranscurrido / tiempoMovimiento);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        capaPuerta.transform.position = posicionInicial;
    }

    private IEnumerator DetenerSonidoPuerta(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (puertaAbriendose.isPlaying)
        {
            puertaAbriendose.Stop();
        }
    }


    public void VerificarEstadoPersonaje(bool esIngreso)
    {
        Character personajeActual = charactersManager.GetCharacter(charactersManager.CurrentCharacterIndex);
        if (personajeActual != null)
        {
            if (esIngreso)
            {
                if (personajeActual.estado == CharacterState.Sano)
                {
                    Debug.Log("¡Elección correcta! Personaje sano ingresado.");
                    sanosIngresados++;

                }
                else if (personajeActual.estado == CharacterState.Enfermo)
                {
                    Debug.Log("¡Elección incorrecta! Personaje enfermo ingresado.");
                    enfermosIngresados++;
                    strikes++;
                    strikesAcumulados++;
                }
            }
            else
            {
                if (personajeActual.estado == CharacterState.Sano)
                {
                    Debug.Log("¡Elección incorrecta! Personaje sano rechazado.");
                    sanosRechazados++;
                    strikes++;
                    strikesAcumulados++;
                }
                else if (personajeActual.estado == CharacterState.Enfermo)
                {
                    Debug.Log("¡Elección correcta! Personaje enfermo rechazado.");
                    enfermosRechazados++;

                }
            }
        }
    }

    private IEnumerator ProximoPersonajeTrasDisturbios()
    {
        radioManager.ActivarDisturbios();
        yield break;
    }

    public void MostrarPanelReporte()
    {
        ruidoAmbiente.Stop();

  uiManager.ActualizarPanelReporte(sanosIngresados, enfermosIngresados, sanosRechazados, enfermosRechazados);

    }

    public void MostrarMensaje()
    {
        GameData.TiempoTotal = tiempoTotalJuego;
        RegisterLevelCompleteEvent();

        if (enfermosIngresados == 0 && sanosRechazados == 0)
        {
            uiManager.mensajeReporte.text = "¡Buen trabajo!";

            if (NivelActual == 1)
            {
                uiManager.botonSiguienteNivel.gameObject.SetActive(true);
            };

            if (NivelActual == 2)
            {
                uiManager.botonGanaste.gameObject.SetActive(true);
                RegisterGameFinishedEvent();
            }

        }
        else 
        {
            uiManager.mensajeReporte.text = "Más cuidado la próxima vez...";
            // GameData.Faltas++;


            if (NivelActual == 1)
            {
                uiManager.botonSiguienteNivel.gameObject.SetActive(true);
            };

            if (NivelActual == 2)
            {
                uiManager.botonGanaste.gameObject.SetActive(true);
                RegisterGameFinishedEvent();
            }
        }
    
      tiempoTotalJuego += tiempoNivel;
    }

  private void RegisterLevelCompleteEvent()
    {
        // Debug para verificar
        Debug.Log($"LevelComplete - Nivel: {GameData.NivelActual}, Tiempo: {Mathf.RoundToInt(tiempoNivel)}, Strikes: {strikes}, Diálogos Omitidos: {dialogueManager.dialogosOmitidos}");
        
        // Crear y configurar el evento
        LevelCompleteEvent  levelComplete = new  LevelCompleteEvent();
        levelComplete.level = GameData.NivelActual;
        levelComplete.time = Mathf.RoundToInt(tiempoNivel);
        levelComplete.strikes = strikes;
        levelComplete.dlgSkipped = dialogueManager.dialogosOmitidos;

        
        // Grabar el evento 
        #if !UNITY_EDITOR
            AnalyticsService.Instance.RecordEvent( levelComplete);
        #else
            Debug.Log("[ANALYTICS] Evento  LevelCompleteEvent registrado");
        #endif
    }

private void RegisterGameFinishedEvent()
    {

         // Debug para verificar todos los parámetros (usando directamente las fuentes originales)
        Debug.Log($"[DEBUG] GameFinishedEvent - Nivel: {GameData.NivelActual}, Tiempo Total: {Mathf.RoundToInt(tiempoTotalJuego)}s, Strikes: {strikes}, Diálogos Omitidos (Totales): {dialogosOmitidosTotal}");

        // Crear y configurar el evento
        GameFinishedEvent  gameFinished = new  GameFinishedEvent();
        gameFinished.time = Mathf.RoundToInt(tiempoTotalJuego);
        gameFinished.strikes = strikes;
        
        // Grabar el evento 
        #if !UNITY_EDITOR
            AnalyticsService.Instance.RecordEvent(gameFinished);
        #else
            Debug.Log("[ANALYTICS] Evento  GameFinishedEvent registrado");
        #endif
    }



    public void OnBotonSiguienteNivel()
    {
        Debug.Log("Botón Siguiente Nivel presionado");
        NivelActual++;
        GameData.NivelActual = NivelActual; // Guardar el nivel actual en la clase GameData
         GameData.Faltas = strikes;
        GameData.DialogosOmitidos = dialogosOmitidosTotal;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string ObtenerMensajeInicioParaNivel(int nivel)
    {
        // Asegurarse de que el índice esté dentro del rango del array
        if (nivel - 1 >= 0 && nivel - 1 < mensajesInicioDia.Length)
        {
            return mensajesInicioDia[nivel - 1];
        }
        else
        {
            return "Mensaje de inicio no definido para este nivel.";
        }
    }


    public void ActualizarTextoStrikes()
    {
        textoStrikes.text = "FALTAS: " + strikes;
    }


}
