using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    private void Awake() {i = this; }

    [SerializeField] float difficultIncreasePeriod;
    float difficultIncreaseCooldown;
    PlayerCombat pCombat => FindAnyObjectByType<PlayerCombat>();

    public int maxEnemies = 3;

    private void Update()
    {
        if (pCombat.dead) SceneManager.LoadScene(0);

        difficultIncreaseCooldown -= Time.deltaTime;
        if (difficultIncreaseCooldown <= 0) {
            difficultIncreaseCooldown = difficultIncreasePeriod;
            maxEnemies += 1;
        }
    }
}
