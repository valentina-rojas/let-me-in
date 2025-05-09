using System;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;

public class Services : MonoBehaviour
{
   [SerializeField] private MenuPrincipal menuPrincipal;

    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }

        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }
    public void StartDataCollection()
    {
        AnalyticsService.Instance.StartDataCollection();

        menuPrincipal.IniciarJuego();
    }


    public void StopDataCollection()
    {
        menuPrincipal.IniciarJuego();
    }
}
