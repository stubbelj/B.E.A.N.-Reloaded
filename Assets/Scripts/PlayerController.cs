using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    PlayerSound pSound => GetComponent<PlayerSound>();
    PlayerAnimator anim => GetComponent<PlayerAnimator>();
    PlayerCombat pCombat => GetComponent<PlayerCombat>();
    PlayerGrapple pGrapple => GetComponent<PlayerGrapple>();

    [SerializeField] float movementSpeed;
    [SerializeField] float airMovementSpeed;
    
    [SerializeField] Vector2 groundCheckBoxDimensions, groundCheckBoxOffset;
    [SerializeField] LayerMask platformLayer;
    
    [Header("StartJump")]
    [SerializeField] float jumpForce;
    [SerializeField][Tooltip("Max amount of time in seconds you can hold the spacebar to get a longer jump")] float jumpMaxTime;
    float jumpTimer;
    [SerializeField] public int maxJumps = 2;
    public int jumpsLeft = 0;
    [SerializeField] public AudioClip jumpSfx;

    [Header("Dash")]
    [SerializeField][Tooltip("Amount of time in seconds a dash lasts")] float dashDuration = 0.3f; 
    [SerializeField] float dashSpeed;
    private float dashTimer; 

    [Header("State bools")]
    public bool isOnGround;
    public bool isJumping = false;
    public bool isDashing = false;
    bool faceMouse = true;
    bool stepping, slamming, busy;

    [Header("limits")]
    [SerializeField] float bulletBoostSpeedMax = 50;


    // Start is called before the first frame update
    void Start(){
        dashTimer = dashDuration;
    }
    // Update is called once per frame
    void Update(){
        if (faceMouse) FaceMouse();

        CheckGround();
        
        if (isDashing && dashTimer > 0) {
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0) {
                isDashing = false;
            }
        }

        if (!slamming && !anim.slamming) {
            if (Input.GetKeyDown(KeyCode.Space)) StartJump();
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) Move(-1);
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) Move(1);
        }
        if(isJumping && Input.GetKey(KeyCode.Space) && jumpTimer > 0){
            ContinueJump();
        }
        if(isJumping && (jumpTimer <= 0 && Input.GetKeyUp(KeyCode.Space))){
            isJumping = false;
        }

        busy = stepping || isDashing || slamming || anim.slamming;
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !busy && !pGrapple.LaunchFromGrapple()) StopMove();

        if(Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !busy){ //Press shift while holding a movement key
            Dash();
        }
    }
    
    public void ContinueJump(){
        rb.velocity = new Vector2(rb.velocity.x, 0); 
        rb.velocity += Vector2.up * jumpForce;
        jumpTimer -= Time.deltaTime;
    }

    public void StartJump(){
        if(isOnGround || jumpsLeft > 0){
            isJumping = true;
            jumpsLeft--;
            pSound.jump.Play();
            rb.velocity = new Vector2(rb.velocity.x, 0); //Reset vertical velocity so the second jump isn't affected by your current velocity
            rb.velocity += Vector2.up * jumpForce;
            jumpTimer = jumpMaxTime;
        }
    }

    void Dash()
    {
        isDashing = true;
        anim.SetDash();
        dashTimer = dashDuration;
        pSound.dash.Play();
    }

    public bool isSlamming()
    {
        return slamming;
    }

    public void RefreshJump(int numJumps)
    {
        jumpsLeft = Mathf.Max(jumpsLeft, numJumps);
    }

    public void BoostUp(float force)
    {
        if (rb.velocity.y < bulletBoostSpeedMax) rb.AddForce(Vector2.up * force);
    }

    public void Step(float xForce, float stepTime)
    {
        rb.AddForce(new Vector2(xForce, 0));
        stepping = true;
        StartCoroutine(EndStep(stepTime));
    }

    IEnumerator EndStep(float time)
    {
        yield return new WaitForSeconds(time);
        stepping = false;
    }

    void FaceMouse()
    {
        if (isDashing) return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
        float yAngle = mousePos.x < transform.position.x ? 180 : 0;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, yAngle, 0);
    }

    public void GroundSlam(float fallSpeed)
    {
        slamming = true;
        rb.velocity = Vector2.down * fallSpeed;
    }

    private void StopMove(){ //Stops the player's *horizontal* movement
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Move(int dir){ //dir = -1 for left, 1 for right 
        if (stepping) return;
        bool moveRight = dir > 0;
        bool faceRight = transform.eulerAngles.y == 0;
        if (faceRight != moveRight && pCombat.IsPunching()) return;
        pGrapple.EndGrappleLaunch();

        float baseSpeed = (isOnGround ? movementSpeed : airMovementSpeed);
        float speed     = (isDashing  ? dashSpeed : baseSpeed);

        if(CheckWallSide(dir)){
            //Debug.Log("Hit Wall");
            StopMove();
        } else {
            rb.velocity = new Vector2(dir * speed, rb.velocity.y);
        }
    }

    private void CheckGround(){
        bool wasOnGround = isOnGround;
        isOnGround = Physics2D.BoxCast(transform.position + (Vector3) groundCheckBoxOffset, groundCheckBoxDimensions, 0f, -transform.up, 0.1f /*distance*/, platformLayer);
        if (!wasOnGround && isOnGround){ //This line triggers when you first touch ground after being off it
            if(!slamming) pSound.Land();
            jumpsLeft = maxJumps; 
        }
        if (isOnGround) slamming = false;
    }

    private bool CheckWallSide(int dir){ //dir = -1 for left, 1 for right
        //Checks if you are touching a wall on the right or left side
        Vector2 checkDir = Vector2.right * dir;
        bool isTouching = Physics2D.BoxCast(transform.position, groundCheckBoxDimensions, 0f, checkDir, 0.1f /*distance*/, platformLayer);
        return isTouching;
    }

    private void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(transform.position + (Vector3)groundCheckBoxOffset, (Vector3)groundCheckBoxDimensions);
    }
}
