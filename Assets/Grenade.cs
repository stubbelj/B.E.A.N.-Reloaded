using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] GameObject explosionFX;
    [SerializeField] float timer;
    [SerializeField] Sound explodeSound;
    float timeLeft;

    private void Start()
    {
        timeLeft = timer;
        explodeSound = Instantiate(explodeSound);
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0) Explode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var enemy = collision.gameObject.GetComponentInParent<BaseEnemy>();
        if (!enemy) return;

        Explode();
    }

    void Explode()
    {
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        explodeSound.Play();
        Destroy(gameObject);
    }
}
