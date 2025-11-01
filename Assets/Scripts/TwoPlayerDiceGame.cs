using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TwoPlayerDiceGame : MonoBehaviour
{
    [Header("Dice UI")] [SerializeField]
    private TextMeshProUGUI diceResultText; // Текст для отображения числа на кубике
    [SerializeField] private AudioManager audioManager; // Ссылка на AudioManager

    [SerializeField] private Button rollButton; // Кнопка для броска кубика
    
    [Header("Click Movement")]
    [SerializeField] private Material interactableMaterial;
    [SerializeField] private Material highlightMaterial;
    
    [Header("Step Color System")]
    [SerializeField] private Color step1Color = Color.red;      // Индекс 1 - красный
    [SerializeField] private Color step2Color = Color.yellow;   // Индекс 2 - желтый
    [SerializeField] private Color step3Color = Color.green;    // Индекс 3 - зеленый
    [SerializeField] private Color normalSpotColor = Color.white; // Обычный цвет точек
    
    [Header("Stage System")]
    [SerializeField] private int positionsPerStage = 6;
    [SerializeField] private float playerHeightOffset = 1f;
    
    private int currentDiceResult = 0;
    private bool waitingForPlayerMove = false;
    private InteractiveSpot[] interactiveSpots;

    [Header("Players Setup")] [SerializeField]
    private Transform player1; // Трансформ первого игрока

    [SerializeField] private Transform player2; // Трансформ второго игрока
    [SerializeField] private Transform[] boardSpots; // Массив существующих точек на доске
    [SerializeField] private float moveSpeed = 5f; // Скорость перемещения игроков
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота игроков

    [Header("Player Info UI")] [SerializeField]
    private TextMeshProUGUI player1PositionText; // Текст позиции первого игрока

    [SerializeField] private GameObject winPL;
    [SerializeField] private TextMeshProUGUI player2PositionText; // Текст позиции второго игрока
    [SerializeField] private TextMeshProUGUI currentPlayerText; // Текст текущего игрока
    [SerializeField] private TextMeshProUGUI winText; // Текст для отображения множителя игрока 1
    [SerializeField] private TextMeshProUGUI DiceText; // Текст для отображения победителя

    [Header("Dice Animation Settings")] [SerializeField]
    private float diceAnimationDuration = 2f; // Длительность анимации кубика

    [SerializeField] private float numberChangeInterval = 0.1f; // Интервал смены чисел во время анимации
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // Виртуальная камера для анимации

    [Header("Player Animations")] [SerializeField]
    private Animator player1Animator; // Animator component for player 1

    [SerializeField] private Animator player2Animator; // Animator component for player 2

    [Header("Betting System")] [SerializeField]
    private Button startGameButton;

    [SerializeField] private Button increaseBetButton;
    [SerializeField] private Button decreaseBetButton;
    [SerializeField] private TextMeshProUGUI currentBetText;
    [SerializeField] private int minimumBet = 5;
    [SerializeField] private int betIncrement = 5;

    private string player1Name;
    private string player2Name;
    private int currentBet;
    private bool gameStarted = false;
    private MoneyController moneyController;

    [Header("Boost Settings")] [SerializeField]
    private GameObject boostEffectPrefab; // Префаб визуального эффекта буста

    [SerializeField] private GameObject negativeBoostPrefab; // Префаб визуального эффекта негативного буста
    [SerializeField] private GameObject skipTurnBoostPrefab; // Префаб визуального эффекта буста пропуска хода
    [SerializeField] private GameObject multiplierBoostPrefab; // Префаб для буста множителя

    [SerializeField]
    private GameObject skipTurnBoostDisappearEffectPrefab; // Префаб эффекта исчезновения буста пропуска хода

    [SerializeField]
    private GameObject negativeBoostDisappearEffectPrefab; // Префаб эффекта исчезновения негативного буста

    [Range(0f, 1f)] private float boostSpawnChance = 0.3f; // Шанс появления буста на этапе
    [Range(0f, 1f)] private float negativeBoostSpawnChance = 0.2f; // Шанс появления негативного буста
    [Range(0f, 1f)] private float multiplierBoostSpawnChance = 0.15f; // Шанс появления буста множителя
    private int maxBoosts = 15; // Максимальное количество бустов на карте
    private int maxNegativeBoosts = 7; // Максимальное количество негативных бустов
    //private int maxSkipTurnBoosts = 5; // Максимальное количество бустов пропуска хода
    private int maxMultiplierBoosts = 8; // Максимальное количество бустов множителя

    private int player1SpotIndex = 0; // Текущая позиция первого игрока
    private int player2SpotIndex = 0; // Текущая позиция второго игрока
    private int player1CurrentStage = 1; // Текущий этап первого игрока
    private int player2CurrentStage = 1; // Текущий этап второго игрока
    private int player1WinMultiplier = 2; // Начальный множитель для игрока 1
    private int player2WinMultiplier = 2; // Начальный множитель для игрока 2
    private bool isPlayer1Turn = true; // Флаг для отслеживания чей сейчас ход
    private bool isMoving = false; // Флаг для отслеживания движения игрока

    private bool isRolling = false; // Флаг для отслеживания броска кубика
    private bool isWinning = false; // Флаг для отслеживания победы

    // Replace single skipTurnBoostSpot with lists for each player
    private List<int> player1SkipTurnSpots = new List<int>();
    private List<int> player2SkipTurnSpots = new List<int>();

    private enum BoostType
    {
        None,
        Positive,
        Negative,
        SkipTurn,
        Multiplier
    }

    private BoostType[] spotBoostTypes; // Массив для отслеживания типов бустов
    private List<GameObject> boostEffects = new List<GameObject>(); // Список созданных эффектов бустов
    private int winPosition = 0; // Позиция для победы (по умолчанию - полный круг)
    private bool player1SkipNextTurn = false;
    private bool player2SkipNextTurn = false;
    private int skipTurnBoostSpot = -1; // Track which spot has the skip turn boost that was activated

    private void Start()
    {
        // Инициализация массива бустов
        spotBoostTypes = new BoostType[boardSpots.Length];
        player1Name = PlayerPrefs.GetString("Player1Nickname", "Игрок 1");
        player2Name = PlayerPrefs.GetString("Player2Nickname", "Игрок 2");
        // Настройка позиции для победы (конец доски)
        winPosition = boardSpots.Length - 1;

        moneyController = FindObjectOfType<MoneyController>();
        if (moneyController == null)
        {
            Debug.LogError("MoneyController not found in scene!");
        }

        SetupInteractiveSpots();

        currentBet = minimumBet;
        UpdateBetText();

        // Set up betting UI buttons
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Start Game button is not assigned!");
        }

        if (increaseBetButton != null)
        {
            increaseBetButton.onClick.AddListener(IncreaseBet);
        }
        else
        {
            Debug.LogError("Increase Bet button is not assigned!");
        }

        if (decreaseBetButton != null)
        {
            decreaseBetButton.onClick.AddListener(DecreaseBet);
        }
        else
        {
            Debug.LogError("Decrease Bet button is not assigned!");
        }

        // Disable roll button until game starts
        if (rollButton != null)
        {
            rollButton.interactable = false;
        }

        // Setup board components but don't start game yet
        SetupBoardComponents();

        // Инициализация начального положения игроков
        if (boardSpots.Length > 0)
        {
            if (player1 != null)
                player1.position = boardSpots[0].position + Vector3.up * playerHeightOffset;
            else
                Debug.LogError("Player 1 is not assigned!");

            if (player2 != null)
                player2.position = boardSpots[0].position + Vector3.up * playerHeightOffset;
            else
                Debug.LogError("Player 2 is not assigned!");
        }
        else
        {
            Debug.LogError("BoardSpots array is empty!");
        }

        // Добавление обработчика на кнопку броска
        if (rollButton != null)
        {
            rollButton.onClick.AddListener(RollDice);
        }
        else
        {
            Debug.LogError("Roll button is not assigned!");
        }

        // Инициализация текстовых полей
        if (diceResultText != null)
        {
            diceResultText.text = "";
        }

        LoadBoostUpgrades();
        spotBoostTypes = new BoostType[boardSpots.Length];
        // Обновление UI
        UpdateUI();
    }

    private void SetupInteractiveSpots()
    {
        interactiveSpots = new InteractiveSpot[boardSpots.Length];

        for (int i = 0; i < boardSpots.Length; i++)
        {
            // Check for collider
            Collider collider = boardSpots[i].GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogWarning($"Spot {i} doesn't have a Collider! Adding BoxCollider.");
                boardSpots[i].gameObject.AddComponent<BoxCollider>();
            }

            InteractiveSpot spot = boardSpots[i].GetComponent<InteractiveSpot>();
            if (spot == null)
            {
                spot = boardSpots[i].gameObject.AddComponent<InteractiveSpot>();
            }

            // Initialize the spot with its index
            spot.Initialize(i);
            interactiveSpots[i] = spot;

            // Assign materials
            if (interactableMaterial != null)
                spot.normalMaterial = interactableMaterial;
            if (highlightMaterial != null)
                spot.highlightMaterial = highlightMaterial;

            // Initially all spots are non-interactive
            spot.SetInteractable(false);
        }
    }

    // Add these methods to TwoPlayerDiceGame.cs
    public void SetPositiveBoostChance(float chance)
    {
        boostSpawnChance = Mathf.Clamp01(chance);
    }

    public void SetNegativeBoostChance(float chance)
    {
        negativeBoostSpawnChance = Mathf.Clamp01(chance);
    }

    public void SetMultiplierBoostChance(float chance)
    {
        multiplierBoostSpawnChance = Mathf.Clamp01(chance);
    }

    // Add this method to load boost upgrades from PlayerPrefs
    private void LoadBoostUpgrades()
    {
        int decreaseNegativeBoostLevel = PlayerPrefs.GetInt("DecreaseNegativeBoostLevel", 0);
        int increasePositiveBoostLevel = PlayerPrefs.GetInt("IncreasePositiveBoostLevel", 0);
        int decreaseSkipTurnLevel = PlayerPrefs.GetInt("DecreaseSkipTurnLevel", 0);
        int increaseMultiplierLevel = PlayerPrefs.GetInt("IncreaseMultiplierLevel", 0);

        // Apply the loaded values
        float negativeBoostReductionPerLevel = 0.03f;
        float positiveBoostIncreasePerLevel = 0.04f;
        float skipTurnReductionPerLevel = 0.025f;
        float multiplierBoostIncreasePerLevel = 0.03f;

        // Base values
        float baseNegativeChance = 0.2f;
        float basePositiveChance = 0.3f;
        float baseSkipTurnChance = 0.15f;
        float baseMultiplierChance = 0.15f;

        // Calculate and apply the new values
        negativeBoostSpawnChance = Mathf.Max(0.05f,
            baseNegativeChance - (decreaseNegativeBoostLevel * negativeBoostReductionPerLevel));
        boostSpawnChance = Mathf.Min(0.5f,
            basePositiveChance + (increasePositiveBoostLevel * positiveBoostIncreasePerLevel));
        multiplierBoostSpawnChance = Mathf.Min(0.3f,
            baseMultiplierChance + (increaseMultiplierLevel * multiplierBoostIncreasePerLevel));
        Debug.Log("Boost spawn chance: " + boostSpawnChance);
    }

    private void SetupBoardComponents()
    {
        // Инициализация массива бустов
        spotBoostTypes = new BoostType[boardSpots.Length];

        // Настройка позиции для победы (конец доски)
        winPosition = boardSpots.Length - 1;

        // Инициализация начального положения игроков
        if (boardSpots.Length > 0)
        {
            if (player1 != null)
                player1.position = boardSpots[0].position + Vector3.up * playerHeightOffset;
            else
                Debug.LogError("Player 1 is not assigned!");

            if (player2 != null)
                player2.position = boardSpots[0].position + Vector3.up * playerHeightOffset;
            else
                Debug.LogError("Player 2 is not assigned!");
        }
        else
        {
            Debug.LogError("BoardSpots array is empty!");
        }

        // Инициализация текстовых полей
        if (diceResultText != null)
        {
            diceResultText.text = "";
        }

        // Обновление UI
        UpdateUI();
    }

    private void StartGame()
    {
        // Check if player has enough money for the bet
        if (moneyController != null && moneyController.SubtractMoney(currentBet))
        {
            gameStarted = true;

            player1WinMultiplier = 2;
            player2WinMultiplier = 2;
            // Enable roll button
            if (rollButton != null)
            {
                rollButton.interactable = true;
            }

            // Disable betting controls
            if (startGameButton != null)
                startGameButton.interactable = false;

            if (increaseBetButton != null)
                increaseBetButton.interactable = false;

            if (decreaseBetButton != null)
                decreaseBetButton.interactable = false;

            // Setup boosts and start game
            SetupBoosts();

            // Display bet confirmation
            currentPlayerText.text = $"Turn {player1Name}";
        }
        else
        {
            // Not enough money message
            currentPlayerText.text = "no money!";
        }
    }

    private void IncreaseBet()
    {
        currentBet += betIncrement;
        UpdateBetText();
    }

    private void DecreaseBet()
    {
        if (currentBet > minimumBet)
        {
            currentBet -= betIncrement;
            UpdateBetText();
        }
    }

    private void UpdateBetText()
    {
        if (currentBetText != null)
        {
            currentBetText.text = "" + currentBet;
        }
    }

    private void SetupBoosts()
    {
        // Очистка старых эффектов
        foreach (var effect in boostEffects)
        {
            if (effect != null)
                Destroy(effect);
        }

        boostEffects.Clear();

        // Инициализация массива бустов
        spotBoostTypes = new BoostType[boardSpots.Length];
        for (int i = 0; i < spotBoostTypes.Length; i++)
        {
            spotBoostTypes[i] = BoostType.None;
        }

        // НАЗНАЧАЕМ СЛУЧАЙНЫЕ ЦВЕТА ВСЕМ ТОЧКАМ СРАЗУ ПОСЛЕ СТАРТА
        AssignRandomColorsToSpots();

        // Создание положительных бустов (не на стартовой позиции)
        int boostsCreated = 0;
        while (boostsCreated < maxBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1);

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= boostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.Positive;
                boostsCreated++;

                // Создание только визуального эффекта, БЕЗ изменения цвета
                if (boostEffectPrefab != null)
                {
                    GameObject boostEffect = Instantiate(boostEffectPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f + Vector3.up * playerHeightOffset,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }
            }
        }

        // Создание негативных бустов
        int negativeBoostsCreated = 0;
        while (negativeBoostsCreated < maxNegativeBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1);

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= negativeBoostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.Negative;
                negativeBoostsCreated++;

                // Создание только визуального эффекта, БЕЗ изменения цвета
                if (negativeBoostPrefab != null)
                {
                    GameObject boostEffect = Instantiate(negativeBoostPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f + Vector3.up * playerHeightOffset,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }
            }
        }

        // Создание бустов множителя
        int multiplierBoostsCreated = 0;
        while (multiplierBoostsCreated < maxMultiplierBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1);

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= multiplierBoostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.Multiplier;
                multiplierBoostsCreated++;

                // Создание только визуального эффекта, БЕЗ изменения цвета
                if (multiplierBoostPrefab != null)
                {
                    GameObject boostEffect = Instantiate(multiplierBoostPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f + Vector3.up * playerHeightOffset,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }
            }
        }
    }

    private void AssignRandomColorsToSpots()
    {
        for (int i = 0; i < boardSpots.Length; i++)
        {
            // Первая точка (стартовая) остается белой
            if (i == 0)
            {
                Renderer renderer = boardSpots[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = normalSpotColor;
                }
                continue;
            }

            // Назначаем случайный цвет из трех доступных
            Color[] availableColors = { step1Color, step2Color, step3Color };
            Color assignedColor = availableColors[Random.Range(0, 3)];

            Renderer spotRenderer = boardSpots[i].GetComponent<Renderer>();
            if (spotRenderer != null)
            {
                spotRenderer.material.color = assignedColor;
                Debug.Log($"Spot {i} assigned color: {assignedColor}");
            }
        }
    }

    public void RollDice()
    {
        if (!gameStarted || waitingForPlayerMove)
            return;
            
        // Проверка, что игрок не движется и кубик не в процессе броска
        if (!isMoving && !isRolling)
        {
            // Check if current player should skip their turn
            if ((isPlayer1Turn && player1SkipNextTurn) || (!isPlayer1Turn && player2SkipNextTurn))
            {
                if (isPlayer1Turn)
                {
                    Debug.Log("Player 1 skips turn");
                    currentPlayerText.text = $"{player1Name} skips a turn";
                    player1SkipNextTurn = false;

                    // Remove all skip turn boosts for player 1
                    foreach (int spotIndex in player1SkipTurnSpots)
                    {
                        if (spotBoostTypes[spotIndex] == BoostType.SkipTurn)
                        {
                            spotBoostTypes[spotIndex] = BoostType.None;
                            RemoveBoostEffect(spotIndex);
                        }
                    }

                    player1SkipTurnSpots.Clear();
                }
                else
                {
                    Debug.Log("Player 2 skips turn");
                    currentPlayerText.text = $"{player2Name} skips turn";
                    player2SkipNextTurn = false;

                    // Remove all skip turn boosts for player 2
                    foreach (int spotIndex in player2SkipTurnSpots)
                    {
                        if (spotBoostTypes[spotIndex] == BoostType.SkipTurn)
                        {
                            spotBoostTypes[spotIndex] = BoostType.None;
                            RemoveBoostEffect(spotIndex);
                        }
                    }

                    player2SkipTurnSpots.Clear();
                }

                // Switch turn without rolling
                SwitchTurn();
                return;
            }

            StartCoroutine(DiceRollAnimation());
        }
    }

    private IEnumerator DiceRollAnimation()
{
    isRolling = true;
    rollButton.interactable = false; // Disable button during roll

    // Get reference to wheel controller
    WheelController wheelController = FindObjectOfType<WheelController>();
    if (wheelController == null)
    {
        Debug.LogError("WheelController not found!");
        isRolling = false;
        rollButton.interactable = true; // Re-enable if error
        yield break;
    }

    // Check if wheel is already spinning
    if (wheelController.IsSpinning())
    {
        Debug.LogWarning("Wheel is already spinning, waiting...");
        yield return new WaitUntil(() => !wheelController.IsSpinning());
    }

    // Play spin sound
    if (audioManager != null)
        audioManager.PlaySound(0);

    // Hide result text during spin
    if (diceResultText != null)
        diceResultText.text = "";

    // Spin the wheel and wait for result
    int result = 0;
    bool spinCompleted = false;
    
    yield return StartCoroutine(wheelController.SpinWheel((value) => {
        result = value;
        spinCompleted = true;
    }));

    // Fallback timeout in case wheel doesn't respond
    float timeout = 10f;
    while (!spinCompleted && timeout > 0)
    {
        timeout -= Time.deltaTime;
        yield return null;
    }

    if (!spinCompleted)
    {
        Debug.LogError("Wheel spin timed out!");
        result = Random.Range(1, 4); // Fallback random result
    }

    if (diceResultText != null)
        diceResultText.text = GetColorName(result);

    yield return new WaitForSeconds(0.5f); // Pause briefly to show the result

    isRolling = false;
    currentDiceResult = result;

    // Show available spots for move
    if (isPlayer1Turn)
    {
        ShowAvailableSpots(player1SpotIndex, result);
    }
    else
    {
        ShowAvailableSpots(player2SpotIndex, result);
    }

    waitingForPlayerMove = true;
    currentPlayerText.text = $"{(isPlayer1Turn ? player1Name : player2Name)}, choose a spot to move to!";
}

    private string GetColorName(int colorValue)
    {
        switch (colorValue)
        {
            case 0:
            case 1:
                return "Red";
            case 2:
                return "Yellow";
            case 3:
                return "Green";
            default:
                return "Red";
        }
    }
    
    private void ShowAvailableSpots(int currentPosition, int diceResult)
    {
        Debug.Log($"ShowAvailableSpots called: currentPosition={currentPosition}, diceResult={diceResult}");

        // Сначала делаем все точки неинтерактивными
        for (int i = 0; i < interactiveSpots.Length; i++)
        {
            interactiveSpots[i].SetInteractable(false);
        }

        if (diceResult <= 0)
        {
            Debug.Log("Dice result is 0 or negative, no movement possible");
            currentPlayerText.text = $"{(isPlayer1Turn ? player1Name : player2Name)} rolled 0 - turn skipped!";
            waitingForPlayerMove = false;
            StartCoroutine(DelayedTurnSwitch());
            return;
        }

        // Определяем текущий этап игрока
        int currentStage = isPlayer1Turn ? player1CurrentStage : player2CurrentStage;

        // КЛЮЧЕВАЯ ЛОГИКА: если игрок уже сделал ход на текущем этапе, переводим его на следующий
        if (currentPosition > 0) // Если игрок не на стартовой позиции
        {
            int positionStage = ((currentPosition - 1) / positionsPerStage) + 1;
            int nextStage = positionStage + 1;

            // Обновляем этап на следующий
            if (isPlayer1Turn)
            {
                player1CurrentStage = nextStage;
            }
            else
            {
                player2CurrentStage = nextStage;
            }
            
            currentStage = nextStage;
            Debug.Log($"Player moved from stage {positionStage} to stage {nextStage}");
        }

        // Вычисляем диапазон позиций для текущего этапа
        int stageStartIndex = (currentStage - 1) * positionsPerStage + 1;
        int stageEndIndex = currentStage * positionsPerStage;

        // Убеждаемся, что не выходим за границы массива
        stageEndIndex = Mathf.Min(stageEndIndex, boardSpots.Length - 1);

        Debug.Log($"Player at position {currentPosition}, current stage {currentStage}");
        Debug.Log($"Stage {currentStage}: positions {stageStartIndex} to {stageEndIndex}");

        Color stepColor = GetColorForStep(diceResult);
        int availableCount = 0;

        // Проверяем только позиции в пределах текущего этапа
        for (int spotIndex = stageStartIndex; spotIndex <= stageEndIndex; spotIndex++)
        {
            // Убеждаемся, что это позиция после текущей
            if (spotIndex <= currentPosition) continue;

            Renderer renderer = boardSpots[spotIndex].GetComponent<Renderer>();
            if (renderer != null)
            {
                Color spotColor = renderer.material.color;
                bool matches = ColorEquals(spotColor, stepColor);
                Debug.Log($"Checking spot {spotIndex}: Expected={stepColor}, Actual={spotColor}, Matches={matches}");

                if (matches)
                {
                    Debug.Log($"Making spot {spotIndex} interactable");
                    interactiveSpots[spotIndex].SetInteractable(true);
                    availableCount++;
                }
            }
        }
        
        Debug.Log($"Total available spots: {availableCount}");

        if (availableCount <= 0)
        {
            currentPlayerText.text = $"{(isPlayer1Turn ? player1Name : player2Name)}, no spots available for this color in stage {currentStage}!";
            waitingForPlayerMove = false;
            StartCoroutine(DelayedTurnSwitch());
        }
    }
    
    private IEnumerator DelayedTurnSwitch()
    {
        yield return new WaitForSeconds(1.5f);
        SwitchTurn();
        waitingForPlayerMove = false;
        // Reactivate the roll button after switching turns
        if (rollButton != null && gameStarted)
        {
            rollButton.interactable = true;
        }
    }
    
    private Color GetColorForStep(int stepValue)
    {
        switch (stepValue)
        {
            case 0:
                return step1Color; // Красный
            case 1:
                return step1Color; // Красный  
            case 2:
                return step2Color; // Желтый
            case 3:
                return step3Color; // Зеленый
            default:
                return step1Color;
        }
    }

    private void HideAvailableSpots()
    {
        for (int i = 0; i < interactiveSpots.Length; i++)
        {
            interactiveSpots[i].SetInteractable(false);
        }
        // Цвета точек НЕ меняем - они остаются такими как назначены при старте
    }
    
    public void OnSpotClicked(int spotIndex)
    {
        if (!waitingForPlayerMove)
            return;

        // Проверяем, что точка интерактивна и находится в допустимом диапазоне этапа
        if (interactiveSpots[spotIndex].isInteractable)
        {
            int currentPosition = isPlayer1Turn ? player1SpotIndex : player2SpotIndex;
            int currentStage = isPlayer1Turn ? player1CurrentStage : player2CurrentStage;
            int stageStartIndex = (currentStage - 1) * positionsPerStage + 1;
            int stageEndIndex = currentStage * positionsPerStage;
            stageEndIndex = Mathf.Min(stageEndIndex, boardSpots.Length - 1);

            if (spotIndex > currentPosition && spotIndex >= stageStartIndex && spotIndex <= stageEndIndex)
            {
                waitingForPlayerMove = false;
                HideAvailableSpots();
                
                if (isPlayer1Turn)
                    StartCoroutine(MovePlayerToSpot(player1, spotIndex, 1));
                else
                    StartCoroutine(MovePlayerToSpot(player2, spotIndex, 2));
            }
            else
            {
                Debug.Log($"Invalid move! Must be within stage {currentStage} (positions {stageStartIndex}-{stageEndIndex})");
            }
        }
        else
        {
            Debug.Log("Wrong color clicked!");
        }
    }
    
    // Вспомогательный метод для сравнения цветов (учитывает небольшие погрешности)
    private bool ColorEquals(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
    
    private IEnumerator MovePlayerToSpot(Transform playerTransform, int targetSpot, int playerNumber)
    {
        isMoving = true;

        int currentPosition = playerNumber == 1 ? player1SpotIndex : player2SpotIndex;

        // Анимация бега
        Animator playerAnimator = playerNumber == 1 ? player1Animator : player2Animator;
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsRunning", true);
        }

        yield return StartCoroutine(MoveToNextSpot(playerTransform, targetSpot, currentPosition));

        // Обновляем позицию игрока
        if (playerNumber == 1)
        {
            player1SpotIndex = targetSpot;
            // Проверяем, нужно ли перейти на следующий этап
            CheckStageProgression(1);
        }
        else
        {
            player2SpotIndex = targetSpot;
            CheckStageProgression(2);
        }

        // Останавливаем анимацию бега
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsRunning", false);
        }

        UpdateUI();

        // Обрабатываем действие на точке
        bool shouldSwitchTurn = HandleSpotAction(playerNumber, targetSpot);

        // Проверяем победу
        if (CheckWinCondition())
        {
            string winner = isPlayer1Turn ? $"{player1Name}" : $"{player2Name}";
            currentPlayerText.text = winner + " Win!";
            rollButton.interactable = false;
        }
        else if (shouldSwitchTurn)
        {
            SwitchTurn();
            rollButton.interactable = true;
        }
        else
        {
            rollButton.interactable = true;
        }

        isMoving = false;
    }

    private void CheckStageProgression(int playerNumber)
    {
        int playerPosition = playerNumber == 1 ? player1SpotIndex : player2SpotIndex;

        // После каждого хода игрок автоматически переходит на следующий этап
        if (playerPosition > 0) // Если не на стартовой позиции
        {
            int positionStage = ((playerPosition - 1) / positionsPerStage) + 1;
            int nextStage = positionStage + 1;

            if (playerNumber == 1)
            {
                player1CurrentStage = nextStage;
                Debug.Log($"Player 1 automatically advanced to stage {player1CurrentStage} after moving to position {playerPosition}");
            }
            else
            {
                player2CurrentStage = nextStage;
                Debug.Log($"Player 2 automatically advanced to stage {player2CurrentStage} after moving to position {playerPosition}");
            }
        }
    }

    private IEnumerator MoveToNextSpot(Transform playerTransform, int spotIndex, int startSpotIndex)
    {
        Vector3 startPosition = playerTransform.position;
        Vector3 targetPosition = boardSpots[spotIndex].position + Vector3.up * playerHeightOffset;
        Vector3 startPositionNoY = new Vector3(startPosition.x, 0, startPosition.z);
        Vector3 targetPositionNoY = new Vector3(targetPosition.x, 0, targetPosition.z);

        float journeyLength = Vector3.Distance(startPositionNoY, targetPositionNoY);
        float startTime = Time.time;
        float moveDuration = journeyLength / moveSpeed;
        float elapsedTime = 0f;


        while (elapsedTime < moveDuration)
        {
            elapsedTime = Time.time - startTime;
            float fractionOfJourney = Mathf.Clamp01(elapsedTime / moveDuration);
            
            // Horizontal movement
            Vector3 horizontalPosition = Vector3.Lerp(startPositionNoY, targetPositionNoY, fractionOfJourney);
            
            // Vertical movement (smooth transition between heights + jump animation)
            float startY = startPosition.y;
            float targetY = targetPosition.y;
            float currentBaseY = Mathf.Lerp(startY, targetY, fractionOfJourney);
            
            // Jump animation
            float jumpHeight = 0.5f;
            float jumpCurve = Mathf.Sin(fractionOfJourney * Mathf.PI) * jumpHeight;
            
            // Apply position
            playerTransform.position = new Vector3(
                horizontalPosition.x, 
                currentBaseY + jumpCurve, 
                horizontalPosition.z
            );

            // Rotate toward movement direction
            Vector3 direction = (targetPositionNoY - startPositionNoY).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                playerTransform.rotation = Quaternion.Slerp(
                    playerTransform.rotation, 
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }

            yield return null;
        }

        // Ensure final position is exact
        playerTransform.position = targetPosition;
    }

    private bool HandleSpotAction(int playerNumber, int spotIndex)
    {
        bool shouldSwitchTurn = true; // Default: switch turns

        // Проверка наличия буста на точке
        if (spotBoostTypes[spotIndex] != BoostType.None)
        {
            Vector3 spotPosition = boardSpots[spotIndex].position;

            if (spotBoostTypes[spotIndex] == BoostType.Positive)
            {
                // Дополнительный ход
                if (playerNumber == 1)
                {
                    Debug.Log("Player 1 got a boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player1Name} got a boost at spot!";
                }
                else
                {
                    Debug.Log("Player 2 got a boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player2Name} got a boost at spot!";
                }
                audioManager.PlaySound(2);
                // Don't switch turns when positive boost is found
                shouldSwitchTurn = false;

                // Удаляем буст после использования
                spotBoostTypes[spotIndex] = BoostType.None;
                RemoveBoostEffect(spotIndex);
            }
            else if (spotBoostTypes[spotIndex] == BoostType.Negative)
            {
                int startSpot = 0;
                // Перемещение игрока на первую точку (индекс 0)
                if (playerNumber == 1)
                {
                    Debug.Log("Player 1 got a negative boost at spot " + spotIndex);
                    player1CurrentStage = 1;
                    currentPlayerText.text = $"{player1Name} returns to the first stage!";
                    player1.position = boardSpots[startSpot].position + Vector3.up * playerHeightOffset;
                    player1SpotIndex = startSpot;
                }
                else
                {
                    Debug.Log("Player 2 got a negative boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player2Name} returns to the first stage!";
                    player2CurrentStage = 1;
                    player2.position = boardSpots[startSpot].position + Vector3.up * playerHeightOffset;
                    player2SpotIndex = startSpot;
                }
                audioManager.PlaySound(1);
                // Создаём эффект исчезновения негативного буста
                if (negativeBoostDisappearEffectPrefab != null)
                {
                    GameObject disappearEffect = Instantiate(negativeBoostDisappearEffectPrefab,
                        spotPosition + Vector3.up * 0.5f + Vector3.up * playerHeightOffset,
                        Quaternion.identity);

                    // Автоматическое удаление эффекта через некоторое время
                    Destroy(disappearEffect, 2.0f);
                }

                // Удаляем буст после использования
                spotBoostTypes[spotIndex] = BoostType.None;
                RemoveBoostEffect(spotIndex);
            }
            else if (spotBoostTypes[spotIndex] == BoostType.SkipTurn)
            {
                // Check if both players are on the same skip turn boost spot
                if ((playerNumber == 1 && player2SpotIndex == spotIndex) ||
                    (playerNumber == 2 && player1SpotIndex == spotIndex))
                {
                    // Both players are on the same skip turn boost!
                    Debug.Log("Both players landed on the same skip turn boost! Teleporting both to start!");
                    currentPlayerText.text = "Both players are teleported to the first stage!";

                    // Reset both skip turn flags
                    player1SkipNextTurn = false;
                    player2SkipNextTurn = false;

                    // Create negative boost disappear effect
                    if (negativeBoostDisappearEffectPrefab != null)
                    {
                        GameObject disappearEffect = Instantiate(negativeBoostDisappearEffectPrefab,
                            spotPosition + Vector3.up * 0.5f + Vector3.up * playerHeightOffset,
                            Quaternion.identity);

                        // Автоматическое удаление эффекта через некоторое время
                        Destroy(disappearEffect, 2.0f);
                    }

                    // Teleport both players to the first spot
                    int startSpot = 0;
                    player1.position = boardSpots[startSpot].position + Vector3.up * playerHeightOffset;
                    player1SpotIndex = startSpot;
                    player1CurrentStage = 1;
                    player2.position = boardSpots[startSpot].position + Vector3.up * playerHeightOffset;
                    player2SpotIndex = startSpot;
                    player2CurrentStage = 1;

                    // Remove the boost
                    spotBoostTypes[spotIndex] = BoostType.None;
                    RemoveBoostEffect(spotIndex);
                    skipTurnBoostSpot = -1;
                }
                else
                {
                    // Normal skip turn logic
                    // Set flag to skip next turn
                    // When player lands on skip turn boost, store in their list
                    if (playerNumber == 1)
                    {
                        currentPlayerText.text = $"{player1Name} will miss the next turn!";
                        if (skipTurnBoostDisappearEffectPrefab != null)
                        {
                            GameObject disappearEffect = Instantiate(skipTurnBoostDisappearEffectPrefab,
                                spotPosition + Vector3.up * 0.5f + Vector3.up * playerHeightOffset,
                                Quaternion.identity);
                            boostEffects.Add(disappearEffect);
                        }

                        player1SkipNextTurn = true;
                        player1SkipTurnSpots.Add(spotIndex); // Add to player 1's list
                    }
                    else
                    {
                        if (skipTurnBoostDisappearEffectPrefab != null)
                        {
                            GameObject disappearEffect = Instantiate(skipTurnBoostDisappearEffectPrefab,
                                spotPosition + Vector3.up * 0.5f + Vector3.up * playerHeightOffset,
                                Quaternion.identity);
                            boostEffects.Add(disappearEffect);
                        }

                        currentPlayerText.text = $"{player2Name} will miss the next turn!";
                        player2SkipNextTurn = true;
                        player2SkipTurnSpots.Add(spotIndex); // Add to player 2's list
                    }
                    audioManager.PlaySound(2);
                    // Save the spot index for later removal
                    skipTurnBoostSpot = spotIndex;
                }
            }
            else if (spotBoostTypes[spotIndex] == BoostType.Multiplier)
            {
                if (playerNumber == 1)
                {
                    player1WinMultiplier++;
                    Debug.Log("Player 1 got multiplier boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player1Name} got multiplier boost x{player1WinMultiplier}!";
                }
                else
                {
                    player2WinMultiplier++;
                    Debug.Log("Player 2 got multiplier boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player2Name} got multiplier boost x{player2WinMultiplier}!";
                }
                audioManager.PlaySound(3);
                UpdateUI();
                // Удаляем буст после использования
                spotBoostTypes[spotIndex] = BoostType.None;
                RemoveBoostEffect(spotIndex);
            }
        }

        return shouldSwitchTurn;
    }

    // Helper method to remove boost effects
    private void RemoveBoostEffect(int spotIndex)
    {
        // Удаляем все визуальные эффекты буста для данной точки
        Vector3 spotPosition = boardSpots[spotIndex].position;

        for (int i = boostEffects.Count - 1; i >= 0; i--)
        {
            if (boostEffects[i] != null)
            {
                // Проверяем расстояние между эффектом и позицией точки
                float distance = Vector3.Distance(boostEffects[i].transform.position, spotPosition);
                if (distance < 3f) // Допустимое расстояние для считывания эффекта на той же точке
                {
                    Destroy(boostEffects[i]);
                    boostEffects.RemoveAt(i);
                    Debug.Log($"Removed boost effect at spot {spotIndex}");
                }
            }
            else
            {
                // Удаляем null-ссылки
                boostEffects.RemoveAt(i);
            }
        }
    }

    private void SwitchTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;

        if (isPlayer1Turn)
        {
            currentPlayerText.text = $"Turn {player1Name}";
            virtualCamera.Follow = player1;
        }
        else
        {
            currentPlayerText.text = $"Turn {player2Name}";
            virtualCamera.Follow = player2;
        }
    }

    private IEnumerator WaitForReward()
    {
        yield return new WaitForSeconds(4f);
        winPL.SetActive(true);
    }
    
    private bool CheckWinCondition()
    {
        Debug.Log(
            $"Checking win: Player1 at {player1SpotIndex}, Player2 at {player2SpotIndex}, winPosition is {winPosition}");
        if (player1SpotIndex >= winPosition)
        {
            audioManager.PlaySound(4);
            isWinning = true;
            // Player 1 won - award bet x multiplier
            if (moneyController != null)
            {
                int reward = currentBet * player1WinMultiplier;
                moneyController.AddMoney(reward);
                winText.text = $"{player1Name} wins! Received {reward} coins (x{player1WinMultiplier})!";
            }

            if (player1Animator != null)
                player1Animator.SetBool("Win", true);
            if (player2Animator != null)
                player2Animator.SetBool("IsRunning", false);

            if (increaseBetButton != null)
                increaseBetButton.interactable = true;

            if (decreaseBetButton != null)
                decreaseBetButton.interactable = true;
                
            if (startGameButton != null)
                startGameButton.interactable = true;
                
            StartCoroutine(WaitForReward());
            gameStarted = false;
            return true;
        }
        else if (player2SpotIndex >= winPosition)
        {
            audioManager.PlaySound(4);
            isWinning = true;
            if (moneyController != null)
            {
                int reward = currentBet * player2WinMultiplier;
                moneyController.AddMoney(reward);
                winText.text = $"{player2Name} wins! Received {reward} coins (x{player2WinMultiplier})!";
            }

            if (player1Animator != null)
                player1Animator.SetBool("IsRunning", false);
            if (player2Animator != null)
                player2Animator.SetBool("Win", true);

            if (increaseBetButton != null)
                increaseBetButton.interactable = true;

            if (decreaseBetButton != null)
                decreaseBetButton.interactable = true;
                
            if (startGameButton != null)
                startGameButton.interactable = true;
                
            StartCoroutine(WaitForReward());
            gameStarted = false;
            return true;
        }

        return false;
    }

    private void UpdateUI()
    {
        if (player1PositionText != null)
        {
            player1PositionText.text = $"{player1Name}: {player1SpotIndex + 1}/{boardSpots.Length} Stage {player1CurrentStage} (x{player1WinMultiplier})";
        }

        if (player2PositionText != null)
        {
            player2PositionText.text = $"{player2Name}: {player2SpotIndex + 1}/{boardSpots.Length} Stage {player2CurrentStage} (x{player2WinMultiplier})";
        }

        if (currentPlayerText != null && !isWinning)
        {
            if (isPlayer1Turn)
                currentPlayerText.text = $"Turn {player1Name}";
            else
                currentPlayerText.text = $"Turn {player2Name}";
        }
    }
}

