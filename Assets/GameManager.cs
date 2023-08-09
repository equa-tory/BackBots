using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerController> pc;


    private void Awake() {
        if(Instance) { Destroy(gameObject); return; }
        Instance = this;
    }

    
}
