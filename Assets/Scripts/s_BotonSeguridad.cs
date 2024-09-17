using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_BotonSeguridad : MonoBehaviour
{
    public GameObject PanelSeguridad;
    private Coroutine toggleCoroutine;
    public AudioSource audioSeguridad;
    public Button botonSeguridad;


   void Start()
    {
        botonSeguridad.interactable = false; 
    }

    public void Peligro()
    {
        if (!PanelSeguridad.activeInHierarchy)
        {
            if (toggleCoroutine == null)
            {
                toggleCoroutine = StartCoroutine(TogglePanel());
            }
        }
    }

    IEnumerator TogglePanel()
    {
        while (true)
        {
            // Activate the panel and play sound
            PanelSeguridad.SetActive(true);
            audioSeguridad.Play();

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Deactivate the panel
            PanelSeguridad.SetActive(false);

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);
        }
    }

    public void DetenerPeligro()
    {
        if (toggleCoroutine != null)
        {
            StopCoroutine(toggleCoroutine);
            toggleCoroutine = null;
            PanelSeguridad.SetActive(false); // Optionally deactivate the panel when stopping
            audioSeguridad.Stop(); // Stop the sound when deactivating the panel
        }
    }

}
