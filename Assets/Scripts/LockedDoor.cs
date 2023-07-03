using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] float checkRange = 100;
    
    SpriteRenderer srend;
    BoxCollider2D _collider;

    void Start()
    {
        srend = gameObject.GetComponent<SpriteRenderer>();
        _collider = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (EnemyManager.i.EnemyInRadius(checkRange, transform.position)) {
            Close();
            return;
        }
        Open();
    }

    void Open(){
        _collider.enabled = srend.enabled = false;
    }

    void Close(){
        _collider.enabled = srend.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkRange);
    }
}
