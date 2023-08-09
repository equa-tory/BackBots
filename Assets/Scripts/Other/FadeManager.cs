using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Animator anim;
    public TMP_Text deathTitle;


    private void Awake() {
        Instance=this;
        anim.SetTrigger("FadeOut");
    }

    public void DarkFade(){anim.SetTrigger("Dark");}
    public void PhotoFade(){anim.SetTrigger("Photo");}
    public void DyingFade(){anim.SetTrigger("Dying");}

}
