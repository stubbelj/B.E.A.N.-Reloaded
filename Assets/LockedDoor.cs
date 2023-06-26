using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    GameManager manager;
    [SerializeField] float OOBRange = 100;
    
    SpriteRenderer sr;
    BoxCollider2D collider;
    
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("gameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        sr = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    { 
        //Debug.Log(Vector3.Distance(player.transform.position, gameObject.transform.position));
        foreach(GameObject enemy in manager.enemies){
            if(Vector3.Distance(enemy.transform.position, gameObject.transform.position) < OOBRange){
                Close();
                return;
            }
        }
        Open();
    }

    void Open(){
        //sr.enabled = false;
        collider.enabled = false;

        GetComponent<Animator>().Play("DoorDown");
    }

    void Close(){
        //sr.enabled = true;
        collider.enabled = true;

        GetComponent<Animator>().Play("DoorUp");
    }
}
