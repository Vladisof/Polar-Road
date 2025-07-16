using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostUpgradeManager : MonoBehaviour
{
    [Header("Upgrade Buttons")]
    [SerializeField] private Button decreaseNegativeBoostButton;
    [SerializeField] private Button increasePositiveBoostButton;
    [SerializeField] private Button decreaseSkipTurnButton;
    [SerializeField] private Button increaseMultiplierButton;

    [Header("Upgrade Costs")]
    [SerializeField] private int[] decreaseNegativeBoostCosts = { 500, 1500, 3000, 5000, 8000 };
    [SerializeField] private int[] increasePositiveBoostCosts = { 500, 1500, 3000, 5000, 8000 };
    [SerializeField] private int[] decreaseSkipTurnCosts = { 500, 1500, 3000, 5000, 8000 };
    [SerializeField] private int[] increaseMultiplierCosts = { 500, 1500, 3000, 5000, 8000 };

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI[] upgradeLevelTexts;
    [SerializeField] private TextMeshProUGUI[] upgradeCostTexts;

    [Header("References")]
    [SerializeField] private MoneyController moneyController;
    [SerializeField] private TwoPlayerDiceGame diceGame;

    // Upgrade levels (0-5, where 0 is no upgrade)
    private int decreaseNegativeBoostLevel = 0;
    private int increasePositiveBoostLevel = 0;
    private int decreaseSkipTurnLevel = 0;
    private int increaseMultiplierLevel = 0;

    // Upgrade effects (per level)
    private float negativeBoostReductionPerLevel = 0.03f;
    private float positiveBoostIncreasePerLevel = 0.04f;
    private float skipTurnReductionPerLevel = 0.025f;
    private float multiplierBoostIncreasePerLevel = 0.03f;

    private void Start()
    {
        LoadUpgradeLevels();
        SetupButtons();
        UpdateUITexts();
        ApplyUpgradesToGame();
    }

    private void SetupButtons()
    {
        if (decreaseNegativeBoostButton)
            decreaseNegativeBoostButton.onClick.AddListener(() => UpgradeBoost(0));
        
        if (increasePositiveBoostButton)
            increasePositiveBoostButton.onClick.AddListener(() => UpgradeBoost(1));
        
        if (decreaseSkipTurnButton)
            decreaseSkipTurnButton.onClick.AddListener(() => UpgradeBoost(2));
        
        if (increaseMultiplierButton)
            increaseMultiplierButton.onClick.AddListener(() => UpgradeBoost(3));
    }

    private void UpgradeBoost(int boostType)
    {
        int currentLevel = 0;
        int[] costArray = null;
        
        switch (boostType)
        {
            case 0: // Decrease negative boost chance
                currentLevel = decreaseNegativeBoostLevel;
                costArray = decreaseNegativeBoostCosts;
                break;
            case 1: // Increase positive boost chance
                currentLevel = increasePositiveBoostLevel;
                costArray = increasePositiveBoostCosts;
                break;
            case 2: // Decrease skip turn chance
                currentLevel = decreaseSkipTurnLevel;
                costArray = decreaseSkipTurnCosts;
                break;
            case 3: // Increase multiplier chance
                currentLevel = increaseMultiplierLevel;
                costArray = increaseMultiplierCosts;
                break;
        }

        // Check if max level reached
        if (currentLevel >= 5)
            return;

        // Check if player has enough money
        int cost = costArray[currentLevel];
        if (moneyController.SubtractMoney(cost))
        {
            // Upgrade successful
            switch (boostType)
            {
                case 0:
                    decreaseNegativeBoostLevel++;
                    break;
                case 1:
                    increasePositiveBoostLevel++;
                    break;
                case 2:
                    decreaseSkipTurnLevel++;
                    break;
                case 3:
                    increaseMultiplierLevel++;
                    break;
            }

            SaveUpgradeLevels();
            UpdateUITexts();
            //ApplyUpgradesToGame();
        }
    }

    private void UpdateUITexts()
    {
        if (upgradeLevelTexts.Length >= 4)
        {
            upgradeLevelTexts[0].text = "Lvl: " + decreaseNegativeBoostLevel;
            upgradeLevelTexts[1].text = "Lvl: " + increasePositiveBoostLevel;
            upgradeLevelTexts[2].text = "Lvl: " + decreaseSkipTurnLevel;
            upgradeLevelTexts[3].text = "Lvl: " + increaseMultiplierLevel;
        }

        if (upgradeCostTexts.Length >= 4)
        {
            UpdateCostText(0, decreaseNegativeBoostLevel, decreaseNegativeBoostCosts);
            UpdateCostText(1, increasePositiveBoostLevel, increasePositiveBoostCosts);
            UpdateCostText(2, decreaseSkipTurnLevel, decreaseSkipTurnCosts);
            UpdateCostText(3, increaseMultiplierLevel, increaseMultiplierCosts);
        }
    }

    private void UpdateCostText(int index, int level, int[] costs)
    {
        if (level >= 5)
            upgradeCostTexts[index].text = "MAX";
        else
            upgradeCostTexts[index].text = costs[level].ToString();
    }

    private void ApplyUpgradesToGame()
    {
        if (diceGame != null)
        {
            // Reduce negative boost chance
            float baseNegativeChance = 0.2f; // Base value from TwoPlayerDiceGame
            float newNegativeChance = Mathf.Max(0.05f, baseNegativeChance - (decreaseNegativeBoostLevel * negativeBoostReductionPerLevel));
            //diceGame.SetNegativeBoostChance(newNegativeChance);
            
            // Increase positive boost chance
            float basePositiveChance = 0.3f; // Base value from TwoPlayerDiceGame
            float newPositiveChance = Mathf.Min(0.5f, basePositiveChance + (increasePositiveBoostLevel * positiveBoostIncreasePerLevel));
            //diceGame.SetPositiveBoostChance(newPositiveChance);
            
            // Reduce skip turn chance
            float baseSkipTurnChance = 0.15f; // Base value from TwoPlayerDiceGame
            float newSkipTurnChance = Mathf.Max(0.05f, baseSkipTurnChance - (decreaseSkipTurnLevel * skipTurnReductionPerLevel));
            //diceGame.SetSkipTurnBoostChance(newSkipTurnChance);
            
            // Increase multiplier chance
            float baseMultiplierChance = 0.15f; // Base value from TwoPlayerDiceGame
            float newMultiplierChance = Mathf.Min(0.3f, baseMultiplierChance + (increaseMultiplierLevel * multiplierBoostIncreasePerLevel));
            //diceGame.SetMultiplierBoostChance(newMultiplierChance);
        }
    }

    private void SaveUpgradeLevels()
    {
        PlayerPrefs.SetInt("DecreaseNegativeBoostLevel", decreaseNegativeBoostLevel);
        PlayerPrefs.SetInt("IncreasePositiveBoostLevel", increasePositiveBoostLevel);
        PlayerPrefs.SetInt("DecreaseSkipTurnLevel", decreaseSkipTurnLevel);
        PlayerPrefs.SetInt("IncreaseMultiplierLevel", increaseMultiplierLevel);
        PlayerPrefs.Save();
    }

    private void LoadUpgradeLevels()
    {
        decreaseNegativeBoostLevel = PlayerPrefs.GetInt("DecreaseNegativeBoostLevel", 0);
        increasePositiveBoostLevel = PlayerPrefs.GetInt("IncreasePositiveBoostLevel", 0);
        decreaseSkipTurnLevel = PlayerPrefs.GetInt("DecreaseSkipTurnLevel", 0);
        increaseMultiplierLevel = PlayerPrefs.GetInt("IncreaseMultiplierLevel", 0);
    }
}