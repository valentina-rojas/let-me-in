using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkeningLayer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private Color currentColor;
    private float targetAlpha = 0.6f; // Opacidad final deseada
    private int characterCount = 0;
    public float fadeDuration = 1.0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentColor = spriteRenderer.color;
        currentColor.a = 0f; // Comienza completamente transparente
        spriteRenderer.color = currentColor;
    }

    public void OnCharacterSpawned()
    {
        characterCount++;

        if (characterCount >= 2)
        {
            characterCount = 0; // Reinicia el conteo de personajes
            StartCoroutine(FadeTo(targetAlpha)); // Comienza a oscurecer
        }
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = currentColor.a;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            currentColor.a = alpha;
            spriteRenderer.color = currentColor;
            yield return null;
        }

        currentColor.a = targetAlpha;
        spriteRenderer.color = currentColor;
    }

    public void ResetLayer()
    {
        currentColor.a = 0f; // Resetea la capa a transparente
        spriteRenderer.color = currentColor;
    }
}

