using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class UiManagerScript : MonoBehaviour
{
    [Header("Pc Canvases")]
    [SerializeField] GameObject LevelDiffCanvas;
    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject PhasesCanvas;

    [Header("Vr Canvases")]
    [SerializeField] GameObject Lobby_Vr_Canvas;
    [SerializeField] GameObject Level_Diff_Vr_Canvas;

    [Header("Data")]
    [SerializeField] LevelDiffData LevelDiffcultyData;
    [SerializeField] PlayerDataScript Player_Data;

    [Header("VR Input Fields")]
    [SerializeField] private TMP_InputField nameInputField_VR;
    [SerializeField] private TMP_InputField ageInputField_VR;

    [Header("Private Vals")]
    private bool is_on_Vr;

    private void Start()
    {
        if(FindObjectOfType<PlayerMovement_Vr>() != null)
        {
            is_on_Vr = true;
            LobbyCanvas.SetActive(false);
            Lobby_Vr_Canvas.SetActive(true);
        }
        else
        {
            is_on_Vr = false;
            LobbyCanvas.SetActive(true) ;
            Lobby_Vr_Canvas.SetActive(false);
        }
    }

    #region Lobby Buttons

    public void OnStartButton()
    {
        if (!is_on_Vr)
        {
            LevelDiffCanvas.SetActive(true);
            LobbyCanvas.SetActive(false);
        }
        else
        {
            Level_Diff_Vr_Canvas.SetActive(true);
            Lobby_Vr_Canvas.SetActive(false) ;
        }
    }

    public void OnQuitButton()
    {
        // Tagged as [Dev] because quitting from the main menu doesn't impact an active clinical session
        Debug.Log("[Dev] Quitting Application from Main Menu");
        Application.Quit();
    }

    #endregion

    #region Level Diffuclty Buttons

    public void OnEasyButton()
    {
        LevelDiffcultyData.currentLevelDiff = LevelDiff.Easy;
        LevelDiffCanvas.SetActive(false);
        SavePlayerData();
        LoadScene("FlightScene");
    }

    public void OnMediumButton()
    {
        LevelDiffcultyData.currentLevelDiff = LevelDiff.Medium;
        LevelDiffCanvas.SetActive(false);
        SavePlayerData();
        LoadScene("FlightScene");
    }

    public void OnHardButton()
    {
        LevelDiffcultyData.currentLevelDiff = LevelDiff.Hard;
        LevelDiffCanvas.SetActive(false);
        SavePlayerData();
        LoadScene("FlightScene");
    }
    
    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    } 

    #endregion

    #region Data
   
    private void SavePlayerData()
    {
        string name = nameInputField_VR.text;
        string ageText = ageInputField_VR.text;

        int age;
        bool isAgeValid = int.TryParse(ageText, out age) && CheckAge(age);
        bool isNameValid = CheckName(name);

        if (isAgeValid && isNameValid)
        {
            Player_Data.Player_Name = name;
            Player_Data.Player_Age = age;
            
            // Tagged as [Dev] - patient details are handled safely; this log is just for the developer
            Debug.Log("[Dev] Player data saved!");
        }
        else
        {
            // Tagged as [Dev]
            Debug.LogWarning("[Dev] Invalid input: " +
                             (isNameValid ? "" : "Name ") +
                             (!isAgeValid ? "Age" : ""));
        }
    }

    private bool CheckAge(int age)
    {
        if(age < 0)
            return false;
        return true;
    }

    private bool CheckName(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }

    #endregion
}