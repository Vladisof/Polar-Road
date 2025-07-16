using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedItemLoader : MonoBehaviour
{
    public GameObject[] items; // Масив об'єктів на сцені

    void Start()
    {
        int selectedItemIndex = PlayerPrefs.GetInt("SelectedItem", 0);

        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(i == selectedItemIndex);
        }
    }
    
    public void UpdatePurchasedItems()
    {
        int selectedItemIndex = PlayerPrefs.GetInt("SelectedItem", 0);

        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(i == selectedItemIndex);
        }
    }
}
