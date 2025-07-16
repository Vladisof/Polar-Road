using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rename this class to BoardSetup
public class BoardSetup : MonoBehaviour
{
    [Header("Board Generation")]
    [SerializeField] private GameObject spotPrefab;
    [SerializeField] private int totalSpots = 20;
    [SerializeField] private float boardRadius = 10f;
    
    [Header("Board Styling")]
    [SerializeField] private Material normalSpotMaterial;
    [SerializeField] private Material specialSpotMaterial;
    [SerializeField] private Material startFinishMaterial;
    [SerializeField] private int specialSpotFrequency = 5; // e.g., every 5th spot is special
    
    private List<Transform> boardSpots = new List<Transform>();
    
    private void Start()
    {
        GenerateBoard();
    }
    
    private void GenerateBoard()
    {
        if (spotPrefab == null)
        {
            Debug.LogError("Spot prefab is not assigned!");
            return;
        }
        
        // Clear old board spots if they exist
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        boardSpots.Clear();
        
        // Generate spots in a circle
        for (int i = 0; i < totalSpots; i++)
        {
            // Calculate position on circle
            float angle = i * 360f / totalSpots;
            float radians = angle * Mathf.Deg2Rad;
            Vector3 position = new Vector3(
                boardRadius * Mathf.Sin(radians),
                0f,
                boardRadius * Mathf.Cos(radians)
            );
            
            // Create spot
            GameObject spot = Instantiate(spotPrefab, position, Quaternion.identity, transform);
            spot.name = "Spot_" + i;
            boardSpots.Add(spot.transform);
            
            // Set material based on spot type
            Renderer renderer = spot.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (i == 0) // Start/Finish spot
                {
                    if (startFinishMaterial != null)
                        renderer.material = startFinishMaterial;
                }
                else if (i % specialSpotFrequency == 0) // Special spots
                {
                    if (specialSpotMaterial != null)
                        renderer.material = specialSpotMaterial;
                }
                else // Normal spots
                {
                    if (normalSpotMaterial != null)
                        renderer.material = normalSpotMaterial;
                }
            }
        }
        
        // Find and update references to these spots in game controllers
        UpdateGameControllersWithSpots();
    }
    
    private void UpdateGameControllersWithSpots()
    {
        // Update single player controller
        DiceRollAndMovement singlePlayerController = FindObjectOfType<DiceRollAndMovement>();
        if (singlePlayerController != null)
        {
            // Get the field via reflection to circumvent access protection
            System.Reflection.FieldInfo fieldInfo = typeof(DiceRollAndMovement).GetField("boardSpots", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
            if (fieldInfo != null)
                fieldInfo.SetValue(singlePlayerController, boardSpots.ToArray());
            else
                Debug.LogError("Could not find boardSpots field in DiceRollAndMovement");
        }
        
        // Update two player controller
        TwoPlayerDiceGame twoPlayerController = FindObjectOfType<TwoPlayerDiceGame>();
        if (twoPlayerController != null)
        {
            // Get the field via reflection to circumvent access protection
            System.Reflection.FieldInfo fieldInfo = typeof(TwoPlayerDiceGame).GetField("boardSpots", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
            if (fieldInfo != null)
                fieldInfo.SetValue(twoPlayerController, boardSpots.ToArray());
            else
                Debug.LogError("Could not find boardSpots field in TwoPlayerDiceGame");
        }
    }
    
    // Public method to get board spots
    public Transform[] GetBoardSpots()
    {
        return boardSpots.ToArray();
    }
}
