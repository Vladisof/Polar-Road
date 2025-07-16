using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
  public TextMeshProUGUI moneyText;
  private float money;

  private void Start()
  {
    money = PlayerPrefs.GetFloat("Money", 10000);
    UpdateMoneyText();
  }

  public void AddMoney(float amount)
  {
    money += amount;
    SaveMoney();
    UpdateMoneyText();
  }

  public bool SubtractMoney(int amount)
  {
    if (money >= amount)
    {
      money -= amount;
      SaveMoney();
      UpdateMoneyText();
      return true;
    }
    else
    {
      Debug.LogWarning("Недостаточно монет для выполнения операции.");
      return false;
    }
  }

  private void UpdateMoneyText()
  {
    moneyText.text = "" + money.ToString("F0");
  }

  private void SaveMoney()
  {
    PlayerPrefs.SetFloat("Money", money);
    PlayerPrefs.Save();
    if (money > 10000)
    {
      PlayerPrefs.SetInt("10000Money", 1);
      PlayerPrefs.Save();
    }

    if (money > 50000)
    {
      PlayerPrefs.SetInt("50000Money", 1);
      PlayerPrefs.Save();
    }

    if (money > 100000)
    {
      PlayerPrefs.SetInt("100000Money", 1);
      PlayerPrefs.Save();
    }
  }

  private void OnApplicationQuit()
  {
    SaveMoney();
  }
}