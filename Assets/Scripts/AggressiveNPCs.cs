using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Analytics; 
using static EventManager;

public class AggressiveNPCs : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public s_GameManager gameManager;
    public CharactersManager charactersManager;
    public StressBar stressBar;
    public GameObject panelPerdiste;
    public GameObject seguridadPrefab;
    public Transform spawnPointSeguridad;
    public Transform targetPoint;

    private GameObject seguridadInstance;

    private float tiempoRestante;
    private bool temporizadorActivo = false;
    public GameObject PanelSeguridad;
    public GameObject PanelTimer;
    private Coroutine toggleCoroutine;
    public AudioSource audioSeguridad;
    public AudioSource pasosSeguridad;
    public AudioSource escobaSeguridad;
    public AudioSource golpe;


    public GameObject[] vidriosRotos;  // Array para múltiples filtros de vidrio roto
    private int vidrioActual = 0;      // Índice para saber qué filtro activar


    public Button botonSeguridad;

    public AudioSource sonidoBoton;

    public float tiempoBaseTemporizador;

    public Transform cameraTransform;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 3f;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;
    private Coroutine shakeCoroutine;

    private float tiempoInicioAgresion;


    void Start()
    {
        audioSeguridad.Stop();
        pasosSeguridad.Stop();
        escobaSeguridad.Stop();
        botonSeguridad.interactable = false;

        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.position;
        }

        if (gameManager.NivelActual == 2)
        {
            tiempoBaseTemporizador = 3f;
        }
        else
        {
            tiempoBaseTemporizador = 5f;
        }
    }

    void Update()
    {
        if (temporizadorActivo)
        {
            tiempoRestante -= Time.deltaTime;
            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                temporizadorActivo = false;
                ActualizarTextoTemporizador();
                FinTemporizador();
            }
            else
            {
                ActualizarTextoTemporizador();
            }
        }
    }


    void ShakeCamera()
    {
        if (!isShaking)
        {
            shakeCoroutine = StartCoroutine(ShakeCameraCoroutine());
        }
    }

    IEnumerator ShakeCameraCoroutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            cameraTransform.position = originalCameraPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = originalCameraPosition;
        isShaking = false;
    }

    public void MostrarComportamientoAgresivo()
    {

        Debug.Log("¡El personaje está actuando de manera agresiva!");

          tiempoInicioAgresion = Time.time; 

        if (temporizadorActivo)
        {
            temporizadorActivo = false;
        }

        botonSeguridad.interactable = true;
        StartTimer(tiempoBaseTemporizador);
        Peligro();


        if (cameraTransform != null)
        {
            ShakeCamera();
        }

        // Activar un nuevo filtro de vidrio roto
        if (vidriosRotos.Length > 0)
        {

            // Activar el siguiente filtro de vidrio roto
            vidrioActual = (vidrioActual + 1) % vidriosRotos.Length;
            if (vidriosRotos[vidrioActual] != null)
            {
                vidriosRotos[vidrioActual].SetActive(true);
            }
        }
    }

    void StartTimer(float tiempo)
    {
        PanelTimer.SetActive(true);
        timerText.gameObject.SetActive(true);
        tiempoRestante = tiempo;
        temporizadorActivo = true;
        ActualizarTextoTemporizador();
    }

    void ActualizarTextoTemporizador()
    {
        if (timerText != null)
        {
            int tiempoEntero = Mathf.FloorToInt(tiempoRestante);
            timerText.text = tiempoEntero.ToString();
        }
    }


    void FinTemporizador()
    {
        RegisterGameOverEvent();

        DetenerPeligro();
        panelPerdiste.SetActive(true);
    }

       private void RegisterGameOverEvent()
{
     // Debug para verificars
    Debug.Log($"[DEBUG] GameOver registrado - Nivel: {GameData.NivelActual}, Razón: Riot");

    // Crear y configurar el evento
    GameOverEvent gameOver = new GameOverEvent();
    gameOver.level = GameData.NivelActual;
    gameOver.reason = "Riot";

    // Grabar el evento 
    #if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(gameOver);
    #else
        Debug.Log("[ANALYTICS] Evento GameOverEvent registrado");
    #endif
}

    public void Peligro()
    {
        if (!PanelSeguridad.activeInHierarchy)
        {
            if (toggleCoroutine == null)
            {
                toggleCoroutine = StartCoroutine(TogglePanel());
            }

            GameObject personajeAgresivoActual = charactersManager.GetCharacterGameObject();
            if (personajeAgresivoActual != null)
            {
                NPCAnimationController animController = personajeAgresivoActual.GetComponent<NPCAnimationController>();
                animController?.StartDangerAnimation();
            }
        }
    }

    IEnumerator TogglePanel()
    {
        while (true)
        {
            PanelSeguridad.SetActive(true);
            audioSeguridad.Play();

            yield return new WaitForSeconds(0.5f);

            PanelSeguridad.SetActive(false);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DetenerPeligro()
    {

        RegisterSecurityCalledEvent();

        if (toggleCoroutine != null)
        {
            StopCoroutine(toggleCoroutine);
            toggleCoroutine = null;
            PanelSeguridad.SetActive(false);
            PanelTimer.SetActive(false);
            audioSeguridad.Stop();
        }

    
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
            cameraTransform.position = originalCameraPosition;
            isShaking = false;
        }

        GameObject personajeAgresivoActual = charactersManager.GetCharacterGameObject();
        if (personajeAgresivoActual != null)
        {
            NPCAnimationController animController = personajeAgresivoActual.GetComponent<NPCAnimationController>();
            animController?.StopDangerAnimation();
        }
    }


   private void RegisterSecurityCalledEvent()
{
    float tiempoReaccion = Time.time - tiempoInicioAgresion;

    Debug.Log($"Tiempo de reacción: {tiempoReaccion:F2}s");

    // Crear y configurar el evento
    SecurityCalledEvent securityCalled = new SecurityCalledEvent();
   securityCalled.reactionTime = Mathf.RoundToInt(tiempoReaccion);

    // Grabar el evento 
    #if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(securityCalled);
    #else
        Debug.Log("[ANALYTICS] Evento SecurityCalledEvent registrado");
    #endif
}


    public void LlamarSeguridad()
    {
        sonidoBoton.Play();
        DetenerPeligro();
        botonSeguridad.interactable = false;
        if (temporizadorActivo)
        {
            temporizadorActivo = false;
            timerText.gameObject.SetActive(false);
        }


        StartCoroutine(EsperarAntesDeLlamarSeguridad(3f));
    }

    IEnumerator EsperarAntesDeLlamarSeguridad(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(gameManager.AbrirPuerta(20f));


        seguridadInstance = Instantiate(seguridadPrefab, spawnPointSeguridad.position, Quaternion.identity);

        pasosSeguridad.Play();
        escobaSeguridad.Play();


        GameObject personajeAgresivoActual = charactersManager.GetCharacterGameObject();

        if (personajeAgresivoActual != null)
        {

            yield return new WaitForSeconds(1f);


            StartCoroutine(EmpujarPersonajeAggressivo(personajeAgresivoActual.transform));
        }
    }

    IEnumerator EmpujarPersonajeAggressivo(Transform personajeAggressivo)
    {
        float distanciaSeguridadYAgresivo = 0.1f;
        float velocidadMovimiento = 2f;
        float velocidadSeguridad = 3.5f;


        Vector3 puntoFueraDePantalla = new Vector3(targetPoint.position.x - 1f, personajeAggressivo.position.y, personajeAggressivo.position.z);

        while (Vector3.Distance(personajeAggressivo.position, puntoFueraDePantalla) > 0.1f)
        {

            personajeAggressivo.position = Vector3.MoveTowards(personajeAggressivo.position, puntoFueraDePantalla, Time.deltaTime * velocidadMovimiento);


            if (seguridadInstance != null)
            {
                Vector3 posicionSeguridad = personajeAggressivo.position - new Vector3(distanciaSeguridadYAgresivo, 0, 0);
                seguridadInstance.transform.position = Vector3.MoveTowards(seguridadInstance.transform.position, posicionSeguridad, Time.deltaTime * velocidadSeguridad);
            }

            yield return null;
        }


        if (personajeAggressivo != null)
        {
            Destroy(personajeAggressivo.gameObject);
        }


        golpe.Play();

        stressBar.ActualizarEstres(1);

        Vector3 escalaOriginal = seguridadInstance.transform.localScale;

        if (escalaOriginal.x > 0)
        {
            seguridadInstance.transform.localScale = new Vector3(-escalaOriginal.x, escalaOriginal.y, escalaOriginal.z);
        }

        while (Vector3.Distance(seguridadInstance.transform.position, spawnPointSeguridad.position) > 0.1f)
        {
            seguridadInstance.transform.position = Vector3.MoveTowards(seguridadInstance.transform.position, spawnPointSeguridad.position, Time.deltaTime * velocidadSeguridad);
            yield return null;
        }

        if (seguridadInstance != null)
        {
            Destroy(seguridadInstance);
        }

        pasosSeguridad.Stop();
        escobaSeguridad.Stop();

        yield return new WaitForSeconds(1f);
        charactersManager.AparecerSiguientePersonaje();
    }
}
