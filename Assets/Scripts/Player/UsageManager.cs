using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsageManager : MonoBehaviour
{
    public float range;
    public bool canUse;
    public LayerMask usableMask;

    public GameObject usableIcon;

    private void Update() {
        if(Physics.Raycast(transform.position,transform.forward,out RaycastHit hit, range, usableMask)){
            canUse = true;
            usableIcon.SetActive(true);
            usableIcon.transform.position = hit.collider.transform.position;
            if(Input.GetKeyDown(KeyCode.Mouse1)){
                // hit.collider.GetComponent<Item>()?.Use();
            }
        }
        else {
            canUse = false;
            usableIcon.SetActive(false);

        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position,transform.forward*range);
    }
}
