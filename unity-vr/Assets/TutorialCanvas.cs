using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialCanvas : MonoBehaviour
{
 

    public void OnSkipButton()
    {
        Debug.Log("[User Action] Patient skipped the tutorial and returned to Main Menu");
        SceneManager.LoadScene("MainMenuScene");
    }
}
