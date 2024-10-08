using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeverController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image palancaSprite;
    public Sprite palancaNeutral;
    public Sprite palancaDerecha;
    public Sprite palancaIzquierda;

    private Vector2 startPointerPosition;
    private bool isDragged = false;
    private bool decisionMade = false;

    public s_GameManager gameManager;


    public void Start()
    {
        DesactivarPalanca();
    }


    public void DesactivarPalanca()
    {
        palancaSprite.raycastTarget = false;
    }

    public void ActivarPalanca()
    {
        palancaSprite.raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPointerPosition = eventData.position;
        isDragged = true;
        decisionMade = false; // Resetea la decisión al empezar a arrastrar
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            Vector2 currentPointerPosition = eventData.position;
            float dragDistance = currentPointerPosition.x - startPointerPosition.x;

            if (dragDistance > 0) // Arrastrando hacia la derecha
            {
                palancaSprite.sprite = palancaDerecha;

                // Verificar si no se ha tomado una decisión aún
                if (!decisionMade)
                {
                    gameManager.OnBotonIngresoClick();
                    decisionMade = true; // Marca que se ha tomado una decisión
                }
            }
            else if (dragDistance < 0) // Arrastrando hacia la izquierda
            {
                palancaSprite.sprite = palancaIzquierda;

                // Verificar si no se ha tomado una decisión aún
                if (!decisionMade)
                {
                    gameManager.OnBotonRechazoClick();
                    decisionMade = true; // Marca que se ha tomado una decisión
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        palancaSprite.sprite = palancaNeutral; // Regresar la palanca a su sprite neutral
        decisionMade = false; // Resetea la decisión al soltar la palanca
    }
}
