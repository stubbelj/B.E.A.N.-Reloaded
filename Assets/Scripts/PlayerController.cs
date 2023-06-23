using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();


    [SerializeField] float jumpForce;
    [SerializeField] float movementSpeed;
    [SerializeField] float airMovementSpeed;
    
    float groundCheckBoxYOffset = -0.06f;
    [SerializeField] Vector2 groundCheckBoxDimensions;
    [SerializeField] LayerMask platformLayer;
    

    public int maxJumps = 2;
    public int jumpsLeft = 0;

    public bool isOnGround;

    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update(){
        CheckGround();
        
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) Move(-1);
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) Move(1);

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) StopMove();
    }

    public void Jump(){
        if(isOnGround || jumpsLeft > 0){
            jumpsLeft--;
            rb.velocity *= Vector2.up * 0; //Reset vertical velocity so the second jump isn't affected by your current velocity
            rb.velocity += Vector2.up * jumpForce;
        }
    }

    private void StopMove(){ //Stops the player's *horizontal* movement
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Move(int dir){ //dir = -1 for left, 1 for right 
        Debug.Log("Move! " + dir);

        float speed = (isOnGround ? movementSpeed : airMovementSpeed);
        
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
