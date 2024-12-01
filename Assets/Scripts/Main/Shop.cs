using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemPool
{
    public List<Item> items = new List<Item>(); // ������ �������� � ����������� ���
}

public class Shop : MonoBehaviour
{
    [Header("Shop Configuration")]
    public List<ItemPool> itemPools = new List<ItemPool>(); // ������ ����, ����� ������� � ���������
    public List<Item> availableItems = new List<Item>();    // ��������, ������� ��� �������
    public int backpackUpgradeCost;                         // ������� ���������� �������
    public int refreshCost = 10;                            // ������� ��������� ������

    private void Start()
    {
        // ���������� ���� ��������, ���� ���� �� �� ��������
        for (int i = 0; i < 5; i++)
        {
            if (itemPools.Count <= i || itemPools[i] == null)
            {
                itemPools.Add(new ItemPool());
            }
        }
    }


    // ����� ��� ��������� ������ � �������, ������ 3 ���������� ������ � ����
    public void UpdateAvailableItems()
    {
        availableItems.Clear();

        // ������� ������ � ��� ���� ��� ������
        List<Item> selectedPool = new List<Item>();
        foreach (var pool in itemPools)
        {
            selectedPool.AddRange(pool.items);
        }

        // �������� 3 ���������� �������� ��� ��������
        for (int i = 0; i < 3; i++)
        {
            if (selectedPool.Count > 0)
            {
                int randomIndex = Random.Range(0, selectedPool.Count);
                availableItems.Add(selectedPool[randomIndex]);
                selectedPool.RemoveAt(randomIndex);
            }
        }

        // ��������� ������� ���������� �������
        backpackUpgradeCost = CalculateBackpackUpgradeCost(Player.Instance.backpack.Size);
        Debug.Log("Shop items updated. Available items count: " + availableItems.Count);
    }

    // ����� ��� ���������� ������� ���������� �������
    private int CalculateBackpackUpgradeCost(int currentBackpackSize)
    {
        return currentBackpackSize; // ������� ����������: 10 ����� �� ������ �����
    }

    // ����� ��� ������� ��������
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

    // ����� ��� ���������� �������
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

    // ����� ��� ��������� ������ � �������
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