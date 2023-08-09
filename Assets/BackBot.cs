using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;


public class BackBot : MonoBehaviour
{
    public NavMeshPath path;
    private Rigidbody rb;
    private float elapsed = 0.0f;

    public BackBotInfo info;

    public Image bb_image;
    public AudioSource source;
    
    public float speed;

    public Transform target;

    IEnumerator Start() {

        SetUp();
		this.path = new NavMeshPath();

		while (true) {

			yield return new WaitForSeconds(1);

			NavMesh.CalculatePath(this.transform.position,this.target.position,NavMesh.AllAreas,this.path);
		}
	}

    private void Update() {

        if(!target && GameManager.Instance.pc) target = GameManager.Instance.pc.transform;
        if(!target)return;

        if(elapsed<1) elapsed+=Time.deltaTime;
        else{
            elapsed=0;
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);

            // for (int i=0;i<path.corners.Length - 1;i++)
            //     Debug.DrawLine(path.corners[i],path.corners[i+1],Color.red);
        }

        // if (this.path.corners.Length < 2) {

		// 	return;
		// }

		// this.rb.velocity = (this.path.corners[1] - this.path.corners[0]).normalized * 4;
            Debug.Log("!");

        for (int __i = 0; __i < this.path.corners.Length - 1; __i++) {
			Debug.DrawLine(this.path.corners[__i],this.path.corners[__i + 1],Color.red);

		}

		if (this.path.corners.Length < 2) {

			return;
		}

		this.rb.velocity = (this.path.corners[1] - this.path.corners[0]).normalized * 4;
    }

    //-------------------------------------------------------------------------------------------------------------------

    private void SetUp(){

        this.bb_image.sprite = info.bb_image;
        this.source.clip = info.bb_ost;
        this.source.Play();

        rb = GetComponent<Rigidbody>();

    }

}
