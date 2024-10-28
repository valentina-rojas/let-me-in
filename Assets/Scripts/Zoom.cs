using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Zoom : MonoBehaviour, IDragHandler
{
    public RectTransform lupa; // El RectTransform de la lupa
    public bool isLupaVisible = false;
    public GameObject panelLupa;
    public RectTransform areaLimite;


    // Método que se llama cuando se arrastra el objeto
    public void OnDrag(PointerEventData eventData)
    {
        // Mueve la lupa a la posición del puntero del mouse durante el arrastre
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lupa.parent as RectTransform, // El contenedor de la lupa
            eventData.position,           // La posición actual del puntero
            eventData.pressEventCamera,   // La cámara que interactúa con el UI
            out pos
        );

        // Limitar la posición de la lupa al área designada
        Vector2 clampedPosition = ClampToArea(pos);
        lupa.anchoredPosition = clampedPosition; // Actualiza la posición de la lupa
    }

    // Método para limitar la posición de la lupa al área definida
    private Vector2 ClampToArea(Vector2 position)
    {
        Vector3[] corners = new Vector3[4];
        areaLimite.GetWorldCorners(corners); // Obtiene los límites del área

        // Convertir los límites del área a coordenadas locales
        Vector2 minBounds = lupa.parent.InverseTransformPoint(corners[0]);
        Vector2 maxBounds = lupa.parent.InverseTransformPoint(corners[2]);

        // Limitar la posición dentro de esos límites
        float clampedX = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);

        return new Vector2(clampedX, clampedY);
    }

    public void ToggleLupa()
    {
        isLupaVisible = !isLupaVisible; // Alterna el estado de visibilidad
        panelLupa.SetActive(isLupaVisible); // Activa o desactiva el panel
    }
}
