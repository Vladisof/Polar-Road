using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceRollAndMovement : MonoBehaviour
{
    [Header("Dice UI")]
    [SerializeField] private TextMeshProUGUI diceResultText; // Текст для отображения числа на кубике
    [SerializeField] private Button rollButton; // Кнопка для броска кубика
    
    [Header("Player Movement")]
    [SerializeField] private Transform player; // Трансформ игрока
    [SerializeField] private Transform[] boardSpots; // Массив существующих точек на доске
    [SerializeField] private float moveSpeed = 5f; // Скорость перемещения игрока
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота игрока
    
    [Header("Dice Animation Settings")]
    [SerializeField] private float diceAnimationDuration = 2f; // Длительность анимации кубика
    [SerializeField] private float numberChangeInterval = 0.1f; // Интервал смены чисел во время анимации

    private int currentSpotIndex = 0; // Текущая позиция игрока
    private bool isMoving = false; // Флаг для отслеживания движения игрока
    private bool isRolling = false; // Флаг для отслеживания броска кубика

    private void Start()
    {
        // Инициализация начального положения игрока
        if (boardSpots.Length > 0 && player != null)
        {
            player.position = boardSpots[0].position;
        }
        else
        {
            Debug.LogError("BoardSpots array is empty or player is not assigned!");
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
        
        // Установка начального текста
        if (diceResultText != null)
        {
            diceResultText.text = "0";
        }
        else
        {
            Debug.LogError("Dice result text is not assigned!");
        }
    }

    public void RollDice()
    {
        // Проверка, что игрок не движется и кубик не в процессе броска
        if (!isMoving && !isRolling)
        {
            StartCoroutine(DiceRollAnimation());
        }
    }

    private IEnumerator DiceRollAnimation()
    {
        isRolling = true;
        rollButton.interactable = false; // Деактивируем кнопку на время броска
        
        float elapsedTime = 0f;
        
        // Анимация смены чисел
        while (elapsedTime < diceAnimationDuration)
        {
            // Генерация случайного числа от 1 до 6
            int randomValue = Random.Range(1, 7);
            diceResultText.text = randomValue.ToString();
            
            // Эффекты для анимации (можно добавить звук, изменение цвета и т.д.)
            float scale = 1f + Mathf.Sin(elapsedTime * 15f) * 0.2f;
            diceResultText.transform.localScale = new Vector3(scale, scale, 1f);
            
            elapsedTime += numberChangeInterval;
            yield return new WaitForSeconds(numberChangeInterval);
        }
        
        // Финальный результат броска
        int steps = Random.Range(1, 7);
        diceResultText.text = steps.ToString();
        diceResultText.transform.localScale = Vector3.one; // Сброс масштаба
        
        isRolling = false;
        
        // Начало движения игрока
        StartCoroutine(MovePlayer(steps));
    }

    private IEnumerator MovePlayer(int steps)
    {
        isMoving = true;
        
        for (int i = 0; i < steps; i++)
        {
            // Вычисление следующей позиции с учетом цикличности доски
            int nextSpotIndex = (currentSpotIndex + 1) % boardSpots.Length;
            
            // Анимация перемещения к следующей точке
            yield return StartCoroutine(MoveToNextSpot(nextSpotIndex));
            
            // Обновление текущей позиции
            currentSpotIndex = nextSpotIndex;
            
            // Небольшая пауза между шагами
            yield return new WaitForSeconds(0.2f);
        }
        
        // Триггер событий на новой клетке (можно добавить логику для разных типов клеток)
        Debug.Log("Player arrived at spot: " + currentSpotIndex);
        
        isMoving = false;
        rollButton.interactable = true; // Активируем кнопку снова
    }

    private IEnumerator MoveToNextSpot(int spotIndex)
    {
        Vector3 startPosition = player.position;
        Vector3 targetPosition = boardSpots[spotIndex].position;
        
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;
        
        while (Vector3.Distance(player.position, targetPosition) > 0.01f)
        {
            // Вычисление текущего прогресса движения
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = Mathf.Clamp01(distanceCovered / journeyLength);
            
            // Плавное перемещение
            player.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            
            // Плавный поворот в сторону движения
            if (targetPosition != startPosition)
            {
                Vector3 direction = (targetPosition - player.position).normalized;
                
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
            
            // Дополнительная анимация прыжка (опционально)
            float jumpHeight = 0.5f;
            float jumpCurve = Mathf.Sin(fractionOfJourney * Mathf.PI) * jumpHeight;
            player.position = new Vector3(player.position.x, startPosition.y + jumpCurve, player.position.z);
            
            yield return null;
        }
        
        // Убедимся, что игрок точно в позиции
        player.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
    }
}