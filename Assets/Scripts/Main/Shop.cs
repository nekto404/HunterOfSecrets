using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<Item>[] itemPools = new List<Item>[5]; // Пули предметів для кожного рівня (задати в інспекторі)
    public List<Item> availableItems = new List<Item>(); // Предмети, доступні для покупки
    public int backpackUpgradeCost;                      // Вартість покращення рюкзака
    public int refreshCost = 10;                         // Вартість оновлення товарів

    private void Start()
    {
        // Ініціалізуємо пули (якщо вони не задані в інспекторі)
        for (int i = 0; i < itemPools.Length; i++)
        {
            if (itemPools[i] == null)
            {
                itemPools[i] = new List<Item>();
            }
        }

        // Заповнюємо початковий набір предметів
        UpdateAvailableItems();
    }

    // Метод для оновлення товарів у магазині, вибирає 3 випадкових айтеми з пулів
    public void UpdateAvailableItems()
    {
        availableItems.Clear();

        // Збираємо айтеми з усіх пулів для вибору
        List<Item> selectedPool = new List<Item>();
        foreach (var pool in itemPools)
        {
            selectedPool.AddRange(pool);
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
        return currentBackpackSize * 10; // Приклад розрахунку: 10 монет за кожний розмір
    }

    // Метод для покупки предмету
    public bool BuyItem(Item item)
    {
        // Перевіряємо наявність монет та достатньо місця в рюкзаку перед спробою покупки
        if (!Player.Instance.CanAfford(item.Price))
        {
            Debug.LogWarning("Not enough coins to buy the item: " + item.Name);
            return false;
        }

        if (!Player.Instance.backpack.HasEnoughSpace(item.Size))
        {
            Debug.LogWarning("Not enough space in the backpack for the item: " + item.Name);
            return false;
        }

        // Якщо перевірка пройдена, виконуємо покупку
        if (Player.Instance.SpendCoins(item.Price))
        {
            Player.Instance.backpack.AddItem(item);
            availableItems.Remove(item);
            Debug.Log("Item purchased: " + item.Name);
            return true;
        }

        return false;
    }

    public bool UpgradeBackpack()
    {
        if (Player.Instance.SpendCoins(backpackUpgradeCost))
        {
            Player.Instance.backpack.Size++;
            Debug.Log("Backpack upgraded. New size: " + Player.Instance.backpack.Size);
            backpackUpgradeCost = CalculateBackpackUpgradeCost(Player.Instance.backpack.Size);
            return true;
        }
        return false;
    }

    // Метод для оновлення товарів у магазині
    public bool RefreshShop()
    {
        if (Player.Instance.SpendCoins(refreshCost))
        {
            UpdateAvailableItems();
            Debug.Log("Shop items refreshed.");
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough coins to refresh the shop.");
            return false;
        }
    }
}