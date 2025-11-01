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
        Shop.SetActive(true);
    }

}
