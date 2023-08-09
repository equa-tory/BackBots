using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BackBot))]
public class MoveToClosestTargetEditor : Editor
{
    private void OnSceneGUI() {
        GUIStyle Style = new GUIStyle(){
            normal = new GUIStyleState(){
                textColor = Color.white
            }
        };

        BackBot moveToClosestTargetEditor = (BackBot)target;
        
        if(!moveToClosestTargetEditor) return;

        int closestIndex = 0;
        float closestTargetDistance = float.MaxValue;
        List<NavMeshPath> paths = new List<NavMeshPath>();

        for(int i=0;i<moveToClosestTargetEditor.targets.Count;i++){
            paths.Add(new NavMeshPath());
            if(moveToClosestTargetEditor.targets[i]==null)continue;
            NavMeshPath path = paths[i];

            if(NavMesh.CalculatePath(moveToClosestTargetEditor.transform.position,
                moveToClosestTargetEditor.targets[i].position,NavMesh.AllAreas,path)){
                    float d = Vector3.Distance(moveToClosestTargetEditor.transform.position,path.corners[0]);

                    for(int j=1;j<path.corners.Length;j++){
                        d+=Vector3.Distance(path.corners[j-1],path.corners[j]);
                    }

                    if(d<closestTargetDistance){
                        closestTargetDistance=d;
                        closestIndex=i;
                    }

                    Handles.Label(moveToClosestTargetEditor.targets[i].position,$"Vector3 Distance: {Vector3.Distance(moveToClosestTargetEditor.transform.position,moveToClosestTargetEditor.targets[i].position).ToString("N3")}\r\nPath Distance: {d.ToString("N3")}",Style);
                }
                else
                {
                    Handles.Label(moveToClosestTargetEditor.targets[i].position,$"Vector3 Distance: {Vector3.Distance(moveToClosestTargetEditor.transform.position,moveToClosestTargetEditor.targets[i].position).ToString("N3")}\r\nPath Distance: Invalid Path",Style);
                }

        }

        foreach(NavMeshPath path in paths){
            if(paths.IndexOf(path)==closestIndex) Handles.color = Color.green;
            else Handles.color = Color.red;


            if(path.corners.Length > 0){
                Handles.DrawLine(moveToClosestTargetEditor.transform.position,path.corners[0]);
                
                for(int i =0;i<path.corners.Length-1;i++){
                    Handles.DrawLine(path.corners[i],path.corners[i+1]);
                    Handles.Label(path.corners[i],"Corner №" + i);
                }
            }
        }
    }
}
