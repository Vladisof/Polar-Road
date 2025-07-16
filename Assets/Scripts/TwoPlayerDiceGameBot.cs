using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TwoPlayerDiceGameBot : MonoBehaviour
{
    [Header("Dice UI")]
    [SerializeField] private AudioManager audioManager; // AudioManager reference
    [SerializeField] private TextMeshProUGUI diceResultText; // Текст для отображения числа на кубике
    
    [Header("Bot Settings")]
    [SerializeField] private float botThinkingDelay = 1.5f; // Delay before bot makes a move
    private bool isBotTurn = false;
    
    [SerializeField] private Button rollButton; // Кнопка для броска кубика

    [Header("Players Setup")] [SerializeField]
    private Transform player1; // Трансформ первого игрока

    [SerializeField] private Transform player2; // Трансформ второго игрока
    [SerializeField] private Transform[] boardSpots; // Массив существующих точек на доске
    [SerializeField] private float moveSpeed = 5f; // Скорость перемещения игроков
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота игроков

    [Header("Wheel of Fortune")]
    [SerializeField] private RectTransform wheelImage; // Reference to your wheel image
    [SerializeField] private float spinSpeed = 500f; // Base speed for spinning the wheel
    [SerializeField] private float spinSlowdownRate = 0.97f; // Rate at which the wheel slows down
    [SerializeField] private float minSpinSpeed = 20f; // Minimum speed before stopping
    
    [Header("Player Info UI")] [SerializeField]
    private TextMeshProUGUI player1PositionText; // Текст позиции первого игрока

    [SerializeField] private GameObject winPL;
    [SerializeField] private GameObject losePL;
    [SerializeField] private TextMeshProUGUI player2PositionText; // Текст позиции второго игрока
    [SerializeField] private TextMeshProUGUI currentPlayerText; // Текст текущего игрока
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
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
    [Range(0f, 1f)] private float skipTurnBoostSpawnChance = 0.15f; // Шанс появления буста пропуска хода
    [Range(0f, 1f)] private float multiplierBoostSpawnChance = 0.15f; // Шанс появления буста множителя
    private int maxBoosts = 3; // Максимальное количество бустов на карте
    private int maxNegativeBoosts = 2; // Максимальное количество негативных бустов
    private int maxSkipTurnBoosts = 1; // Максимальное количество бустов пропуска хода
    private int maxMultiplierBoosts = 2; // Максимальное количество бустов множителя
    [SerializeField] private Color boostColor = Color.yellow; // Цвет для выделения этапов с бустами
    [SerializeField] private Color negativeBoostColor = Color.red; // Цвет для выделения этапов с негативными бустами
    [SerializeField] private Color skipTurnBoostColor = Color.blue; // Цвет для выделения этапов с бустами пропуска хода
    [SerializeField] private Color multiplierBoostColor = new Color(1f, 0.5f, 0f); // Оранжевый цвет для множителя

    private int player1SpotIndex = 0; // Текущая позиция первого игрока
    private int player2SpotIndex = 0; // Текущая позиция второго игрока
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
        isPlayer1Turn = true;
        isBotTurn = false;
        moneyController = FindObjectOfType<MoneyController>();
        if (moneyController == null)
        {
            Debug.LogError("MoneyController not found in scene!");
        }

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
                player1.position = boardSpots[0].position;
            else
                Debug.LogError("Player 1 is not assigned!");

            if (player2 != null)
                player2.position = boardSpots[0].position;
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
            diceResultText.text = "0";
        }

        LoadBoostUpgrades();
        spotBoostTypes = new BoostType[boardSpots.Length];
        // Обновление UI
        UpdateUI();
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

    public void SetSkipTurnBoostChance(float chance)
    {
        skipTurnBoostSpawnChance = Mathf.Clamp01(chance);
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
        skipTurnBoostSpawnChance =
            Mathf.Max(0.05f, baseSkipTurnChance - (decreaseSkipTurnLevel * skipTurnReductionPerLevel));
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
                player1.position = boardSpots[0].position;
            else
                Debug.LogError("Player 1 is not assigned!");

            if (player2 != null)
                player2.position = boardSpots[0].position;
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
            diceResultText.text = "0";
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

            // Make sure player 1 (human) starts
            isPlayer1Turn = true;
            isBotTurn = false;

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

        // Создание положительных бустов (не на стартовой позиции)
        int boostsCreated = 0;
        while (boostsCreated < maxBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1); // Исключаем стартовую позицию

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= boostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.Positive;
                boostsCreated++;

                // Создание визуального эффекта
                if (boostEffectPrefab != null)
                {
                    GameObject boostEffect = Instantiate(boostEffectPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }

                // Изменение цвета точки
                Renderer spotRenderer = boardSpots[spotIndex].GetComponent<Renderer>();
                if (spotRenderer != null)
                {
                    spotRenderer.material.color = boostColor;
                }
            }
        }

        // Создание негативных бустов
        int negativeBoostsCreated = 0;
        while (negativeBoostsCreated < maxNegativeBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1); // Исключаем стартовую позицию

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= negativeBoostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.Negative;
                negativeBoostsCreated++;

                // Создание визуального эффекта
                if (negativeBoostPrefab != null)
                {
                    GameObject boostEffect = Instantiate(negativeBoostPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }

                // Изменение цвета точки
                Renderer spotRenderer = boardSpots[spotIndex].GetComponent<Renderer>();
                if (spotRenderer != null)
                {
                    spotRenderer.material.color = negativeBoostColor;
                }
            }
        }

        // Создание бустов пропуска хода
        int skipTurnBoostsCreated = 0;
        while (skipTurnBoostsCreated < maxSkipTurnBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1); // Исключаем стартовую позицию

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= skipTurnBoostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.SkipTurn;
                skipTurnBoostsCreated++;

                // Создание визуального эффекта
                if (skipTurnBoostPrefab != null)
                {
                    GameObject boostEffect = Instantiate(skipTurnBoostPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }

                // Изменение цвета точки
                Renderer spotRenderer = boardSpots[spotIndex].GetComponent<Renderer>();
                if (spotRenderer != null)
                {
                    spotRenderer.material.color = skipTurnBoostColor;
                }
            }
        }

        int multiplierBoostsCreated = 0;
        while (multiplierBoostsCreated < maxMultiplierBoosts)
        {
            int spotIndex = Random.Range(1, boardSpots.Length-1); // Исключаем стартовую позицию

            if (spotBoostTypes[spotIndex] == BoostType.None && Random.value <= multiplierBoostSpawnChance)
            {
                spotBoostTypes[spotIndex] = BoostType.Multiplier;
                multiplierBoostsCreated++;

                // Создание визуального эффекта
                if (multiplierBoostPrefab != null)
                {
                    GameObject boostEffect = Instantiate(multiplierBoostPrefab,
                        boardSpots[spotIndex].position + Vector3.up * 0.1f,
                        Quaternion.identity);
                    boostEffects.Add(boostEffect);
                }

                // Изменение цвета точки
                Renderer spotRenderer = boardSpots[spotIndex].GetComponent<Renderer>();
                if (spotRenderer != null)
                {
                    spotRenderer.material.color = multiplierBoostColor;
                }
            }
        }
    }

