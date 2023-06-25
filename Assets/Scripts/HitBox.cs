using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    float damage, knockBackStrength, stunTime;
    Transform knockBackSource;
    public bool hitting;
    [SerializeField] int hitLimit = 3;
    List<GameObject> alreadyHit = new List<GameObject>();
    Sound hitSound;
    [SerializeField] bool player;
    [HideInInspector] public string attackType;

    public void Setup(float damage, Transform knockBackSource, float knockBackStrength, float stunTime = -1, Sound hitSound = null)
    {
        StartHitting(damage, knockBackSource, knockBackStrength, stunTime, hitSound);
        hitting = false;
    }

    public void StartHitting()
    {
        hitting = true;
    }

    public void StartHitting(float damage, Transform knockBackSource, float knockBackStrength, float stunTime = -1, Sound hitSound = null)
    {
        this.damage = damage;
        this.knockBackStrength = knockBackStrength;
        this.knockBackSource = knockBackSource; 
        this.stunTime = stunTime;
        hitting = true;
        this.hitSound = hitSound;
    }

    public void EndHitting()
    {
        hitting = false;
        alreadyHit.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Check(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Check(collision);
    }

    void Check(Collider2D obj)
    {
        if (!hitting) return;

        float damage = alreadyHit.Count > hitLimit ? this.damage / 4 : this.damage;

        if (obj.GetComponent<TargetScript>()) {
            var target = obj.GetComponent<TargetScript>();
            target.Hit(attackType);
        }

        if (player) {
            var enemy = obj.GetComponentInParent<BaseEnemy>();
            if (!enemy || alreadyHit.Contains(enemy.gameObject)) return;
            alreadyHit.Add(enemy.gameObject);
            enemy.Hit(damage, (enemy.transform.position - knockBackSource.position).normalized * knockBackStrength, stunTime);
        }
        else {
            var p = obj.GetComponentInParent<PlayerCombat>();
            if (!p || alreadyHit.Contains(p.gameObject)) return;
            alreadyHit.Add(p.gameObject);
            p.Hit(damage);
        }

        if (hitSound != null) hitSound.Play();
        
    }
}
