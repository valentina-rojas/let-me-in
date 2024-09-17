using System.Collections;
using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
  public Sprite[] sprites;
  private SpriteRenderer spriteRenderer;
  private Coroutine dangerAnimationCoroutine;

  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.sprite = sprites[0];
  }


  public void StartDangerAnimation()
  {
    if (dangerAnimationCoroutine == null)
    {
      dangerAnimationCoroutine = StartCoroutine(PlayDangerAnimation());
    }
  }


  public void StopDangerAnimation()
  {
    if (dangerAnimationCoroutine != null)
    {
      StopCoroutine(dangerAnimationCoroutine);
      dangerAnimationCoroutine = null;
    }
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