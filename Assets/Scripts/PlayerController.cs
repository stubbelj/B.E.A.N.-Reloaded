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
    void Update(){}

    private void OnJump(){
        Debug.Log("Jump!");
        rb.velocity += Vector2.up * jumpForce;
    }

    private void OnMove(){
        Debug.Log("Move!");
    }
}
