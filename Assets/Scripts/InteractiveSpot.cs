using UnityEngine;

public class InteractiveSpot : MonoBehaviour
{
    public int spotIndex;
    private bool _isInteractable = false;
    private Material originalMaterial;
    private Renderer spotRenderer;

    [SerializeField] public Material highlightMaterial;
    [SerializeField] public Material normalMaterial;

    public bool isInteractable => _isInteractable;

    private void Start()
    {
        spotRenderer = GetComponent<Renderer>();
        if (spotRenderer != null)
        {
            originalMaterial = spotRenderer.material;
        }
        
        // Убеждаемся, что есть коллайдер
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
        
        // Обновляем originalMaterial каждый раз при изменении состояния
        if (spotRenderer != null)
        {
            originalMaterial = spotRenderer.material;
        }
        
        Debug.Log($"Spot {spotIndex} interactable set to: {_isInteractable}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"Mouse clicked on spot {spotIndex}");
        
        if (_isInteractable)
        {
            Debug.Log($"Clicked on spot {spotIndex}, isInteractable: {_isInteractable}");
            TwoPlayerDiceGameBot gameBot = FindObjectOfType<TwoPlayerDiceGameBot>();
            if (gameBot != null)
            {
                gameBot.OnSpotClicked(spotIndex);
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
}