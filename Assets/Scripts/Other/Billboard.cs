using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if(!target&&GameManager.Instance.pc) target = GameManager.Instance.pc.cam.transform;
        
        if(target){
            transform.LookAt(target.position);
            transform.Rotate(new Vector3(0,180,0));
        }
    }
}
