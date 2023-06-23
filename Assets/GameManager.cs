using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PlayerCombat pCombat => FindAnyObjectByType<PlayerCombat>();

    private void Update()
    {
        if (pCombat.dead) SceneManager.LoadScene(0);
    }
}
