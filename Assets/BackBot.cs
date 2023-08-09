using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;


public class BackBot : MonoBehaviour
{
    public BackBotInfo info;
    public Image bb_image;
    public AudioSource source;

    private Rigidbody rb;
	public float speed;
    
    public List<Transform> targets;
	public NavMeshPath shortestPath;
	private Transform shotestTarget;



    private void Awake() {
        
        SetUp();
    }

    private void Update() {

        if(targets.Count<=0){
            foreach(PlayerController l in GameManager.Instance.pc){
                targets.Add(l.transform);
            }            

            return;
        }
        

        Move();

		if(shortestPath!=null){
			if(shortestPath.corners[1]!=null) rb.AddForce((shortestPath.corners[1] - transform.position).normalized * speed);
			else rb.AddForce((shotestTarget.position-transform.position).normalized*speed);
		}

    }

    //-------------------------------------------------------------------------------------------------------------------

    private void SetUp(){

        this.bb_image.sprite = info.bb_image;
        this.source.clip = info.bb_ost;
        this.source.Play();

        rb = GetComponent<Rigidbody>();

    }

    public void Move(){

		float closestTargetDistance = float.MaxValue;
		NavMeshPath path = null;

		for(int i=0;i<targets.Count;i++){
			if(targets[i]==null)continue;
			path=new NavMeshPath();

			if(NavMesh.CalculatePath(transform.position,targets[i].position,NavMesh.AllAreas,path)){
				shotestTarget = targets[i];

				float d = Vector3.Distance(transform.position,path.corners[0]);

				for(int j=1;j<path.corners.Length;j++){
					d+=Vector3.Distance(path.corners[j-1],path.corners[j]);
				}

				if(d<closestTargetDistance){
					closestTargetDistance=d;
					shortestPath = path;
				}
			}
		}

	}

}
