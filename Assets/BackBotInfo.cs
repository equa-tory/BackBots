using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptables/new BackBot", fileName = "bb_")]
public class BackBotInfo : ScriptableObject
{
    public string bb_name;
    public Sprite bb_image;
    public AudioClip bb_ost;
}
