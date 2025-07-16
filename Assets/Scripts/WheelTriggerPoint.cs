using UnityEngine;

public class WheelTriggerPoint : MonoBehaviour
{
    public int triggerValue; // Number value (1-6)
    [SerializeField] private WheelController wheelController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering is your wheel marker
        if (other.CompareTag("WheelMarker"))
        {
            if (wheelController != null)
            {
                Debug.Log("Trigger hit! " + triggerValue);
                wheelController.RegisterTriggerHit(triggerValue);
            }
        }
    }
}