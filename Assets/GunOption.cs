using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOption : MonoBehaviour
{
    [SerializeField] Gun gun;
    [SerializeField] float range;
    PlayerCombat player;
    [SerializeField] Sound equipSound;

    private void Start()
    {
        equipSound = Instantiate(equipSound);
        player = FindObjectOfType<PlayerCombat>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > range) return;

        if (Input.GetKeyDown(KeyCode.E)) {
            player.GetNewGun(gun, false);
            PlayerPrefs.SetString("gun", gun.displayName);
            equipSound.Play();
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
