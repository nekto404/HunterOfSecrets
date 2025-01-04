using System.Collections.Generic;
using UnityEngine;

public class ShopBackpackUI : BaseStorageUI
{
    private void OnEnable()
    {
        // ϳ��������� �� ���� ��������� ������
        Player.Instance.Backpack.OnStorageUpdated += UpdateInventoryUI;
        UpdateInventoryUI(); // ��������� ��������� ��� ���������
    }

    private void OnDisable()
    {
        // ³��������� �� ��䳿 ��� �������� UI
        Player.Instance.Backpack.OnStorageUpdated -= UpdateInventoryUI;
    }
    protected override List<Item> GetItems()
    {
        // ������� �������� � ������� ������
        return Player.Instance.Backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // ������� �������� ������ �������
        return Player.Instance.Backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // ����� ������� ��������
        if (Player.Instance.Backpack.RemoveItem(item))
        {
            Player.Instance.AddCoins(item.Price);
            Debug.Log($"������� '{item.Name}' ������� �� {item.Price} �����. ����� ������: {Player.Instance.Coins} �����.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"�� ������� ������� ������� '{item.Name}'.");
        }
    }
}