using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum CharacterState
{
    Sano,
    Enfermo
}

[System.Serializable]
public class Character
{
    public string nombre;
    public CharacterState estado;
    public List<string> dialogos;
    public List<string> respuestas;

    public string[] dialogosIngreso;  // Diálogo cuando el personaje es aceptado
    public string[] dialogosRechazo;

    public GameObject prefab;
    public int nivel;
    public bool esAgresivo;

    [HideInInspector] public Animator animator;
}

public class CharactersManager : MonoBehaviour
{

    public GameObject[] personajesPrefabs;
    public Transform spawnPoint;
    public Transform centerPoint;
    public Transform exitPoint;
    public List<Character> characters;
    public DialogueManager dialogueManager;
    public s_GameManager gameManager;
    public UI_Manager uiManager;
    public CheckCondition checkCondition;
    public LeverController leverController;
    public DarkeningLayer darkeningLayer;

    private List<GameObject> personajesEnPantalla = new List<GameObject>();
    private List<Character> charactersForCurrentLevel = new List<Character>();
    private int personajesPorNivel = 13;
    private int index = 0;
    public float tiempoDeEspera = 4.0f;
    public float moveDuration = 4.0f;
    public float bounceHeight = 0.2f;
    public float bounceSpeed = 2.0f;

    public AudioSource sonidoPasos;

    public TextMeshProUGUI contadorPersonas;

    private bool juegoTerminado = false;



    void Start()
    {

        ConfigurarPersonajesParaNivel(gameManager.NivelActual);
    }

    public void DetenerPersonajes()
    {
        juegoTerminado = true;  // Marcar que el juego ha terminado
        StopAllCoroutines();    // Detener todos los coroutines activos en este script
        Debug.Log("Todos los coroutines han sido detenidos, no se aparecerán más personajes.");
    }

    public void ConfigurarPersonajesParaNivel(int nivel)
    {
        charactersForCurrentLevel.Clear();

        // Filtra los personajes para el nivel actual
        foreach (var character in characters)
        {
            if (character.nivel == nivel)
            {
                charactersForCurrentLevel.Add(character);
            }
        }

        Debug.Log($"Número de personajes para el nivel {nivel}: {charactersForCurrentLevel.Count}");

        // Asegúrate de que la cantidad de personajes por nivel no exceda el número total de personajes configurados
        if (charactersForCurrentLevel.Count > personajesPorNivel)
        {
            charactersForCurrentLevel = charactersForCurrentLevel.GetRange(0, personajesPorNivel);
        }

        contadorPersonas.text = charactersForCurrentLevel.Count.ToString();

        // Mezcla la lista de personajes
        Shuffle(charactersForCurrentLevel);
    }

    public void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    public Character GetCurrentCharacter()
    {
        if (charactersForCurrentLevel.Count > 0)
        {
            return charactersForCurrentLevel[index - 1]; // Retorna el personaje actual basado en el índice
        }
        Debug.LogError("No hay personajes disponibles.");
        return null;
    }


    public void AparecerSiguientePersonaje()
    {


        if (juegoTerminado)
        {
            Debug.Log("El juego ha terminado. No se aparecerán más personajes.");
            return; // Detener si el juego ha terminado
        }


        StartCoroutine(AparecerConRetraso());

        darkeningLayer.OnCharacterSpawned();
    }

