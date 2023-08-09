using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;
    
    private bool isOnline;
    public GameObject pc;


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // CreatePM(this.isOnline);
    }

    public void CreatePM(bool isOnline)
    {
        this.isOnline = isOnline;

        if(this.isOnline) PhotonNetwork.Instantiate(Path.Combine("PlayerManager"), Vector3.zero, Quaternion.identity);
        else Instantiate(pc,Vector3.zero,Quaternion.identity).transform.GetChild(0)
            .GetComponent<PlayerController>().onlineMode=false;
    }


}
