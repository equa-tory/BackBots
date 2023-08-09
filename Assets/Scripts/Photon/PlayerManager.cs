using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    GameObject pc;

    public float spawnCd;
    private float spawnTimer;
    private bool spawned = true;

    
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        pc = PhotonNetwork.Instantiate(Path.Combine("PlayerObj"), Vector3.zero, Quaternion.identity, 0, new object[] {PV.ViewID});
    }

    public void Die()
    {
        // GameManager.Instance.pc.Remove(pc.transform.GetChild(0).GetComponent<PlayerController>());
        PhotonNetwork.Destroy(pc.gameObject);

        spawnTimer = spawnCd;
        spawned = false;
    }

}
