using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TMP_Text _text;

    private void Start() {
        _text.text = playerPV.Owner.NickName;
        if(playerPV.IsMine) gameObject.SetActive(false);
    } 
}
