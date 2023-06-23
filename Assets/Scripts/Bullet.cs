using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject createOnDestroy;
    public float damage, knockBack, stunTime, lifeTime = 5;
    [SerializeField] bool hurtPlayer, hurtEnemy;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<HitBox>()) return;
        if (!hurtPlayer && collision.gameObject.GetComponentInParent<PlayerCombat>() != null) return;
        if (!hurtEnemy && collision.gameObject.GetComponentInParent<BaseEnemy>() != null) return;

        var enemy = collision.gameObject.GetComponentInParent<BaseEnemy>();
        if (enemy != null) enemy.Hit(damage, (collision.gameObject.transform.position - transform.position).normalized * knockBack, stunTime);

        var player = collision.gameObject.GetComponentInParent<PlayerCombat>();
        if (player != null) player.Hit(damage);

        if (createOnDestroy != null) Instantiate(createOnDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}