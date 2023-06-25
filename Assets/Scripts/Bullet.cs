using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject createOnDestroy;
    public float damage, knockBack, stunTime, lifeTime = 5;
    [SerializeField] bool hurtPlayer, hurtEnemy;

    [Header("Ghost")]
    [SerializeField] GameObject ghostObject;
    [SerializeField] float ghostSpacing;
    float ghostCooldown;

    private void Update()
    {
        if (ghostObject == null) return;

        ghostCooldown -= Time.deltaTime;
        if (ghostCooldown > 0) return;

        ghostCooldown = ghostSpacing;
        Instantiate(ghostObject, transform.position, transform.rotation, transform.parent);
    }

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

        var target = collision.gameObject.GetComponentInParent<TargetScript>();
        if(target != null) target.Hit("bullet");

        if (createOnDestroy != null) Instantiate(createOnDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
