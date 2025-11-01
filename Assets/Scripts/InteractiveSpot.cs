using UnityEngine;

public class InteractiveSpot : MonoBehaviour
{
    public int spotIndex;
    private bool _isInteractable = false;
    private Material originalMaterial;
    private Renderer spotRenderer;
    
    [SerializeField] public Material highlightMaterial;
    [SerializeField] public Material normalMaterial;
    [SerializeField] private GameObject arrowObject; // Reference to arrow child object

    public bool isInteractable => _isInteractable;

    private void Start()
    {
        spotRenderer = GetComponent<Renderer>();
        if (spotRenderer != null)
        {
            originalMaterial = spotRenderer.material;
        }
        
        // Find arrow object if not assigned
        if (arrowObject == null)
        {
            Transform arrowTransform = transform.Find("Arrow");
            if (arrowTransform != null)
            {
                arrowObject = arrowTransform.gameObject;
            }
        }
        // Automatically find the arrow object in children
        if (transform.childCount > 0)
        {
            arrowObject = transform.GetChild(0).gameObject;
            if (arrowObject != null)
            {
                arrowObject.SetActive(false); // Initially hidden
                Debug.Log($"Auto-found arrow for spot {spotIndex}: {arrowObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"No child objects found for spot {spotIndex}");
        }
        
        // Setup arrow click handler if arrow exists
        if (arrowObject != null)
        {
            SetupArrowClickHandler();
        }
        // Ensure collider exists
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    
    public void Initialize(int index)
    {
        spotIndex = index;
        if (arrowObject != null)
        {
            Debug.Log($"Auto-found arrow for spot {spotIndex}: {arrowObject.name}");
            SetupArrowClickHandler();
        }
    }
    
    private void SetupArrowClickHandler()
    {
        if (arrowObject != null)
        {
            ArrowClickHandler clickHandler = arrowObject.GetComponent<ArrowClickHandler>();
            if (clickHandler == null)
            {
                clickHandler = arrowObject.AddComponent<ArrowClickHandler>();
            }

            // Try to find TwoPlayerDiceGame first
            TwoPlayerDiceGame twoPlayerGame = FindObjectOfType<TwoPlayerDiceGame>();
            if (twoPlayerGame != null)
            {
                clickHandler.Initialize(spotIndex, twoPlayerGame);
                return;
            }

            // If not found, try BotTwoPlayerDiceGame
            TwoPlayerDiceGameBot botGame = FindObjectOfType<TwoPlayerDiceGameBot>();
            if (botGame != null)
            {
                clickHandler.Initialize(spotIndex, botGame);
                return;
            }

            Debug.LogError("No compatible game controller found!");
        }
    }
    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable; // ЭТО ДОЛЖНО БЫТЬ В НАЧАЛЕ

        // Show/hide arrow based on interactable state
        if (arrowObject != null)
        {
            arrowObject.SetActive(interactable);
        }

        // Change material only for highlighting, preserve original colors
        if (spotRenderer != null)
        {
            if (interactable && highlightMaterial != null)
            {
                // Store current material before highlighting
                if (originalMaterial == null)
                    originalMaterial = spotRenderer.material;
                spotRenderer.material = highlightMaterial;
                spotRenderer.material = originalMaterial;
            }
            else if (!interactable && originalMaterial != null)
            {
                // Restore original material (preserves assigned colors)
                spotRenderer.material = originalMaterial;
            }
        }

       // Debug.Log($"Spot {spotIndex} interactable set to: {_isInteractable}, arrow active: {(arrowObject != null ? arrowObject.activeSelf : false)}");
    }

    private void OnMouseDown()
    {
        //Debug.Log($"Mouse clicked on spot {spotIndex}");
        
        if (_isInteractable)
        {
            //Debug.Log($"Clicked on spot {spotIndex}, isInteractable: {_isInteractable}");
            
            // First try to find TwoPlayerDiceGameBot
            TwoPlayerDiceGameBot gameBot = FindObjectOfType<TwoPlayerDiceGameBot>();
            if (gameBot != null)
            {
                gameBot.OnSpotClicked(spotIndex);
                return;
            }
            
            // If bot game not found, try regular two player game
            TwoPlayerDiceGame regularGame = FindObjectOfType<TwoPlayerDiceGame>();
            if (regularGame != null)
            {
                regularGame.OnSpotClicked(spotIndex);
            }
        }
        else
        {
            Debug.Log($"Spot {spotIndex} is not interactable");
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log($"Mouse entered spot {spotIndex}, interactable: {_isInteractable}");
        
        if (_isInteractable && highlightMaterial != null && spotRenderer != null)
        {
            spotRenderer.material = highlightMaterial;
        }
    }

    private void OnMouseExit()
    {
        Debug.Log($"Mouse exited spot {spotIndex}");
        
        if (_isInteractable && originalMaterial != null && spotRenderer != null)
        {
            spotRenderer.material = originalMaterial;
        }
    }

    // Method to manually set arrow reference if needed
    public void SetArrowObject(GameObject arrow)
    {
        arrowObject = arrow;
        if (arrowObject != null)
        {
            arrowObject.SetActive(_isInteractable);
        }
    }
}