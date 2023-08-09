using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbDetection : MonoBehaviour
{
    public bool Obstruction;
    public GameObject Object;
    private Collider colnow;

    private void Update()
    {
        
        if(Object == null)
        {
            Obstruction = false;
        }
        if(colnow && !colnow.enabled) Obstruction = false;
        if (Object != null)
        {
            if (!Object.activeInHierarchy)
            {
                Obstruction = false;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (!Obstruction && !col.isTrigger)
        {
            Obstruction = true;
            Object = col.gameObject;
            colnow = col;
            
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col == colnow)
        {
            Obstruction = false;
        }

    }
}
