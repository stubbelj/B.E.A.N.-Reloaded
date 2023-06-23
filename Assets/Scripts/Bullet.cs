using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject createOnDestroy;
    [SerializeField] float damage, knockBack, stunTime, lifeTime = 5;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerCombat>() != null) return;

        var enemy = collision.gameObject.GetComponentInParent<BaseEnemy>();
        if (enemy != null) enemy.Hit(damage, (collision.gameObject.transform.position - transform.position).normalized * knockBack, stunTime); 

        if (createOnDestroy != null) Instantiate(createOnDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
