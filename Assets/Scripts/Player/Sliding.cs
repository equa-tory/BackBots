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

        if(Input.GetKeyDown(KeyCode.X) && rb.velocity.magnitude > pc.walkSpeed && (horizontalInput!=0||verticalInput!=0)) StartSlide();
        if(Input.GetKeyUp(KeyCode.X) && pc.isSliding) StopSlide();
    }

    private void FixedUpdate() {
        if(pc.isSliding) SlidingMovement();
    }

    private void StartSlide(){

        pc.isSliding = true;

        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        pc.currentSpeed = pc.slideSpeed;

        slideTimer = maxSlideTime;

        if(lastInput == Vector3.zero) lastInput = orientation.forward * verticalInput + orientation.right * horizontalInput;

    }
    
    private void SlidingMovement(){
        Vector3 inputDir = lastInput;

        // sliding normal   
        if(!pc.OnSlope() || rb.velocity.y > -0.1f){
            
            rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down slope
        else{

            rb.AddForce(pc.GetSlopeMoveDirection(inputDir) * slideForce, ForceMode.Force);
        }


        // rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);

        // if(slideTimer < 0) StopSlide();
    }

    private void StopSlide(){
        pc.isSliding = false;
        lastInput = Vector3.zero;

        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
    }
}
