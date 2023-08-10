using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BackBot : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    public bool isEnabled;


    public BackBotInfo info;
    public Image bb_image;
    public AudioSource source;

    private Rigidbody rb;
	public float speed;
    
    public List<Transform> targets;

	public NavMeshPath shortestPath;
	private Transform shotestTarget;

    public float antiStuckCd = 1.5f;
    private float antiStuckTimer;
    public float antiStuckForce;

    public LayerMask playerMask;
    public LayerMask groundMask;

    public float sightRange = 15f;
    public float attackRange = 5f;

    private bool walkPointSet;
    public float walkPointRange;
    public Transform walkPoint;


    private void Awake() {
        
        SetUp();
    }

    private void Update() {
        
        //Clear Nulls
        targets.RemoveAll(Transform => Transform == null);

        if(targets.Count>0){
            foreach(Transform c in targets){
                if(c.GetComponent<PlayerController>().inSafeZone) targets.Remove(c);
            }
        }

        AI();

    }

    //-------------------------------------------------------------------------------------------------------------------

    public override void OnPlayerEnteredRoom(Player player){
        PV.RPC(nameof(RPC_Init),RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Init(){
        Invoke(nameof(Init),1f);
    }
    private void Init(){
        this.bb_image.sprite = info.bb_image;
        this.source.clip = info.bb_ost;
        this.source.Play();
    }

    private void SetUp(){

        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        GetComponent<SphereCollider>().radius = sightRange;

        Init();
        // PV.RPC(nameof(RPC_Init),RpcTarget.All);
        antiStuckTimer = antiStuckCd;

        if(!PhotonNetwork.IsMasterClient) Destroy(rb);

    }

    public void AI(){

        Attack();

        if(!isEnabled) return;

        SearchPath();

        //SpeedControl
        if (rb.velocity.magnitude > speed*1.5f)
                rb.velocity = rb.velocity.normalized * speed * 1.5f;

        //Move
		if(shortestPath!=null){
			if(shortestPath.corners[1]!=null) rb.AddForce((shortestPath.corners[1] - transform.position).normalized * speed);
			else if(shortestPath.corners[1]==null)rb.AddForce((shotestTarget.position-transform.position).normalized*speed);
		}

        //AntiStuck
        // Debug.Log(rb.velocity.magnitude);
        if(rb.velocity.magnitude < 2){
            if(antiStuckTimer>0)antiStuckTimer-=Time.deltaTime;
            else {
                antiStuckTimer = antiStuckCd;
                int r = Random.Range(0,3);

                if(r==0) rb.AddForce(transform.right*antiStuckForce);
                else if(r==1)rb.AddForce(-transform.right*antiStuckForce);
                else rb.AddForce(transform.up*antiStuckForce);
            }

            walkPointSet=false;
        }

        if(PhotonNetwork.IsMasterClient) Patroling();


    }

    private void Patroling(){

        if(!walkPointSet) SearchWalkPoint();
        

        //Research if distance = 5f
        Vector3 distanceToWalk = transform.position - walkPoint.position;
        if(distanceToWalk.magnitude<5f) walkPointSet=false;
    }
    private void SearchWalkPoint(){
        
        float rZ = Random.Range(-walkPointRange,walkPointRange);
        float rX = Random.Range(-walkPointRange,walkPointRange);
        
        Vector3 _pos = new Vector3(transform.position.x + rX, transform.position.y, transform.position.z + rZ);
        walkPoint.position = _pos;
        // PV.RPC(nameof(RPC_SetWalkpoint),RpcTarget.All, _pos);

        if(Physics.Raycast(walkPoint.position, -transform.up, 2f, groundMask)) walkPointSet=true;
    }

    [PunRPC]
    public void RPC_SetWalkpoint(Vector3 _pos){
        walkPoint.position = _pos;
    }

    protected void SearchPath(){

		float closestTargetDistance = float.MaxValue;
		NavMeshPath path = null;

        if(targets.Count>0){
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
        else
        {
            path=new NavMeshPath();
            if(NavMesh.CalculatePath(transform.position,walkPoint.position,NavMesh.AllAreas,path)){
                    shotestTarget = walkPoint;

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

    public void Attack(){
        if(Physics.CheckSphere(transform.position,attackRange,playerMask))
            Physics.OverlapSphere(transform.position,attackRange,playerMask)[0].GetComponent<PlayerController>().Die();
    }

    //-------------------------------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other) {
    }
    private void OnTriggerStay(Collider other) {
        PlayerController _pc = other.GetComponent<PlayerController>(); 
        
        if(!_pc.inSafeZone){
            if(!targets.Contains(_pc.transform)) {
                targets.Add(other.transform);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.GetComponent<PlayerController>()) targets.Remove(other.transform);
    }

    //-------------------------------------------------------------------------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(walkPoint.position, .25f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }

}
