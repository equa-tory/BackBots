// ShowGoldenPath.cs
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
public class unit_component_nav : MonoBehaviour {
	
	public Transform[] targets;
	public NavMeshAgent agent;
	public NavMeshPath shortestPath;
	public Transform shotestTarget;
	
	public Transform debug;
	private Rigidbody rb;
	public float speed;


	private void Awake() {
		rb = GetComponent<Rigidbody>();
	}

	public void Move(){

		float closestTargetDistance = float.MaxValue;
		NavMeshPath path = null;

		for(int i=0;i<targets.Length;i++){
			if(targets[i]==null)continue;
			path=new NavMeshPath();

			if(NavMesh.CalculatePath(transform.position,targets[i].position,agent.areaMask,path)){
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

		// if(shortestPath!=null){debug.position = shortestPath.corners[1];}

		// if(shortestPath!=null){agent.SetPath(shortestPath);}
	}

	private void Update() {
		Move();

		if(shortestPath!=null){
			if(shortestPath.corners[1]!=null) rb.AddForce((shortestPath.corners[1] - transform.position).normalized * speed);
			else rb.AddForce((shotestTarget.position-transform.position).normalized*speed);
		}
	}
}