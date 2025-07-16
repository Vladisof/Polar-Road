using System.Collections;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] private RectTransform wheelImage;
    [SerializeField] private float spinSpeed = 800f; // Increased initial speed
    [SerializeField] private float spinSlowdownRate = 0.99f; // More gradual slowdown
    [SerializeField] private float minSpinSpeed = 10f; // Lower minimum speed

    // Optional: Add minimum spin time to ensure it always spins at least this long
    [SerializeField] private float minimumSpinTime = 3.0f; 

    private bool isSpinning = false;
    private int lastHitTriggerValue = 0;

    // Reference to your TwoPlayerDiceGameBot script
    private TwoPlayerDiceGameBot gameController;

    private void Start()
    {
        gameController = FindObjectOfType<TwoPlayerDiceGameBot>();
    }

    // Called by wheel trigger points
    public void RegisterTriggerHit(int value)
    {
        // Only register hits when spinning
        if (isSpinning)
        {
            lastHitTriggerValue = value;
            Debug.Log($"Wheel trigger hit: {value}");
        }
    }

    public IEnumerator SpinWheel(System.Action<int> onComplete)
    {
        if (isSpinning)
            yield break;

        isSpinning = true;

        // Initial setup
        float currentSpinSpeed = spinSpeed;
        lastHitTriggerValue = 0;
        float spinTime = 0f;

        // Add some randomness to initial speed
        currentSpinSpeed *= Random.Range(0.9f, 1.3f);

        // Spin the wheel
        while (currentSpinSpeed > minSpinSpeed || spinTime < minimumSpinTime)
        {
            // Rotate the wheel
            if (wheelImage != null)
            {
                wheelImage.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
            }

            // Slow down gradually, but slower at the beginning
            if (spinTime > 1.0f) // Only start slowing down after 1 second
            {
                currentSpinSpeed *= spinSlowdownRate;
            }
            
            spinTime += Time.deltaTime;
            yield return null;
        }

        // Add a final slow deceleration for dramatic effect
        float finalSlowdownTime = 1.0f;
        float finalSlowdownTimer = 0f;
        float startingFinalSpeed = currentSpinSpeed;
        
        while (finalSlowdownTimer < finalSlowdownTime)
        {
            float slowdownFactor = 1.0f - (finalSlowdownTimer / finalSlowdownTime);
            float currentFinalSpeed = startingFinalSpeed * slowdownFactor;
            
            if (wheelImage != null)
            {
                wheelImage.Rotate(0, 0, currentFinalSpeed * Time.deltaTime);
            }
            
            finalSlowdownTimer += Time.deltaTime;
            yield return null;
        }

        // Ensure we had at least one trigger hit
       // if (lastHitTriggerValue == 0)
       // {
         //   lastHitTriggerValue = Random.Range(1, 7);
        //    Debug.LogWarning("No trigger was hit during spin, using random value: " + lastHitTriggerValue);
        //}

        // Wheel has stopped
        isSpinning = false;

        // Return the result
        onComplete?.Invoke(lastHitTriggerValue);
    }
}