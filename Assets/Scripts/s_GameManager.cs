using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class s_GameManager : MonoBehaviour
{
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


    void Start()
    {
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


        yield return new WaitForSeconds(3f); // Esperar 2 segundos

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

        yield return new WaitForSeconds(3f); // Esperar 2 segundos

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

        // Levantar la capaPuerta hacia arriba
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoMovimiento)
        {
            capaPuerta.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tiempoTranscurrido / tiempoMovimiento);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        capaPuerta.transform.position = posicionFinal;

        // Esperar el tiempo extendido o el tiempo por defecto
        float tiempoEsperaActual = tiempoEsperaExtendido ?? tiempoEspera;
        yield return new WaitForSeconds(tiempoEsperaActual);

        // Devolver la capa a su posición original
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
                }
            }
            else
            {
                if (personajeActual.estado == CharacterState.Sano)
                {
                    Debug.Log("¡Elección incorrecta! Personaje sano rechazado.");
                    sanosRechazados++;
                    strikes++;
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

        if (GameData.Faltas <= 0)
        {
            uiManager.ActualizarPanelReporte(sanosIngresados, enfermosIngresados, sanosRechazados, enfermosRechazados);
        }
        else
        {
            uiManager.PanelReporte();
        }
    }

    public void MostrarMensaje()
    {
        // Mostrar mensaje según la cantidad de enfermos ingresados
        if (enfermosIngresados == 0 && sanosRechazados == 0)
        {
            uiManager.mensajeReporte.text = "¡Buen trabajo!";

            //modificacion temporal para que no pase al nivel 3
            if (NivelActual == 1)
            {
                uiManager.botonSiguienteNivel.gameObject.SetActive(true);
            };

            if (NivelActual == 2)
            {

                uiManager.botonGanaste.gameObject.SetActive(true);
            }

        }
        else 
        {
            uiManager.mensajeReporte.text = "Más cuidado la próxima vez...";
            // GameData.Faltas++;


            //modificacion temporal para que no pase al nivel 3
            if (NivelActual == 1)
            {
                uiManager.botonSiguienteNivel.gameObject.SetActive(true);
            };

            if (NivelActual == 2)
            {

                uiManager.botonGanaste.gameObject.SetActive(true);
            }
        }
    
    }


    public void OnBotonSiguienteNivel()
    {
        Debug.Log("Botón Siguiente Nivel presionado");
        NivelActual++;
        GameData.NivelActual = NivelActual; // Guardar el nivel actual en la clase GameData
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
