using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    float damage, knockBackStrength, stunTime;
    Transform knockBackSource;
    bool hitting;
    List<BaseEnemy> alreadyHit = new List<BaseEnemy>();
    Sound hitSound;

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

        var enemy = obj.GetComponentInParent<BaseEnemy>();
        if (!enemy || alreadyHit.Contains(enemy)) return;
        alreadyHit.Add(enemy);

        if (hitSound != null) hitSound.Play();
        enemy.Hit(damage, (enemy.transform.position - knockBackSource.position).normalized * knockBackStrength, stunTime);
    }
}