public void RollDice()
    {
        if (!gameStarted)
            return;
        
        // If it's bot's turn, don't allow manual dice roll
        if (!isPlayer1Turn && !isBotTurn)
        {
            Debug.Log("Bot will roll automatically");
            return;
        }
        
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
            yield break;
        }
    
        // Play spin sound
        audioManager.PlaySound(0);
    
        // Hide result text during spin
        if (diceResultText != null)
            diceResultText.text = "";
    
        // Spin the wheel and wait for result
        int result = 0;
        yield return StartCoroutine(wheelController.SpinWheel((value) => result = value));
    
        // Show result
        if (diceResultText != null)
            diceResultText.text = result.ToString();
    
        yield return new WaitForSeconds(0.5f); // Pause briefly to show the result
    
        isRolling = false;
    
        // Start moving the current player
        if (isPlayer1Turn)
            StartCoroutine(MovePlayer(player1, player1SpotIndex, result, 1));
        else
            StartCoroutine(MovePlayer(player2, player2SpotIndex, result, 2));
    }

    private IEnumerator MovePlayer(Transform playerTransform, int currentIndex, int steps, int playerNumber)
    {
        isMoving = true;

        // Set the Run animation
        Animator playerAnimator = playerNumber == 1 ? player1Animator : player2Animator;
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsRunning", true);
        }

        for (int i = 0; i < steps; i++)
        {
            // Вычисление следующей позиции с учетом цикличности доски
            int nextSpotIndex = (currentIndex + 1) % boardSpots.Length;

            if (nextSpotIndex >= boardSpots.Length || nextSpotIndex < 1)
            {
                break;
            }

            // Анимация перемещения к следующей точке
            yield return StartCoroutine(MoveToNextSpot(playerTransform, nextSpotIndex, currentIndex));

            // Обновление текущей позиции
            currentIndex = nextSpotIndex;

            // Обновление UI после каждого шага
            UpdateUI();

            // Небольшая пауза между шагами
            yield return new WaitForSeconds(0.2f);
        }

        if (playerNumber == 1)
            player1SpotIndex = currentIndex;
        else
            player2SpotIndex = currentIndex;
        UpdateUI();

        // Set the Idle animation when movement is complete
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsRunning", false);
        }

        // Проверка специальных действий на новой клетке
        bool shouldSwitchTurn = HandleSpotAction(playerNumber, currentIndex);
