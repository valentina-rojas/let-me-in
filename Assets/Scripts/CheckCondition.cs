using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckCondition : MonoBehaviour
{
    public GameObject medicoPrefab;
    public Transform spawnPointMedico;
    public Transform centroPantalla;
    public Transform puntoSalidaMedico;
    public CharactersManager charactersManager;
    public DialogueManager dialogueManager;
    public s_GameManager gameManager;
      public LeverController leverController;

    private GameObject medicoInstance;

    public Button botonMedico;
    public AudioSource sonidoBoton;

    public void Start()
    {
        botonMedico.interactable = false;
    }

    public void EvaluarSalud()
    {
     sonidoBoton.Play();
        if (medicoInstance == null)
        {
            medicoInstance = Instantiate(medicoPrefab, spawnPointMedico.position, Quaternion.identity);
            StartCoroutine(MedicoEvaluacionRoutine());

            botonMedico.interactable = false;
            dialogueManager.medicoUsado = true;
        }

    }

    private IEnumerator MedicoEvaluacionRoutine()
    {
        

        leverController.DesactivarPalanca();

        StartCoroutine(gameManager.AbrirPuerta(8f));

        yield return StartCoroutine(MoverPersonaje(medicoInstance.transform, centroPantalla.position));

        yield return new WaitForSeconds(2f);

        EvaluarEstadoPersonaje();

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(MoverPersonaje(medicoInstance.transform, puntoSalidaMedico.position));

        Destroy(medicoInstance);

       // dialogueManager.botonIngreso.interactable = true;
       // dialogueManager.botonRechazo.interactable = true;

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
         

            if (personajeActual.estado == CharacterState.Sano)
            {
                Debug.Log("El personaje está sano.");
              
            }
            else if (personajeActual.estado == CharacterState.Enfermo)
            {
                Debug.Log("El personaje está enfermo.");
           
            }
        }
    }


    public void ReiniciarNivel()
    {
        // Resetea la variable para permitir el uso del médico en el próximo nivel
        dialogueManager.medicoUsado = false;
    }
}

