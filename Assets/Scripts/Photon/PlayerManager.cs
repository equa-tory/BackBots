using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using TMPro;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    GameObject pc;

    public float spawnCd;
    private float spawnTimer;
    private bool spawned = true;

    public GameObject spawnCam;
    public TMP_Text respawnText;

    
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

    private void Update() {
        if(!PV.IsMine) return;

        if(spawnTimer>0){
            spawnTimer-=Time.deltaTime;
            respawnText.text = "Respawn: " + Mathf.Ceil(spawnTimer).ToString();
        }
        else if(!spawned){
            spawned=true;
            CreateController();
        }

        if(spawnTimer<=0 && Input.GetKeyDown(KeyCode.Escape)){
            ExitMenu();
        }
    }

    void CreateController()
    {
        spawnCam.SetActive(false);

        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        pc = PhotonNetwork.Instantiate(Path.Combine("PlayerObj"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {PV.ViewID});
    }

    public void ExitMenu(){
        PhotonManager.Instance.ShowMainMenu();

        GameManager.Instance.pc.Remove(pc.transform.GetChild(0).GetComponent<PlayerController>());
        PhotonNetwork.Destroy(pc.gameObject);
        Destroy(gameObject);
    }

    public void Die()
    {
        GameManager.Instance.pc.Remove(pc.transform.GetChild(0).GetComponent<PlayerController>());
        PhotonNetwork.Destroy(pc.gameObject);

        spawnCam.SetActive(true);

        spawnTimer = spawnCd;
        spawned = false;
    }

}
