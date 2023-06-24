using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class BaseEnemy : MonoBehaviour
{
    [SerializeField] float maxHealth, health;
    [SerializeField] Slider hpSlider;
    [SerializeField] GameObject deathFX;
    [SerializeField] int lootTableID = -1;
    [SerializeField] float XPAmount = 0;

    [Space()]
    [SerializeField] protected float walkSpeed;
    [SerializeField] float defaultStunTime = 0.6f, stunTimeResist;
    [SerializeField] Vector2 knockBackLimits = new Vector2(0.2f, 15);

    [Space()]
    [SerializeField] Sprite whiteSprite;
    [SerializeField] float flashWhiteTime = 0.1f;
    Sprite originalSprite;

    [Space()]
    [SerializeField] protected bool debug;
    [SerializeField] protected bool flipToFacePlayer;

    protected Rigidbody2D rb => GetComponent<Rigidbody2D>();
    protected SpriteRenderer srend => GetComponent<SpriteRenderer>();
    protected float stunTime;
    protected Transform target;
    protected float dist;

    protected virtual void OnValidate()
    {
        health = maxHealth;
    }

    protected virtual void Update()
    {
        dist = Vector2.Distance(transform.position, target.position);
        Cooldowns();

        if (hpSlider != null) hpSlider.value = health / maxHealth;
        if (flipToFacePlayer) srend.flipX = target.position.x < transform.position.x;
    }

    protected virtual void Cooldowns()
    {
        stunTime -= Time.deltaTime;
    }

    protected bool LineOfSightToTarget(float range)
    {
        int layerMask = 1 << gameObject.layer;
        layerMask = ~layerMask;

        var dir = target.position - transform.position;
        var hit = Physics2D.Raycast(transform.position, dir, range, layerMask:layerMask);

        return hit.collider != null;
    }

    protected virtual void Start()
    {
        health = maxHealth;
        target = FindObjectOfType<PlayerCombat>().transform;
    }

    public virtual void Stun(float stunTime)
    {
        this.stunTime = stunTime;
    }

    public virtual void Hit(float damage, Vector3 knockBack, float stunTime = -1)
    {
        knockBack.y = Mathf.Clamp(knockBack.y, knockBackLimits.x, knockBackLimits.y);
        if (stunTime == -1) stunTime = defaultStunTime;
        stunTime -= stunTimeResist;
        Stop();

        health -= damage;
        if (health <= 0) Die();

        this.stunTime = stunTime;
        rb.AddForce(knockBack);


        if (whiteSprite != null) StartCoroutine(flashWhite(flashWhiteTime));
    }

    protected virtual IEnumerator flashWhite(float time)
    {
        var anim = GetComponent<Animator>();
        if (anim) anim.enabled = false;

        if (originalSprite == null) originalSprite = srend.sprite;
        srend.sprite = whiteSprite;

        yield return new WaitForSeconds(time);

        srend.sprite = originalSprite;
        if (anim) anim.enabled = true;
    }

    protected void WalkAwayFromPlayer()
    {
        var targetSpeed = GetWalkTowardSpeed() * -1;
        rb.velocity = Vector2.Lerp(rb.velocity, targetSpeed, 0.25f);
    }

    Vector2 GetWalkTowardSpeed()
    {
        Vector2 dir = target.transform.position - transform.position;
        return new Vector2(dir.x > 0 ? walkSpeed : -walkSpeed, rb.velocity.y);
    }

    protected void WalkTowardPlayer()
    {
        var targetSpeed = GetWalkTowardSpeed();
        rb.velocity = Vector2.Lerp(rb.velocity, targetSpeed, 0.25f);
    }

    protected void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public virtual void EndAttack() { }
    public virtual void StartAttck() { }

    protected virtual void Die()
    {
        if (deathFX != null) Instantiate(deathFX, transform.position, Quaternion.identity);
        if (lootTableID != -1) GameManager.i.SpawnLoot(lootTableID, transform.position);
        if (XPAmount > 0) GameManager.i.SpawnXP(XPAmount, transform.position);
        Destroy(gameObject);
    }
}
