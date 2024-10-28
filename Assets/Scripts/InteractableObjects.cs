using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class InteractableObjects : MonoBehaviour
{
      [SerializeField]
    public List<EventTrigger> eventTriggers; // Asigna los EventTriggers en el Inspector

    public void DesactivarEventTriggers()
    {
        foreach (var trigger in eventTriggers)
        {
            if (trigger != null)
            {
                trigger.enabled = false;
            }
        }
    }

    public void ActivarEventTriggers()
    {
        foreach (var trigger in eventTriggers)
        {
            if (trigger != null)
            {
                trigger.enabled = true;
            }
        }
    }
}
