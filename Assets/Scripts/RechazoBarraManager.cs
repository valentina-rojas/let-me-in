using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Analytics; 
using static EventManager;

public class RechazoBarraManager : MonoBehaviour
{
    public Slider barraRechazoSanos;    // La barra que representa el rechazo de sanos
    public float maxRechazos = 3f;      // Máximo número de rechazos permitidos
    private float rechazosActuales = 0f; // Rechazos actuales

    public Image fillBarImageRechazo;   // Imagen de la barra de relleno

    public Color colorVerde = Color.green;
    public Color colorAmarillo = Color.yellow;
    public Color colorRojo = Color.red;

    public RectTransform panelPerdiste;

    public CharactersManager charactersManager;

    public AudioSource loadingBar;

    void Start()
    {
        // Inicializar la barra con 0 rechazos
        if (barraRechazoSanos != null)
        {
            barraRechazoSanos.maxValue = maxRechazos;
            barraRechazoSanos.value = rechazosActuales;
        }
    }

    // Método para actualizar la barra cuando rechazas a un personaje sano
    public void RechazarSano()
    {
        // Incrementa los rechazos
        rechazosActuales += 1;

        // Asegurarse de no exceder el nivel máximo de rechazos
        if (rechazosActuales > maxRechazos)
        {
            rechazosActuales = maxRechazos;
        }

        // Actualizar la barra visualmente
        if (barraRechazoSanos != null)
        {
            barraRechazoSanos.value = rechazosActuales;
            ActualizarColorBarraRechazo();
            loadingBar.Play();
        }

        // Puedes agregar lógica adicional si el nivel de rechazos llega al máximo
        if (rechazosActuales >= maxRechazos)
        {
            Debug.Log("¡Has rechazado demasiados sanos! Has perdido.");
            Perder();
        }
    }

   
    private void ActualizarColorBarraRechazo()
    {
        float porcentajeRechazo = rechazosActuales / maxRechazos;

        if (porcentajeRechazo <= 0.34f)
        {
            fillBarImageRechazo.color = colorVerde;  
        }
        else if (porcentajeRechazo <= 0.67f)
        {
            fillBarImageRechazo.color = colorAmarillo;  
        }
        else
        {
            fillBarImageRechazo.color = colorRojo;  
        }
    }


    private void Perder()
    {
        RegisterGameOverEvent();

        CharactersManager charactersManager = FindObjectOfType<CharactersManager>();
        if (charactersManager != null)
        {
            charactersManager.DetenerPersonajes();  
        }


        panelPerdiste.gameObject.SetActive(true);
    }

      private void RegisterGameOverEvent()
{
     // Debug para verificars
    Debug.Log($"[DEBUG] GameOver registrado - Nivel: {GameData.NivelActual}, Razón: Fired");

    // Crear y configurar el evento
    GameOverEvent gameOver = new GameOverEvent();
    gameOver.level = GameData.NivelActual;
    gameOver.reason = "Fired";

    // Grabar el evento 
    #if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(gameOver);
    #else
        Debug.Log("[ANALYTICS] Evento GameOverEvent registrado");
    #endif
}
}
