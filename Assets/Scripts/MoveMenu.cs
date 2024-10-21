using UnityEngine;

public class ScrollingImages : MonoBehaviour
{
    public RectTransform image1; // Asigna la primera imagen en el Inspector
    public RectTransform image2; // Asigna la segunda imagen en el Inspector
    public float scrollSpeed = 200f; // Velocidad de desplazamiento

    private void Update()
    {
        // Mover ambas imágenes hacia la izquierda
        image1.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;
        image2.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

        // Comprobar si la primera imagen ha salido de la pantalla
        if (image1.anchoredPosition.x < -image1.rect.width)
        {
            // Reposicionar la primera imagen a la derecha
            image1.anchoredPosition += new Vector2(image1.rect.width * 2, 0);
        }

        // Comprobar si la segunda imagen ha salido de la pantalla
        if (image2.anchoredPosition.x < -image2.rect.width)
        {
            // Reposicionar la segunda imagen a la derecha
            image2.anchoredPosition += new Vector2(image2.rect.width * 2, 0);
        }
    }
}
