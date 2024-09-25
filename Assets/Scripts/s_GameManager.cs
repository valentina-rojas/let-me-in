using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class s_GameManager : MonoBehaviour
{
    public UI_Manager uiManager;
    public CharactersManager charactersManager;
    public DialogueManager dialogueManager;
    public RadioManager radioManager;

    public string[] mensajesInicioDia;

    public int sanosIngresados;
    public int enfermosIngresados;
    public int sanosRechazados;
    public int enfermosRechazados;

    public GameObject Musica;
    public AudioSource backgroundMusic;
    public AudioSource sonidoBoton;
    public AudioSource puertaAbriendose;
    public AudioSource ruidoAmbiente;

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
    }

    public void OnBotonIngresoClick()
    {
        //sonidoBoton.Play();

        StartCoroutine(DetenerSonidoPuerta(4f)); // Detener sonido después de 2 segundos
        VerificarEstadoPersonaje(true);
        charactersManager.MoverPersonajeAlPunto(charactersManager.exitPoint.position);

        StartCoroutine(AbrirPuerta());
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

    public void OnBotonRechazoClick()
    {
       // sonidoBoton.Play();
        VerificarEstadoPersonaje(false);
        charactersManager.MoverPersonajeAlPunto(charactersManager.spawnPoint.position);
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
                     NextCharacter();
                }
                else if (personajeActual.estado == CharacterState.Enfermo)
                {
                    Debug.Log("¡Elección incorrecta! Personaje enfermo ingresado.");
                    enfermosIngresados++;

                   StartCoroutine(ProximoPersonajeTrasDisturbios());
                }
            }
            else
            {
                if (personajeActual.estado == CharacterState.Sano)
                {
                    Debug.Log("¡Elección incorrecta! Personaje sano rechazado.");
                    sanosRechazados++;

                     NextCharacter();
                }
                else if (personajeActual.estado == CharacterState.Enfermo)
                {
                    Debug.Log("¡Elección correcta! Personaje enfermo rechazado.");
                    enfermosRechazados++;
                     NextCharacter();
                }
            }
        }
    }

    private IEnumerator ProximoPersonajeTrasDisturbios()
{
    // Esperar a que termine el disturbio antes de avanzar al siguiente personaje
    yield return StartCoroutine(radioManager.ActivarDisturbiosCoroutine());

    // Llamar a NextCharacter después de que haya terminado el disturbio
    NextCharacter();
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

            //modificacion temporal para que no apse al nivel 3
            if (NivelActual == 1)
            {
                uiManager.botonSiguienteNivel.gameObject.SetActive(true);
            };

        }
        else if ((enfermosIngresados >= 1 && enfermosIngresados <= 3) || (sanosRechazados >= 1 && sanosRechazados <= 3))
        {
            uiManager.mensajeReporte.text = "Más cuidado la próxima vez...";
            // GameData.Faltas++;


            //modificacion temporal para que no apse al nivel 3
            if (NivelActual == 1)
            {
                uiManager.botonSiguienteNivel.gameObject.SetActive(true);
            };
        }
        else if (enfermosIngresados > 3 || sanosRechazados > 3)
        {
            uiManager.mensajeReporte.text = "Fuiste retirado del puesto de trabajo.";
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


}
