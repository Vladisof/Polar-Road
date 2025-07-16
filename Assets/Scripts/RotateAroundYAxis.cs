using UnityEngine;

public class RotateAroundY : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30f; // швидкість обертання у градусах за секунду
    
    // Update викликається один раз на кадр
    void Update()
    {
        // Обертання об'єкта навколо осі Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}