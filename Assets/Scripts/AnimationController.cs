using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCAnimationController : MonoBehaviour
{
  public Sprite[] sprites;
   private AudioSource audioSource;
  public AudioClip sonidoAtaque; 
  private SpriteRenderer spriteRenderer;
  private Coroutine dangerAnimationCoroutine;


  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.sprite = sprites[0];

      // Crea y configura el AudioSource en tiempo de ejecución
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sonidoAtaque;
         audioSource.playOnAwake = false; // Evita que el audio se reproduzca al iniciar la escena
        audioSource.loop = true; // Hace que el audio se repita en loop
  }


  public void StartDangerAnimation()
  {
    if (dangerAnimationCoroutine == null)
    {
      dangerAnimationCoroutine = StartCoroutine(PlayDangerAnimation());
       audioSource.Play();

    }
  }


  public void StopDangerAnimation()
  {
    if (dangerAnimationCoroutine != null)
    {
      StopCoroutine(dangerAnimationCoroutine);
      dangerAnimationCoroutine = null;
    }

    audioSource.Stop();

     spriteRenderer.sprite = sprites[0];
  }

  private IEnumerator PlayDangerAnimation()
  {
    int currentIndex = 1;

    while (true)
    {
      spriteRenderer.sprite = sprites[currentIndex];

      currentIndex++;
      if (currentIndex >= sprites.Length)
      {
        currentIndex = 1;
      }

      yield return new WaitForSeconds(0.2f);
    }
  }
}