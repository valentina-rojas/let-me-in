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

                gameManager.OnBotonIngresoClick();
            }
            else if (dragDistance < 0) // Arrastrando hacia la izquierda
            {
                palancaSprite.sprite = palancaIzquierda;

                gameManager.OnBotonRechazoClick();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        palancaSprite.sprite = palancaNeutral; // Regresar la palanca a su sprite neutral
    }
}
