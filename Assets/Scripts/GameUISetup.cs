using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUISetup : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button twoPlayerButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    [Header("Game UI")]
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button returnToMenuButton;
    
    [Header("Game Controllers")]
    [SerializeField] private DiceRollAndMovement singlePlayerController;
    [SerializeField] private TwoPlayerDiceGame twoPlayerController;
    
    private void Start()
    {
        // Start with main menu active and game UI inactive
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        if (gameUIPanel != null)
            gameUIPanel.SetActive(false);
            
        // Disable game controllers initially
        if (singlePlayerController != null)
            singlePlayerController.enabled = false;
            
        if (twoPlayerController != null)
            twoPlayerController.enabled = false;
            
        // Setup button listeners
        SetupButtonListeners();
    }
    
    private void SetupButtonListeners()
    {
        if (singlePlayerButton != null)
            singlePlayerButton.onClick.AddListener(StartSinglePlayerGame);
            
        if (twoPlayerButton != null)
            twoPlayerButton.onClick.AddListener(StartTwoPlayerGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
            
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
            
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }
    
    private void StartSinglePlayerGame()
    {
        mainMenuPanel.SetActive(false);
        gameUIPanel.SetActive(true);
        
        if (gameModeText != null)
            gameModeText.text = "Одиночная игра";
            
        if (singlePlayerController != null)
            singlePlayerController.enabled = true;
            
        // Disable two player controller
        if (twoPlayerController != null)
            twoPlayerController.enabled = false;
    }
    
    private void StartTwoPlayerGame()
    {
        mainMenuPanel.SetActive(false);
        gameUIPanel.SetActive(true);
        
        if (gameModeText != null)
            gameModeText.text = "Игра для двоих";
            
        if (twoPlayerController != null)
            twoPlayerController.enabled = true;
            
        // Disable single player controller
        if (singlePlayerController != null)
            singlePlayerController.enabled = false;
    }
    
    private void OpenSettings()
    {
        // Implement settings panel logic
        Debug.Log("Settings button pressed");
    }
    
    private void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void PauseGame()
    {
        // Implement pause logic
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = Time.timeScale == 0 ? "Resume" : "Pause";
    }
    
    private void ReturnToMainMenu()
    {
        // Ensure time is normal if returning from pause
        Time.timeScale = 1;
        
        mainMenuPanel.SetActive(true);
        gameUIPanel.SetActive(false);
        
        // Disable game controllers
        if (singlePlayerController != null)
            singlePlayerController.enabled = false;
            
        if (twoPlayerController != null)
            twoPlayerController.enabled = false;
    }
}
