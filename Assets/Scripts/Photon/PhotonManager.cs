using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class PhotonManager : MonoBehaviourPunCallbacks
{

    public static PhotonManager Instance;

    public bool loaded;
    public bool onlineMode;

    public GameObject menuGo;
    public GameObject menuCam;

    public TempTitle tt;
    public Transform ttPos;

    public TMP_InputField nickInput;

    //-------------------------------------------------------------------------------------------------------------------

    private void Awake() {
        Instance=this;
        LoadNickname();
    }

    private void Start() {

        Debug.Log("<color=cyan> Connecting to server! </color>");
        PhotonNetwork.ConnectUsingSettings();

        Invoke(nameof(EnterOfflineMode),6f);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            if(loaded && onlineMode && menuGo.activeSelf) Load(true);
            else if(loaded && !onlineMode && menuGo.activeSelf) Load(false);
        }
    }

    
    //-------------------------------------------------------------------------------------------------------------------

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene=true;
        
        CancelInvoke(nameof(EnterOfflineMode));
        
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("<color=cyan> Joined Lobby! </color>");

        Connect();
    }

    public override void OnJoinedRoom()
    {
        string tt_text = "Player Online: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();

        TempTitle _tt = Instantiate(tt,ttPos);
        _tt.Init(tt_text,Color.cyan);

        loaded=true;
        onlineMode=true;

        menuGo.SetActive(true);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        TempTitle _tt = Instantiate(tt,ttPos);
        _tt.Init(message,Color.cyan);
    }

    //-------------------------------------------------------------------------------------------------------------------

    public void Connect(){
        RoomOptions ro = new RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("test",ro,TypedLobby.Default);
    }

    public void EnterOfflineMode(){

        loaded=true;
        onlineMode=false;

        menuGo.SetActive(true);

        TempTitle _tt = Instantiate(tt,ttPos);
        _tt.Init("Offline!",Color.cyan);

        Debug.Log("<color=cyan> Offline! </color>");

    }

    public void Load(bool mode){
        menuGo.SetActive(false);
        menuCam.SetActive(false);
        RoomManager.Instance.CreatePM(isOnline:mode);
    }

    public void SaveNickname(){
        string _text = nickInput.text;
        PhotonNetwork.NickName = _text;
        PlayerPrefs.SetString("Nickname",_text);
    }

    private void LoadNickname(){
        if(PlayerPrefs.HasKey("Nickname")){
            string _text = PlayerPrefs.GetString("Nickname");
            PhotonNetwork.NickName = _text;
            nickInput.text = _text;
        }
        else{
            nickInput.text = "Player#" + Random.Range(0,10000).ToString("0000");
            SaveNickname();
        }
    }
}
