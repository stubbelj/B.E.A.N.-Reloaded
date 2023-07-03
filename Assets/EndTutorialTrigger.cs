using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorialTrigger : MonoBehaviour
{
    [SerializeField] int sceneNum = 3;
    bool loaded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null && !loaded) {
            loaded = true;
            SceneManager.LoadScene(1);
            SceneManager.LoadScene(sceneNum, LoadSceneMode.Additive);
        }
    }
}
