using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNicknamesManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField player1InputField;
    [SerializeField] private TMP_InputField player2InputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private string defaultPlayer1Name = "Player 1";
    [SerializeField] private string defaultPlayer2Name = "Player 2";

    private void Start()
    {
        // Load saved nicknames if they exist
        string savedPlayer1Name = PlayerPrefs.GetString("Player1Nickname", defaultPlayer1Name);
        string savedPlayer2Name = PlayerPrefs.GetString("Player2Nickname", defaultPlayer2Name);

        // Set input field text to saved values
        if (player1InputField != null)
            player1InputField.text = savedPlayer1Name;
        
        if (player2InputField != null)
            player2InputField.text = savedPlayer2Name;

        // Add listener to confirm button
        if (confirmButton != null)
            confirmButton.onClick.AddListener(SavePlayerNicknames);
    }

    public void SavePlayerNicknames()
    {
        // Get nicknames from input fields
        string player1Name = string.IsNullOrEmpty(player1InputField.text) 
            ? defaultPlayer1Name 
            : player1InputField.text;
        
        string player2Name = string.IsNullOrEmpty(player2InputField.text) 
            ? defaultPlayer2Name 
            : player2InputField.text;

        // Save to PlayerPrefs
        PlayerPrefs.SetString("Player1Nickname", player1Name);
        PlayerPrefs.SetString("Player2Nickname", player2Name);
        PlayerPrefs.Save();

        Debug.Log($"Saved player nicknames: {player1Name} and {player2Name}");
    }
}