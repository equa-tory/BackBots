using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempTitle : MonoBehaviour
{
    public TMP_Text text;

    public void Init(string titleText, Color textColor){
        text.text=titleText;
        text.color=textColor;
    }

    public void Die(){Destroy(gameObject);}
}
