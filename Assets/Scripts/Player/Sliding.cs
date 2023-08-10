using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    private Rigidbody rb;
    private PlayerController pc;

    [Header("Sliding")]
    public float maxSlideTime;
    private float slideTimer;
    public float slideForce;
    private float currentForce;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    private Vector3 lastInput;


    private void Start() {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();

        player = transform;

        startYScale = player.localScale.y;
    }

    private void Update() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(KeyCode.X)){
            
            if(rb.velocity.magnitude > pc.walkSpeed && (horizontalInput!=0||verticalInput!=0)&&!pc.isCrouching) StartSlide();
            else StartCrouching();            
        } 
        if(Input.GetKeyUp(KeyCode.X) && pc.isSliding) StopSlide();
        
        if(!Input.GetKey(KeyCode.X) && pc.isCrouching){
            if(!Physics.Raycast(transform.position,transform.up,3)) StopCrouching();
        }

        Debug.Log(!Physics.Raycast(transform.position,Vector3.up,startYScale/2));
        // Debug.DrawRay(transform.position,,Color.red);
        
    }

    private void FixedUpdate() {
        if(pc.isSliding) SlidingMovement();
    }

    private void StartSlide(){

        pc.isSliding = true;

        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        if(pc.OnSlope()) pc.ditheredTime = pc.currentSpeed;
        else pc.currentSpeed = pc.slideSpeed;

        currentForce = slideForce;

        slideTimer = maxSlideTime;

        if(lastInput == Vector3.zero) lastInput = orientation.forward * verticalInput + orientation.right * horizontalInput;

    }
    
    private void SlidingMovement(){
        Vector3 inputDir = lastInput;

        // sliding normal   
        if(!pc.OnSlope() || rb.velocity.y > -0.1f){
            
            rb.AddForce(inputDir.normalized * currentForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down slope
        else{

            rb.AddForce(pc.GetSlopeMoveDirection(inputDir) * currentForce, ForceMode.Force);
        }

        currentForce = Mathf.Lerp(currentForce,0,Time.deltaTime*4);

        if(rb.velocity.magnitude<=3) {
            StopSlide();
            StartCrouching();
        }

        // rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);

        // if(slideTimer < 0) StopSlide();
    }

    private void StopSlide(){
        pc.isSliding = false;
        lastInput = Vector3.zero;

        if(!Physics.Raycast(transform.position,transform.up,3)) StartCrouching();
        else player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
    }

    //-----------------------------------------------------------------------------------------------

    private void StartCrouching(){

        pc.isCrouching=true;
        rb.velocity = Vector3.zero;
        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

    }

    private void StopCrouching(){
        
        pc.isCrouching = false;

        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
    }

    private void OnDrawGizmos() {
        Gizmos.color=Color.red;
        Gizmos.DrawLine(transform.position,transform.up*(startYScale/2));
    }
}
