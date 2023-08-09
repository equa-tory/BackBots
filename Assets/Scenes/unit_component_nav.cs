// ShowGoldenPath.cs
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
public class unit_component_nav : MonoBehaviour {
	
	// public List<Transform> target = new List<Transform>();

	// public void Move(){
	// 	float closestTargetDistance = float.MaxValue;
	// 	NavMeshPath path = null;
	// 	NavMeshPath shortestPath = null;

	// 	if(target.Length<=0)return;

	// 	if(NavMesh.CalculatePath(transform.position,target.position,NavMesh.AllAreas,path)){
	// 		float d = Vector3.Distance(transform.position,path.corners[0]);

	// 		path = new NavMeshPath();

	// 		for(int j=1;j<path.corners.Length;j++){
	// 			d+=Vector3.Distance(path.corners[j-1],path.corners[j]);
	// 		}

	// 		if(d<closestTargetDistance){
	// 			closestTargetDistance=d;
	// 			shortestPath = path;
	// 		}
	// 	}

	// 	if(shortestPath!=null){Debug.Log("Move!");}
	// }

	// private void OnGUI() {
	// 	if(GUI.Button(new Rect(20,20,300,50),"Move To Target")) Move();	
	// }
}