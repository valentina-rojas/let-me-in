using UnityEngine;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour, IDragHandler
{
    public Camera magnifyingCamera; // La cámara secundaria que generará el zoom
    private RectTransform rectTransform; // El RectTransform de la lupa

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Este método permite arrastrar la lupa por la pantalla
    public void OnDrag(PointerEventData eventData)
    {
        // Mueve la lupa en la dirección del ratón
        rectTransform.anchoredPosition += eventData.delta;
    }

    void Update()
    {
        // Convertir la posición de la lupa (en coordenadas de UI) a coordenadas de mundo
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Actualizar la posición de la cámara secundaria para que siga a la lupa
        magnifyingCamera.transform.position = new Vector3(worldPosition.x, worldPosition.y, magnifyingCamera.transform.position.z);
    }
}
