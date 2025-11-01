using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public GameObject[] shopItems;
    public Button nextButton;
    public Button prevButton;
    public Button buyButton;
    public Button backButton;
    public GameObject MenuButtons;
    public GameObject Menu;
    public Camera mainCamera;
    public Transform ShopCameraPlace;
    public MoneyController moneyController;
    public TextMeshProUGUI itemDescriptionText;
    public string[] itemDescriptions;
    public TextMeshProUGUI priceText;
    public int[] itemPrices;
    public SelectedItemLoader selectedItemLoader1;
    public Sprite selectedSprite;
    public Sprite unselectedSprite;
    public Sprite buySprite;
    
    public float transitionSpeed = 0.3f;
    private int currentIndex = 0;
    private bool[] purchasedItems;
    private int selectedItemIndex = -1; // Індекс вибраного об'єкта

    void Start()
    {
        LoadPurchasedItems();
        selectedItemIndex = PlayerPrefs.GetInt("SelectedItem", 0); // Завантажуємо вибраний об'єкт
        UpdateShopDisplay();

        nextButton.onClick.AddListener(NextItem);
        prevButton.onClick.AddListener(PreviousItem);
        buyButton.onClick.AddListener(HandleBuyOrSelect);
        backButton.onClick.AddListener(BackToMenu);
    }

    public void BackToMenu()
    {
        selectedItemLoader1.UpdatePurchasedItems();
        MenuButtons.SetActive(true);
        Menu.SetActive(true);
        gameObject.SetActive(false);
    }

    private IEnumerator MoveCameraToShop()
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        Vector3 targetPos = ShopCameraPlace.position;
        Quaternion targetRot = ShopCameraPlace.rotation;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * transitionSpeed;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, time);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, time);
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;

        MenuButtons.SetActive(true);
        Menu.SetActive(true);
        gameObject.SetActive(false);
    }

    void NextItem()
    {
        shopItems[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % shopItems.Length;
        shopItems[currentIndex].SetActive(true);
        UpdateShopDisplay();
    }

    void PreviousItem()
    {
        shopItems[currentIndex].SetActive(false);
        currentIndex = (currentIndex - 1 + shopItems.Length) % shopItems.Length;
        shopItems[currentIndex].SetActive(true);
        UpdateShopDisplay();
    }

    void HandleBuyOrSelect()
    {
        if (!purchasedItems[currentIndex])
        {
            BuyItem();
        }
        else
        {
            SelectItem();
        }
    }

    void BuyItem()
    {
        if (moneyController.SubtractMoney(itemPrices[currentIndex]))
        {
            purchasedItems[currentIndex] = true;
            SavePurchasedItems();
            UpdateShopDisplay();
            Debug.Log("Куплено: " + shopItems[currentIndex].name);
        }
    }

    void SelectItem()
    {
        selectedItemIndex = currentIndex;
        PlayerPrefs.SetInt("SelectedItem", selectedItemIndex);
        PlayerPrefs.Save();
        UpdateShopDisplay();
        Debug.Log("Вибрано: " + shopItems[currentIndex].name);
    }

    void UpdateShopDisplay()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i].SetActive(i == currentIndex);
        }
        UpdateBuyButton();
        UpdateDescriptionText();
        UpdatePriceText();
    }

    void UpdatePriceText()
    {
        if (priceText != null && itemPrices.Length > currentIndex)
        {
            priceText.text = "" + itemPrices[currentIndex].ToString() + "";
        }
    }
    
    void UpdateBuyButton()
    {
        if (!purchasedItems[currentIndex])
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
        }
        else if (selectedItemIndex == currentIndex)
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
        }
        else
        {
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
        }
    }

    void UpdateDescriptionText()
    {
        if (itemDescriptionText != null && itemDescriptions.Length > currentIndex)
        {
            itemDescriptionText.text = itemDescriptions[currentIndex];
        }
    }

    void LoadPurchasedItems()
    {
        purchasedItems = new bool[shopItems.Length];
        purchasedItems[0] = true; // Перший об'єкт завжди куплений
        
        for (int i = 1; i < shopItems.Length; i++)
        {
            purchasedItems[i] = PlayerPrefs.GetInt("ShopItem_" + i, 0) == 1;
        }
    }

    void SavePurchasedItems()
    {
        for (int i = 1; i < shopItems.Length; i++)
        {
            PlayerPrefs.SetInt("ShopItem_" + i, purchasedItems[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
}