    private IEnumerator AparecerConRetraso()
    {
        // LimpiarPersonajes();

        // dialogueManager.botonIngreso.interactable = false;
        // dialogueManager.botonRechazo.interactable = false;

        leverController.DesactivarPalanca();


        checkCondition.botonMedico.interactable = false;

        Debug.Log("Esperando antes de aparecer el siguiente personaje...");
        yield return new WaitForSeconds(tiempoDeEspera);



        // Verificar si el juego ha terminado antes de continuar
        if (juegoTerminado)
        {
            Debug.Log("El juego ha terminado durante el retraso. No se aparecerán más personajes.");
            yield break;  // Detener el Coroutine si el juego ha terminado
        }




        Debug.Log("Tiempo de espera completado. Apareciendo el siguiente personaje.");

        if (index < charactersForCurrentLevel.Count)
        {
            Character character = charactersForCurrentLevel[index];
            Debug.Log($"Instanciando personaje: {character.nombre} (Índice: {index})");

            //GameObject personajePrefab = personajesPrefabs[Random.Range(0, personajesPrefabs.Length)];
            GameObject nuevoPersonaje = Instantiate(character.prefab, spawnPoint.position, Quaternion.identity);


            character.animator = nuevoPersonaje.GetComponent<Animator>();  // Obtener el Animator del prefab


            personajesEnPantalla.Add(nuevoPersonaje);



            StartCoroutine(MoverPersonajeAlCentro(nuevoPersonaje, centerPoint.position, index));
            index++;


            /*  if (index % personajesPorDecremento == 0)
              {
                  StartCoroutine(OscurecerLuzGradualmente());
              }*/

            contadorPersonas.text = (charactersForCurrentLevel.Count - index).ToString();

        }
        else
        {
            Debug.Log("Todos los personajes han sido instanciados.");
            gameManager.MostrarPanelReporte();
        }
    }

    private IEnumerator MoverPersonajeAlCentro(GameObject personaje, Vector3 destino, int characterIndex)
    {
        if (personaje == null)
        {
            yield break;
        }

        Vector3 inicio = personaje.transform.position;
        float tiempoTranscurrido = 0f;

        sonidoPasos.Play();

        while (tiempoTranscurrido < moveDuration)
        {
            if (personaje == null)
            {
                yield break;
            }

            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / moveDuration);

            // Movimiento de deslizamiento
            personaje.transform.position = Vector3.Lerp(inicio, destino, t);

            // Movimiento caminar
            float offsetY = Mathf.Sin(tiempoTranscurrido * bounceSpeed) * bounceHeight;
            personaje.transform.position = new Vector3(personaje.transform.position.x, destino.y + offsetY, personaje.transform.position.z);

            yield return null;
        }

        if (personaje == null)
        {
            yield break;
        }

        sonidoPasos.Stop();

        // Asegurarse de que el personaje esté exactamente en el destino final
        personaje.transform.position = destino;

