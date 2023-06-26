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
    [SerializeField] Vector2 knockBackLimits = new Vector2(0.2f, 15), knockBacklimitX;

    [Space()]
    [SerializeField] Sprite whiteSprite;
    [SerializeField] float flashWhiteTime = 0.1f;
    Sprite originalSprite;

    [Space()]
    [SerializeField] protected bool debug;
    [SerializeField] protected bool flipToFacePlayer;

    public Sound dieSound;

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
        if (flipToFacePlayer) transform.eulerAngles = new Vector3(0, target.position.x < transform.position.x ? 180 : 0, 0);
    }

    protected virtual void Cooldowns()
    {
        stunTime -= Time.deltaTime;
    }

    protected Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
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
        dieSound = Instantiate(dieSound);
        GameObject.Find("gameManager").GetComponent<GameManager>().enemies.Add(gameObject);
    }

    public virtual void Stun(float stunTime)
    {
        this.stunTime = stunTime;
    }

    public virtual void Hit(float damage, Vector3 knockBack, float stunTime = -1)
    {
        knockBack.y = Mathf.Clamp(knockBack.y, knockBackLimits.x, knockBackLimits.y);
        knockBack.x = Mathf.Clamp(knockBack.x, knockBacklimitX.x, knockBacklimitX.y);
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

    protected virtual void WalkAwayFromPlayer()
    {
        var targetSpeed = GetWalkTowardSpeed() * -1;
        rb.velocity = Vector2.Lerp(rb.velocity, targetSpeed, 0.25f);
    }

    Vector2 GetWalkTowardSpeed()
    {
        Vector2 dir = target.transform.position - transform.position;
        return new Vector2(dir.x > 0 ? walkSpeed : -walkSpeed, rb.velocity.y);
    }

    protected virtual void WalkTowardPlayer()
    {
        var targetSpeed = GetWalkTowardSpeed();
        rb.velocity = Vector2.Lerp(rb.velocity, targetSpeed, 0.25f);
    }

    protected virtual void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public virtual void EndAttack() { }
    public virtual void StartAttck() { }

    protected virtual void Die(bool playerKill = true)
    {
        dieSound.Play();
        
        if (deathFX != null) Instantiate(deathFX, transform.position, Quaternion.identity);
        if (lootTableID != -1 && playerKill) GameManager.i.SpawnLoot(lootTableID, transform.position);
        if (XPAmount > 0 && playerKill) GameManager.i.SpawnXP(XPAmount, transform.position);
        GameObject.Find("gameManager").GetComponent<GameManager>().enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
