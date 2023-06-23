using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();

    [SerializeField] float jumpForce;

    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    
    }

    public void Jump(){
        print("AH");

        Debug.Log("Jump!");
        rb.velocity += Vector2.up * jumpForce;
    }

    private void OnMove(){
        Debug.Log("Move!");
    }

    
}