        // Mostrar el diálogo del personaje
        MostrarDialogoPersonaje(characterIndex);
    }

    public void LimpiarPersonajes()
    {
        StartCoroutine(MoverPersonajesFueraDePantalla());
    }

    private IEnumerator MoverPersonajesFueraDePantalla()
    {
        List<Coroutine> salidas = new List<Coroutine>();

        foreach (GameObject personaje in personajesEnPantalla)
        {
            salidas.Add(StartCoroutine(MoverPersonajeFueraDePantalla(personaje, exitPoint.position)));
        }

        // Esperar a que todos los personajes se hayan movido fuera de la pantalla
        foreach (Coroutine salida in salidas)
        {
            yield return salida;
        }

        // Limpiar personajes después de moverlos fuera de la pantalla
        foreach (GameObject personaje in personajesEnPantalla)
        {
            Destroy(personaje);
        }
        personajesEnPantalla.Clear();
    }

    private IEnumerator MoverPersonajeFueraDePantalla(GameObject personaje, Vector3 destino)
    {
        if (personaje == null)
        {
            yield break;
        }

        Vector3 inicio = personaje.transform.position;
        float tiempoTranscurrido = 0f;

        sonidoPasos.Play();

        while (tiempoTranscurrido < moveDuration)
        {
            if (personaje == null)
            {
                yield break;
            }

            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / moveDuration);

            personaje.transform.position = Vector3.Lerp(inicio, destino, t);

            float offsetY = Mathf.Sin(tiempoTranscurrido * bounceSpeed) * bounceHeight;
            personaje.transform.position = new Vector3(personaje.transform.position.x, destino.y + offsetY, personaje.transform.position.z);

            yield return null;
        }

        if (personaje == null)
        {
            yield break;
        }

        sonidoPasos.Stop();

        personaje.transform.position = destino;
    }

    public void MostrarDialogoPersonaje(int characterIndex)
    {
        if (characterIndex < charactersForCurrentLevel.Count)
        {
            Character character = charactersForCurrentLevel[characterIndex];
            string[] dialogos = character.dialogos.ToArray();
            dialogueManager.ComenzarDialogo(dialogos, character.respuestas, character.esAgresivo, false);

            //   dialogueManager.IniciarDialogo();
        }
        else
        {
            Debug.Log("Índice de personaje fuera de rango");
        }
    }

    public void MostrarRespuestas(List<string> respuestas)
    {
        dialogueManager.MostrarRespuestas(respuestas);
    }

    public Character GetCharacter(int index)
    {
        if (index >= 0 && index < charactersForCurrentLevel.Count)
        {
            return charactersForCurrentLevel[index];
        }
        Debug.LogError("Índice de personaje fuera de rango");
        return null;
    }

    public int CurrentCharacterIndex
    {
        get { return index - 1; }
    }


    public void MoverPersonajeAlPunto(Vector3 destino)
    {
        StartCoroutine(MoverPersonajesAlPunto(destino));
    }

    private IEnumerator MoverPersonajesAlPunto(Vector3 destino)
    {
        List<Coroutine> salidas = new List<Coroutine>();

        foreach (GameObject personaje in personajesEnPantalla)
        {
            salidas.Add(StartCoroutine(MoverPersonajeFueraDePantalla(personaje, destino)));
        }

        // Esperar a que todos los personajes se hayan movido al destino
        foreach (Coroutine salida in salidas)
        {
            yield return salida;
        }

        // Limpiar personajes después de moverlos al destino
        foreach (GameObject personaje in personajesEnPantalla)
        {
            Destroy(personaje);
        }
        personajesEnPantalla.Clear();
    }

    public GameObject GetCharacterGameObject()
    {
        if (personajesEnPantalla.Count > 0)
        {
            return personajesEnPantalla[personajesEnPantalla.Count - 1];
        }
        return null;
    }

    public void ActivarAnimacion(Character character, string trigger)
    {
        if (character.animator != null)
        {
            character.animator.SetTrigger(trigger);
        }
        else
        {
            Debug.LogWarning($"Animator no encontrado para el personaje {character.nombre}");
        }
    }


    public void Hablando()
    {
        Character character = GetCurrentCharacter();
        if (character != null)
        {
            ActivarAnimacion(character, "triggerTalk");
        }
        else
        {
            Debug.LogWarning("No hay personaje actual para hablar.");
        }
    }



    public void TerminoHablar()
    {
        Character character = GetCurrentCharacter();



        if (character != null)
        {
            ActivarAnimacion(character, "triggerBlink");
        }
        else
        {
            Debug.LogWarning("No hay personaje actual para hablar.");
        }
    }



 public void Reaccion()
    {
        Character character = GetCurrentCharacter();



        if (character != null)
        {
            ActivarAnimacion(character, "triggerReaction");
        }
        else
        {
            Debug.LogWarning("No hay personaje actual para hablar.");
        }
    }



public void ActivarReaccionIngreso()
{

     Character character = GetCurrentCharacter();

  if (character != null)
        {
            ActivarAnimacion(character, "reaccionIngreso");
        }
   
    
}

public void ActivarReaccionRechazo()
{

       Character character = GetCurrentCharacter();

  if (character != null)
        {
           ActivarAnimacion(character, "reaccionRechazo");
        }
   
}

public int GetCurrentIndex()
{
    return index; // Devuelve el índice del personaje que actualmente está siendo evaluado
}

}