using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Analytics; 
using static EventManager;

public class CheckCondition : MonoBehaviour
{
    public GameObject medicoPrefab;
    public GameObject cartelSanoPrefab;
    public GameObject cartelEnfermoPrefab;
    public GameObject luzEscanerPrefab;
    public Transform spawnPointMedico;
    public Transform centroPantalla;
    public Transform puntoSalidaMedico;
    public Transform spawnPointIngreso;
    public Transform spawnPointRechazo;
    public Transform spawnPointLuzEscaner;
    public CharactersManager charactersManager;
    public DialogueManager dialogueManager;
    public s_GameManager gameManager;
    public LeverController leverController;

    private GameObject medicoInstance;

    public Button botonMedico;
    public AudioSource sonidoBoton;
    public AudioSource brazoMecanico;
    public AudioSource luzEscaner;
    public AudioSource beepEscaner;


    public void Start()
    {
        botonMedico.interactable = false;
    }

    public void EvaluarSalud()
    {
        //  MedicalScanUsed
        RegisterScanUsedEvent();

        sonidoBoton.Play();
        if (medicoInstance == null)
        {
            medicoInstance = Instantiate(medicoPrefab, spawnPointMedico.position, Quaternion.identity);
            StartCoroutine(MedicoEvaluacionRoutine());

            botonMedico.interactable = false;
            dialogueManager.medicoUsado = true;
        }
    }

    private void RegisterScanUsedEvent()
    {
        // Debug para verificar
       
        Debug.Log($"ScanUsed - Nivel: {GameData.NivelActual}, Índice personaje: {charactersManager.GetCurrentIndex()}");
        
        // Crear y configurar el evento
        ScanUsedEvent scanUsed = new ScanUsedEvent();
        scanUsed.level = GameData.NivelActual;
        scanUsed.charIndex = charactersManager.GetCurrentIndex();
        
        // Grabar el evento 
        #if !UNITY_EDITOR
            AnalyticsService.Instance.RecordEvent(scanUsed);
        #else
            Debug.Log("[ANALYTICS] Evento ScanUsedEvent registrado");
        #endif
   
    }


    private IEnumerator MedicoEvaluacionRoutine()
    {
        leverController.DesactivarPalanca();

        brazoMecanico.Play();
        yield return StartCoroutine(MoverPersonaje(medicoInstance.transform, centroPantalla.position));
        brazoMecanico.Stop();

        yield return new WaitForSeconds(2f);

        EvaluarEstadoPersonaje();

        yield return new WaitForSeconds(7f);

        brazoMecanico.Play();
        yield return StartCoroutine(MoverPersonaje(medicoInstance.transform, puntoSalidaMedico.position));
        brazoMecanico.Stop();
        Destroy(medicoInstance);

        leverController.ActivarPalanca();
    }

    private IEnumerator MoverPersonaje(Transform personaje, Vector3 destino)
    {
        float velocidadMovimiento = 3f;

        while (Vector3.Distance(personaje.position, destino) > 0.1f)
        {
            personaje.position = Vector3.MoveTowards(personaje.position, destino, velocidadMovimiento * Time.deltaTime);
            yield return null;
        }
    }

    private void EvaluarEstadoPersonaje()
    {
        Character personajeActual = charactersManager.GetCharacter(charactersManager.CurrentCharacterIndex);

        if (personajeActual != null)
        {
            StartCoroutine(MostrarLuzEscanerYCartel(personajeActual));
        }
    }

    private IEnumerator MostrarLuzEscanerYCartel(Character personajeActual)
    {
   
        GameObject luzEscanerInstance = Instantiate(luzEscanerPrefab, spawnPointLuzEscaner.position, Quaternion.identity);
        luzEscaner.Play();

        yield return new WaitForSeconds(3f);
        luzEscaner.Stop();

        Destroy(luzEscanerInstance);
        yield return new WaitForSeconds(2f);
       
        if (personajeActual.estado == CharacterState.Sano)
        {
            Debug.Log("El personaje está sano.");
            StartCoroutine(MostrarCartel(cartelSanoPrefab, spawnPointIngreso));
        }
        else if (personajeActual.estado == CharacterState.Enfermo)
        {
            Debug.Log("El personaje está enfermo.");
            StartCoroutine(MostrarCartel(cartelEnfermoPrefab, spawnPointRechazo));
        }
    }

    private IEnumerator MostrarCartel(GameObject cartelPrefab, Transform spawnPoint)
    {
        GameObject cartelInstance = Instantiate(cartelPrefab, spawnPoint.position, Quaternion.identity);
        beepEscaner.Play();
        yield return new WaitForSeconds(2f);
        Destroy(cartelInstance);
    }

    public void ReiniciarNivel()
    {
       
        dialogueManager.medicoUsado = false;
    }
}

