using UnityEngine;

public class ArrowClickHandler : MonoBehaviour
{
    private int spotIndex;
    private TwoPlayerDiceGame gameController;
    private TwoPlayerDiceGameBot botGameController;

    public void Initialize(int index, TwoPlayerDiceGame controller)
    {
        spotIndex = index;
        gameController = controller;
        botGameController = null;
        Debug.Log($"ArrowClickHandler initialized for spot {spotIndex} with TwoPlayerDiceGame");
    }

    public void Initialize(int index, TwoPlayerDiceGameBot controller)
    {
        spotIndex = index;
        botGameController = controller;
        gameController = null;
        Debug.Log($"ArrowClickHandler initialized for spot {spotIndex} with TwoPlayerDiceGameBot");
    }

    private void OnMouseDown()
    {
        Debug.Log($"Arrow clicked for spot {spotIndex}");
        
        if (gameController != null)
        {
            gameController.OnSpotClicked(spotIndex);
        }
        else if (botGameController != null)
        {
            botGameController.OnSpotClicked(spotIndex);
        }
        else
        {
            Debug.LogError($"No game controller found for arrow at spot {spotIndex}!");
        }
    }
}