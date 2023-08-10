//by TheSuspect
//16.06.2023

using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerController : Damageable
{
    [Header("Photon")]
    public bool onlineMode;
    public PhotonView PV;
    public string _nickName;
    public GameObject[] objsToDestroy;
    public GameObject w_model;
    public PlayerManager playerManager;

    public bool inSafeZone;


    [Header("References")]
    public Transform orientation;
    [HideInInspector] public Rigidbody rb;
    public Camera cam;
    public ParticleSystem sprintParticles;
    public TMP_Text speedText;


    [Header("Sound")]
    public float walkFootstepCd=0.62f;
    public float sprintFootstepCd=0.4f;
    private float currentFootstepCd;
    private float footstepTimer;

    public AudioSource footstepSource;
    public AudioSource landSource;

    [Space]

    [Header("Movement")]
    public float speedLerpSpeed = 3f;
    public float currentSpeed;
    public float walkSpeed = 7f;
    public float crouchSpeed = 3f;
    public float sprintSpeed = 12f;
    public float slideSpeed;

    public float speedIncreaseMult;
    public float slopeIncreaseMult;

    [Space]

    private Vector3 moveDirection;
    public Vector3 moveInput;

    [Space]

    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool extitingSlope;


    [Header("Jumping")]
    public float airMultiplayer = 1f;
    public float groundDrag = 5f;
    public float jumpForce = 4f;
    public float jumpCooldwn = 1f;
    bool readyToJump = true;

    [Space]

    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;


    [Header("Statements")]
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        crouching,
        air
    }

    public bool isSprinting;
    public bool isSliding;
    public bool isCrouching;


    [Header("Input")]

    //InputManager input;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;


    private void Awake()
    {
        SetUp();
    }

    private void Start() {

    }

    public float ditheredTime;

    private void Update()
    {
        if(onlineMode && !PV.IsMine) return;

        //Debug
        if(Input.GetKeyDown(KeyCode.Alpha1)) SceneManager.LoadScene(0);
        else if(Input.GetKeyDown(KeyCode.Alpha2))SceneManager.LoadScene(1);
        ///////

        if(dying) {
            rb.isKinematic = true;
            return;
        }
        StateHandler();
        MyInput();
        SpeedControl();
        GroundCheck();
        FootStep();

        //////////////////////////////////////////////////////
        if(isSliding){

            if(OnSlope() && rb.velocity.y < 0){
                
                ditheredTime+=Time.deltaTime*8;
                // float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                // float slopeAngleIncrease = 1 + (slopeAngle / 90f);
                // Debug.Log(slopeAngle + " " + slopeAngleIncrease);

                currentSpeed = ditheredTime;
            }
            else{
                currentSpeed = Mathf.Lerp(currentSpeed,0,speedLerpSpeed*1.5f*Time.deltaTime);
            }
        }
        if(isSprinting&&!isSliding){
            currentSpeed = Mathf.Lerp(currentSpeed,sprintSpeed,speedLerpSpeed*Time.deltaTime);
        }
        //////////////////////////////////////////////////////

        speedText.text = Mathf.Floor(rb.velocity.magnitude).ToString();

        if(transform.position.y <-25f) {Die();}

        if((isSprinting||isSliding)&&moveDirection!=Vector3.zero) sprintParticles.gameObject.SetActive(true);
        else sprintParticles.gameObject.SetActive(false);

    }

    private void FixedUpdate()
    {
        if(onlineMode && !PV.IsMine) return;

        if(dying) return;
        Movement();
    }

    //--------------------------------------------------------------------------------------------------------------------------------

    #region Movement

    private void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, whatIsGround);

        #region Ground Drag
        if ((state == MovementState.walking || state == MovementState.sprinting) && grounded) rb.drag = groundDrag;
        else rb.drag = 0;
        #endregion

    }

    private void MyInput()
    {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {

            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldwn);
        }

        isSprinting = Input.GetKey(sprintKey);
    }

    private void StateHandler()
    {
        if(isCrouching){
            state = MovementState.crouching;
            currentSpeed = crouchSpeed;
        }

        else if(isSliding){
            state = MovementState.sliding;
        }

        else if (isSprinting)
        {
            state = MovementState.sprinting;
            // currentSpeed = sprintSpeed;
            currentFootstepCd = sprintFootstepCd;
        }

        else if (grounded)
        {
            state = MovementState.walking;
            currentSpeed = walkSpeed;
            currentFootstepCd = walkFootstepCd;
        }

        else
        {
            state = MovementState.air;
            currentSpeed = walkSpeed;
        }
    }

    private void Movement()
    {
        if(isSliding) return;

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (OnSlope() && !extitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * currentSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        //Realistic Air Speed
        if(currentSpeed>0) airMultiplayer = 2 / currentSpeed;

        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplayer, ForceMode.Force);

        rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    {
        if (currentSpeed < 0) currentSpeed = 0;

        if (OnSlope() && !extitingSlope)
        {
            if (rb.velocity.magnitude > currentSpeed)
                rb.velocity = rb.velocity.normalized * currentSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > currentSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * currentSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
    }

    private void Jump()
    {
        extitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        extitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    #endregion

    #region Health
    public void Heal(float _amount){

        currentHealth+=_amount;
        // healthBar.fillAmount = currentHealth/maxHealth;

        if(currentHealth>maxHealth) currentHealth = maxHealth;
    }

    public override void TakeDamage(float _damage)
    {
        if(_damage>=maxHealth) currentHealth=1f;
        else currentHealth-=_damage;

        // healthBar.fillAmount = currentHealth/maxHealth;

        if(currentHealth<=0) Die();
    }

    public override void Die()
    {
        if(!dying){

            if(onlineMode){

                playerManager.Die();

            }else{

                FadeManager.Instance.DyingFade();

                Invoke(nameof(AfterDeath),6f);

                dying=true;
            }
        }

    }

    private void AfterDeath(){
        SceneManager.LoadScene(0);
    }
    #endregion

    private void FootStep(){
        if(moveDirection!=Vector3.zero&&footstepTimer<=0&&grounded){
            footstepTimer=currentFootstepCd;
            PlayRandomSound(footstepSource);
        }
        if(footstepTimer>0)footstepTimer-=Time.deltaTime;
    }

    private void PlayRandomSound(AudioSource source){
        source.volume = Random.Range(.75f,1f);
        source.pitch = Random.Range(1f,1.5f);
        source.Play();
    }

    //--------------------------------------------------------------------------------------------------------------------------------

    private void SetUp()
    {
        
        //input = InputManager.Instance;
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

        currentHealth = maxHealth;

        Time.timeScale = 1f;

        if(onlineMode && !PV.IsMine){
            w_model.layer = LayerMask.NameToLayer("Default");
            foreach(Transform c in w_model.transform) c.gameObject.layer = LayerMask.NameToLayer("Default");

            Destroy(rb);
            foreach(GameObject g in objsToDestroy) Destroy(g);
            return;
        }

        GameManager.Instance.pc.Add(this);

    }

    // [PunRPC]
    // public void RPC_GMSet(){
    // }

}
