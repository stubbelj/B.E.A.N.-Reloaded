using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] int PlaySceneID, PersistantSceneID;

    public void Play(){
        SceneManager.LoadScene(PlaySceneID);
        SceneManager.LoadScene(PersistantSceneID, LoadSceneMode.Additive);
    }

    public void Quit(){
        Application.Quit();
    }

}
