using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();

    [SerializeField] float jumpForce;
    [SerializeField] float movementSpeed;
    [SerializeField] float airMovementSpeed;
    [SerializeField] float dashSpeed;
    
    private float groundCheckBoxYOffset = -0.06f;
    [SerializeField] Vector2 groundCheckBoxDimensions;
    [SerializeField] LayerMask platformLayer;
    
    [SerializeField] public int maxJumps = 2;
    public int jumpsLeft = 0;

    public bool isOnGround;
    public bool isDashing = false;

    [SerializeField][Tooltip("Amount of time in seconds a dash lasts")] float dashDuration = 0.3f; 
    private float dashTimer; 

    [SerializeField] public AudioClip jumpSfx;

    // Start is called before the first frame update
    void Start(){
        dashTimer = dashDuration;
    }
    // Update is called once per frame
    void Update(){
        CheckGround();
        
        //print(dashTimer);
        if (isDashing && dashTimer > 0) {
            dashTimer -= Time.deltaTime;
            if(dashTimer <= 0) {
                //print("done");
                isDashing = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) Move(-1);
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) Move(1);

        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !isDashing) StopMove();

        if(Input.GetKeyDown(KeyCode.LeftShift) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !isDashing){ //Press shift while holding a movement key
            isDashing = true;
            dashTimer = dashDuration;
        }
    }

    public void Jump(){
        if(isOnGround || jumpsLeft > 0){
            jumpsLeft--;
            SoundManager.instance.PlaySound(jumpSfx);
            rb.velocity *= Vector2.up * 0; //Reset vertical velocity so the second jump isn't affected by your current velocity
            rb.velocity += Vector2.up * jumpForce;
        }
    }

    private void StopMove(){ //Stops the player's *horizontal* movement
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Move(int dir){ //dir = -1 for left, 1 for right 
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
        isOnGround = Physics2D.BoxCast(transform.position, groundCheckBoxDimensions, 0f, -transform.up, 0.1f /*distance*/, platformLayer);
        if (!wasOnGround && isOnGround) jumpsLeft = maxJumps; //This line triggers when you first touch ground after being off it, and resets your jumps
    }

    private bool CheckWallSide(int dir){ //dir = -1 for left, 1 for right
        //Checks if you are touching a wall on the right or left side
        Vector2 checkDir = Vector2.right * dir;
        bool isTouching = Physics2D.BoxCast(transform.position, groundCheckBoxDimensions, 0f, checkDir, 0.1f /*distance*/, platformLayer);
        return isTouching;
    }

    private void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(new Vector2(transform.position.x, transform.position.y + groundCheckBoxYOffset), (Vector3)groundCheckBoxDimensions);
    }
}
