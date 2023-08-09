using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if(!target&&GameManager.Instance.pc.Count>0) {
            List<PlayerController> _pc = GameManager.Instance.pc;
            foreach(PlayerController l in _pc){
                if(l.PV.IsMine){
                    target = l.transform;
                    break;
                }
            }
        }
        
        if(target){
            transform.LookAt(target.position);
            transform.Rotate(new Vector3(0,180,0));
        }
    }
}
