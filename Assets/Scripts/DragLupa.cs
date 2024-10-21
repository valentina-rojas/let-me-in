using UnityEngine;
using UnityEngine.EventSystems;

public class DragLupa : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out position
        );
        transform.position = transform.parent.TransformPoint(position);
    }
}