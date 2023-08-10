using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<PlayerController>()) other.GetComponent<PlayerController>().inSafeZone=true;
    }
    private void OnTriggerExit(Collider other) {
        if(other.GetComponent<PlayerController>()) other.GetComponent<PlayerController>().inSafeZone=false;
    }
}
