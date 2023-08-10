using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class BB_Manager : MonoBehaviour
{
    public static BB_Manager Instance;

    // public BackBotInfo[] backBotInfos;
    // private List<BackBotInfo> infos = new List<BackBotInfo>();
    public GameObject[] backBots;
    private List<GameObject> bbs = new List<GameObject>();

    public Transform[] bb_poses;

    public int bbsToSpawn;


    private void Awake() {
        Instance = this;

        // for(int i=0;i<backBotInfos.Length;i++) infos.Add(backBotInfos[i]);
        for(int i=0;i<backBots.Length;i++) bbs.Add(backBots[i]);
    }

    public void CreateBBs(){

        for(int i=0;i<bbsToSpawn;i++){

            int r =Random.Range(0,bbs.Count);
            int p =Random.Range(0,bb_poses.Length);

            GameObject _bb = PhotonNetwork.Instantiate(Path.Combine("BackBots/"+bbs[r].name),bb_poses[p].position,bb_poses[p].rotation);
            // _bb.transform.GetChild(0).GetComponent<BackBot>().info = infos[r];
            // infos.Remove(infos[r]);
            bbs.Remove(bbs[r]);

        }
    
    }

}
