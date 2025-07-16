using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveToShop : MonoBehaviour
{
    [SerializeField] private GameObject shopCameraPlace;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Button ShopButtons;
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Shop;
    
    private void Start()
    {
        ShopButtons.onClick.AddListener(BackToShop);
    }
    public void BackToShop()
    {
        Menu.SetActive(false);
        StartCoroutine(MoveCameraToShop());
    }
    
    private IEnumerator MoveCameraToShop()
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        Vector3 targetPos = shopCameraPlace.transform.position;
        Quaternion targetRot = shopCameraPlace.transform.rotation;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 0.5f; // Зменшити швидкість переходу
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, time);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, time);
            yield return null;
        }
        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;
        Menu.SetActive(false);
        Shop.SetActive(true);
    }

}
