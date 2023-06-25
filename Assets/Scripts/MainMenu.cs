using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] int PlaySceneID, PersistantSceneID, TutorialSceneID;

    public void Play(){
        StartCoroutine(DelayedPlay());
    }

    public IEnumerator DelayedPlay(){
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(PlaySceneID);
        SceneManager.LoadScene(PersistantSceneID, LoadSceneMode.Additive);
    }

    public void Tutorial(){
        SceneManager.LoadScene(TutorialSceneID);
        SceneManager.LoadScene(PersistantSceneID, LoadSceneMode.Additive); 
    }

    public void Quit(){
        Application.Quit();
    }

}
