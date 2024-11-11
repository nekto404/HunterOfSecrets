using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<Item>[] itemPools = new List<Item>[5]; // ���� �������� ��� ������� ���� (������ � ���������)
    public List<Item> availableItems = new List<Item>(); // ��������, ������� ��� �������
    public int backpackUpgradeCost;                      // ������� ���������� �������
    public int refreshCost = 10;                         // ������� ��������� ������

    private void Start()
    {
        // ���������� ���� (���� ���� �� ����� � ���������)
        for (int i = 0; i < itemPools.Length; i++)
        {
            if (itemPools[i] == null)
            {
                itemPools[i] = new List<Item>();
            }
        }

        // ���������� ���������� ���� ��������
        UpdateAvailableItems();
    }

    // ����� ��� ��������� ������ � �������, ������ 3 ���������� ������ � ����
    public void UpdateAvailableItems()
    {
        availableItems.Clear();

        // ������� ������ � ��� ���� ��� ������
        List<Item> selectedPool = new List<Item>();
        foreach (var pool in itemPools)
        {
            selectedPool.AddRange(pool);
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
        return currentBackpackSize * 10; // ������� ����������: 10 ����� �� ������ �����
    }

    // ����� ��� ������� ��������
    public bool BuyItem(Item item)
    {
        // ���������� �������� ����� �� ��������� ���� � ������� ����� ������� �������
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

        // ���� �������� ��������, �������� �������
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

    // ����� ��� ��������� ������ � �������
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