// Проверка на победу
        if (CheckWinCondition())
        {
            // Обработка победы будет здесь
            string winner = isPlayer1Turn ? $"{player1Name}" : $"{player2Name}";
            currentPlayerText.text = winner + " Win!";
            Debug.Log(winner + " wins!");
            rollButton.interactable = false;
        }
        else
        {
            // Переключение хода только если нужно
            if (shouldSwitchTurn)
            {
                SwitchTurn();
            }


            
        }

        isMoving = false;
    }

    private IEnumerator MoveToNextSpot(Transform playerTransform, int spotIndex, int startSpotIndex)
{
    Vector3 startPosition = playerTransform.position;
    Vector3 targetPosition = boardSpots[spotIndex].position;
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
                    isBotTurn = false;
                    rollButton.interactable = true; // Enable roll button for player 1
                }
                else
                {
                    Debug.Log("Player 2 got a boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player2Name} got a boost at spot!";
                    isBotTurn = true;
                    StartCoroutine(BotTurn());
                }

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
                    currentPlayerText.text = $"{player1Name} returns to the first stage!";
                    player1.position = boardSpots[startSpot].position;
                    player1SpotIndex = startSpot;
                }
                else
                {
                    Debug.Log("Player 2 got a negative boost at spot " + spotIndex);
                    currentPlayerText.text = $"{player2Name} returns to the first stage!";

                    player2.position = boardSpots[startSpot].position;
                    player2SpotIndex = startSpot;
                }
                audioManager.PlaySound(1);
                // Создаём эффект исчезновения негативного буста
                if (negativeBoostDisappearEffectPrefab != null)
                {
                    GameObject disappearEffect = Instantiate(negativeBoostDisappearEffectPrefab,
                        spotPosition + Vector3.up * 0.5f,
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
                            spotPosition + Vector3.up * 0.5f,
                            Quaternion.identity);

                        // Автоматическое удаление эффекта через некоторое время
                        Destroy(disappearEffect, 2.0f);
                    }

                    // Teleport both players to the first spot
                    int startSpot = 0;
                    player1.position = boardSpots[startSpot].position;
                    player1SpotIndex = startSpot;
                    player2.position = boardSpots[startSpot].position;
                    player2SpotIndex = startSpot;

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
                                spotPosition + Vector3.up * 0.5f,
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
                                spotPosition + Vector3.up * 0.5f,
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
                    // We don't remove the boost visual effect yet,
                    // it will be removed when the player's turn comes again
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
        // Возвращаем цвет точки
        Renderer spotRenderer = boardSpots[spotIndex].GetComponent<Renderer>();
        if (spotRenderer != null)
        {
            spotRenderer.material.color = Color.green;
        }

        // Удаляем все визуальные эффекты буста для данной точки
        Vector3 spotPosition = boardSpots[spotIndex].position;

        // Use a while loop to repeatedly check for and remove any effects near this spot
        bool foundEffect = true;
        while (foundEffect)
        {
            foundEffect = false;
            for (int i = boostEffects.Count - 1; i >= 0; i--)
            {
                if (boostEffects[i] == null)
                {
                    boostEffects.RemoveAt(i);
                    continue;
                }

                // Check if the effect is near this spot (both at normal height and effect height)
                float distance = Vector3.Distance(boostEffects[i].transform.position,
                    spotPosition + Vector3.up * 0.1f);

                float distanceToEffectHeight = Vector3.Distance(
                    new Vector3(boostEffects[i].transform.position.x,
                        boostEffects[i].transform.position.y - 0.4f,
                        boostEffects[i].transform.position.z),
                    spotPosition + Vector3.up * 0.1f);

                if (distance < 0.5f || distanceToEffectHeight < 0.5f)
                {
                    Destroy(boostEffects[i]);
                    boostEffects.RemoveAt(i);
                    foundEffect = true;
                    break; // Break the for loop, but continue the while loop
                }
            }
        }
    }

    // Modify SwitchTurn to handle bot turns
    private void SwitchTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;

        if (isPlayer1Turn)
        {
            // Player 1's turn (human)
            currentPlayerText.text = $"Turn {player1Name}";
            virtualCamera.Follow = player1;
            rollButton.interactable = true; // Активируем кнопку снова
            isBotTurn = false;
        }
        else
        {
            // Player 2's turn (bot)
            currentPlayerText.text = $"{player2Name} is thinking...";
            virtualCamera.Follow = player2;
            
            // Start bot turn after a delay
            isBotTurn = true;
            StartCoroutine(BotTurn());
        }
    }

    // Add this new method for bot's turn
    private IEnumerator BotTurn()
    {
        // Disable roll button during bot's turn
        if (rollButton != null)
            rollButton.interactable = false;
            
        // Wait for the "thinking" delay
        yield return new WaitForSeconds(botThinkingDelay);
        
        // Make sure we're still in bot turn mode (game might have ended during the delay)
        if (isBotTurn && gameStarted && !isMoving && !isRolling && !player2SkipNextTurn)
        {
            currentPlayerText.text = $"{player2Name} is rolling the dice...";
            
            // Bot rolls dice
            RollDice();
        }

        if (player2SkipNextTurn)
        {
            player2SkipNextTurn = false;
            SwitchTurn();
        }
    }
    private IEnumerator WaitForReward(bool isPlayer)
    {
        yield return new WaitForSeconds(4f);
        if (isPlayer)
        {
            winPL.SetActive(true);
        }
        else
        {
            losePL.SetActive(true);
        }
    }
    private bool CheckWinCondition()
    {
        Debug.Log(
            $"Checking win: Player1 at {player1SpotIndex}, Player2 at {player2SpotIndex}, winPosition is {winPosition}");
        if (player1SpotIndex >= winPosition)
        {
            isWinning = true;
            audioManager.PlaySound(4);
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
            StartCoroutine(WaitForReward(true));
            gameStarted = false;
            return true;
        }
        else if (player2SpotIndex >= winPosition)
        {
            audioManager.PlaySound(4);
            isWinning = true;
                loseText.text = $"{player2Name}  wins!";
                
            if (player1Animator != null)
                player1Animator.SetBool("IsRunning", false);
            if (player2Animator != null)
                player2Animator.SetBool("Win", true);

            if (increaseBetButton != null)
                increaseBetButton.interactable = true;

            if (decreaseBetButton != null)
                decreaseBetButton.interactable = true;
            StartCoroutine(WaitForReward(false));
            gameStarted = false;
            return true;
        }

        return false;
    }

    private void UpdateUI()
    {
        if (player1PositionText != null)
        {
            player1PositionText.text = $"{player1Name}: {player1SpotIndex + 1}/{boardSpots.Length} (x{player1WinMultiplier})";
        }

        if (player2PositionText != null)
        {
            player2PositionText.text = $"{player2Name} (Bot): {player2SpotIndex + 1}/{boardSpots.Length} (x{player2WinMultiplier})";
        }
    }
}