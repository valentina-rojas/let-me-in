using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public float velocidadMovimiento = 0.5f; 
    private Vector3 startPosition;

    public GameObject capaNivel1;
    public GameObject capaNivel2;

    void Start()
    {
        startPosition = transform.position;

           MostrarCapaPorNivel();
    }

    void Update()
    {
        float nuevoPosX = Mathf.Repeat(Time.time * velocidadMovimiento, 30f); 
        transform.position = startPosition + Vector3.right * nuevoPosX;
    }

     void MostrarCapaPorNivel()
    {
        if (GameData.NivelActual == 1)
        {
            capaNivel1.SetActive(true);
            capaNivel2.SetActive(false);
        }
        else if (GameData.NivelActual == 2)
        {
            capaNivel1.SetActive(false);
            capaNivel2.SetActive(true);
        }
        else
        {
            // Si hay más niveles o no corresponde a nivel 1 o 2
            capaNivel1.SetActive(false);
            capaNivel2.SetActive(false);
        }
    }


}