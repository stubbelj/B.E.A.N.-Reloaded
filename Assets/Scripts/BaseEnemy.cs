using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    [SerializeField] float maxHealth, health;
    [SerializeField] Slider hpSlider;

    [SerializeField] float walkSpeed, defaultStunTime = 0.6f, knockBackYMin = 0.2f;

    protected float stunTime;
    protected Transform target;
    protected Rigidbody2D rb => GetComponent<Rigidbody2D>();
    protected float dist;

    protected virtual void Update()
    {
        dist = Vector2.Distance(transform.position, target.position);
        stunTime -= Time.deltaTime;
        if (hpSlider != null) hpSlider.value = health / maxHealth;
    }

    private void Start()
    {
        health = maxHealth;
        target = FindObjectOfType<PlayerCombat>().transform;
    }

    public virtual void Hit(float damage, Vector3 knockBack, float stunTime = -1)
    {
        knockBack.y = Mathf.Max(knockBack.y, knockBackYMin);
        if (stunTime == -1) stunTime = defaultStunTime;
        Stop();

        health -= damage;
        if (health <= 0) Die();

        this.stunTime = stunTime;
        rb.AddForce(knockBack);
    }

    protected void WalkTowardPlayer()
    {
        Vector2 dir = target.transform.position - transform.position;
        Vector2 targetSpeed = new Vector2(dir.x > 0 ? walkSpeed : -walkSpeed, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, targetSpeed, 0.25f);
    }

    protected void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
