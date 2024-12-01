using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemPool
{
    public List<Item> items = new List<Item>(); // Список предметів у конкретному пулі
}

public class Shop : MonoBehaviour
{
    [Header("Shop Configuration")]
    public List<ItemPool> itemPools = new List<ItemPool>(); // Список пулів, тепер видимий у інспекторі
    public List<Item> availableItems = new List<Item>();    // Предмети, доступні для покупки
    public int backpackUpgradeCost;                         // Вартість покращення рюкзака
    public int refreshCost = 10;                            // Вартість оновлення товарів

    private void Start()
    {
        // Ініціалізуємо пули предметів, якщо вони ще не заповнені
        for (int i = 0; i < 5; i++)
        {
            if (itemPools.Count <= i || itemPools[i] == null)
            {
                itemPools.Add(new ItemPool());
            }
        }
    }


    // Метод для оновлення товарів у магазині, вибирає 3 випадкових айтеми з пулів
    public void UpdateAvailableItems()
    {
        availableItems.Clear();

        // Збираємо айтеми з усіх пулів для вибору
        List<Item> selectedPool = new List<Item>();
        foreach (var pool in itemPools)
        {
            selectedPool.AddRange(pool.items);
        }

        // Вибираємо 3 випадкових предмети для магазину
        for (int i = 0; i < 3; i++)
        {
            if (selectedPool.Count > 0)
            {
                int randomIndex = Random.Range(0, selectedPool.Count);
                availableItems.Add(selectedPool[randomIndex]);
                selectedPool.RemoveAt(randomIndex);
            }
        }

        // Оновлюємо вартість покращення рюкзака
        backpackUpgradeCost = CalculateBackpackUpgradeCost(Player.Instance.backpack.Size);
        Debug.Log("Shop items updated. Available items count: " + availableItems.Count);
    }

    // Метод для обчислення вартості покращення рюкзака
    private int CalculateBackpackUpgradeCost(int currentBackpackSize)
    {
        return currentBackpackSize; // Приклад розрахунку: 10 монет за кожний розмір
    }

    // Метод для покупки предмету
    public bool BuyItem(Item item)
    {
        if (Player.Instance.CanAfford(item.Price))
        {
            if (Player.Instance.backpack.AddItem(item))
            {
                Player.Instance.SpendCoins(item.Price);
                availableItems.Remove(item);
                Debug.Log("Item purchased: " + item.Name);
                return true;
            }
            else
            {
                Debug.LogWarning("Not enough space in the backpack for the item: " + item.Name);
            }
        }
        else
        {
            Debug.LogWarning("Not enough coins to buy the item: " + item.Name);
        }
        return false;
    }

    // Метод для покращення рюкзака
    public bool UpgradeBackpack()
    {
        if (Player.Instance.CanAfford(backpackUpgradeCost))
        {
            Player.Instance.SpendCoins(backpackUpgradeCost);
            Player.Instance.backpack.Size++;
            Debug.Log("Backpack upgraded. New size: " + Player.Instance.backpack.Size);
            backpackUpgradeCost = CalculateBackpackUpgradeCost(Player.Instance.backpack.Size);
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough coins to upgrade the backpack.");
        }
        return false;
    }

    // Метод для оновлення товарів у магазині
    public bool RefreshShop()
    {
        if (Player.Instance.CanAfford(refreshCost))
        {
            Player.Instance.SpendCoins(refreshCost);
            UpdateAvailableItems();
            Debug.Log("Shop items refreshed.");
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough coins to refresh the shop.");
        }
        return false;
    }
}