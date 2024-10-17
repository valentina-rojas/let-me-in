using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Zoom : MonoBehaviour, IDragHandler
{
    public RectTransform lupa; // El RectTransform de la lupa

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

        lupa.anchoredPosition = pos; // Actualiza la posición de la lupa
    }
}
