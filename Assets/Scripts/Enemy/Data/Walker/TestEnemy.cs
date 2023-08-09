using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class TestEnemy : Enemy
{

    public GameObject photo;
    public GameObject damageTrigger;
    public GameObject warningImage;
    public float maxChaseDistance;

    // public Projectile projectile;
    public float maxShootCd;
    public float minShootCd;
    private float shootTimer;

    public Animator anim;
    // public List<AttackSO> combo;


    private void Start() {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();

        damageTrigger.transform.localScale = new Vector3(info.attackWide*50,50,info.attackRange*50);
        //damageTrigger.transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,transform.localPosition.z+info.attackRange/2);

        agent.speed = info.walkSpeed;
    }

    private void Update() {
        
        AI();

        if(shootTimer>0) shootTimer-=Time.deltaTime;
        else if(shootTimer<=0&&target){
            shootTimer=Random.Range(-minShootCd,maxShootCd);

            // Projectile newProj = Instantiate(projectile,transform.position,Quaternion.identity);
            // newProj.target=target;
        }
    
    }

    //----------------------------------------------------------------------------------------------------------------------------------

    private void AI(){

        if(stunned&&!stunFlag){
            stunFlag=true;
            Invoke(nameof(ResetStun),info.stunCd);
        }

        if(target){if(Vector3.Distance(transform.position,target.position) > maxChaseDistance) target = null;}
        
        playerInSightRange = Physics.CheckSphere(transform.position, info.sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, info.attackRange, playerMask);

        if(dying||stunned) return;

        if (!target) Patroling();
        if (target) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) Attack();

        if(Physics.CheckSphere(transform.position, info.sightRange, playerMask)) target = Physics.OverlapSphere(transform.position, info.sightRange, playerMask)[0].GetComponent<Transform>();

        if(transform.position.y < -20f || transform.position.y > 50f) Die();

    }

    private void Patroling(){

        if(!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        //Debug.Log(Vector3.Distance(agent.destination,transform.position));
        if(Vector3.Distance(agent.destination,transform.position)<=1.1f){
            anim.SetBool("Walking",false);
        }
        else anim.SetBool("Walking",true);

        //Research if distance = .25f
        Vector3 distanceToWalk = transform.position - walkPoint;
        if(distanceToWalk.magnitude<.25f&&!stunned){
            stunned=true;
        }

    }
    private void ResetStun(){ walkPointSet=false; stunned=false;stunFlag=false;}
    private void SearchWalkPoint(){
        
        float rZ = Random.Range(-info.walkPointRange,info.walkPointRange);
        float rX = Random.Range(-info.walkPointRange,info.walkPointRange);
        
        walkPoint = new Vector3(transform.position.x + rX, transform.position.y, transform.position.z + rZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, groundMask)) walkPointSet=true;
    }

    private void ChasePlayer(){
        if(!canWalk) return;
        agent.speed = info.runSpeed;
        if(target) agent.SetDestination(target.position);

        if(Vector3.Distance(agent.destination,transform.position)>1f){
            anim.SetBool("Running",true);
        }
        else anim.SetBool("Running",false);
    }
    public override void Attack(){

        agent.SetDestination(transform.position);


        //Look at target
        //transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z),Vector3.up);
        if(target) {
            var targetRot = Quaternion.LookRotation(target.position-transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, info.rotationSpeed*Time.deltaTime);
        }

        if(!alreadyAttacked) {

            canWalk = false;

            // int rAnim = Random.Range(0,combo.Count);
            // anim.runtimeAnimatorController=combo[rAnim].animOV;
            // anim.Play("Attack",0,0);
            //Invoke(nameof(ResetAttackAnim),.01f);
            anim.SetBool("Walking",false);
            anim.SetBool("Running",false);
            
            attackSound?.Play();
            warningImage.SetActive(true);
            Invoke(nameof(ToggleTrigger),info.attackCd);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), info.attackSpeed);

        }

    }
    private void ResetAttack(){alreadyAttacked=false;canWalk = true;}
    private void ToggleTrigger(){damageTrigger.SetActive(true); Invoke(nameof(ResetTrigger),.05f);}
    private void ResetTrigger() {warningImage.SetActive(false); damageTrigger.SetActive(false);}


    //----------------------------------------------------------------------------------------------------------------------------------

    public override void TakeDamage(float _damage)
    {
        // if(!target) target = GameManager.Instance.pc.transform;

        if(damageSound){
            damageSound.volume=Random.Range(0.85f,1f);
            damageSound.pitch=Random.Range(1f,1.25f);
            damageSound.Play();
        }

        currentHealth-=_damage;
        if(currentHealth<=0) Die();
    }

    public override void Die()
    {
        if(!dying){
            //Instantiate(photo,new Vector3(transform.position.x,transform.position.y,transform.position.z),Quaternion.identity);
            
            //FX
            deathSound?.Play();
            Destroy(GetComponent<Collider>());
            dying=true;
        }
        
        // if(sm) sm.enemies.Remove(this);

        Destroy(gameObject,3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(walkPoint, .25f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, info.attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, info.sightRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);
    }

}
