using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_InformacionVisible : MonoBehaviour
{
    public GameObject PanelInformacion;

    public void Trigger()
    {
        if (PanelInformacion.activeInHierarchy == false)
        {
            PanelInformacion.SetActive(true);

        }
        else
        {
            PanelInformacion.SetActive(false);
        }

    }
}
