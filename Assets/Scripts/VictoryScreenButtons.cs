using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreenButtons : MonoBehaviour
{
    [SerializeField] int MainMenuID;

    public void ReturnToMenu(){
        SceneManager.LoadScene(MainMenuID);
    }

    public void Quit(){
        Application.Quit();
    }
}
