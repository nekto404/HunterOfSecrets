using System.Collections.Generic;
using UnityEngine;

public class ShopBackpackUI : BaseStorageUI
{
    protected override List<Item> GetItems()
    {
        // ������� �������� � ������� ������
        return Player.Instance.backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // ������� �������� ������� �������
        return Player.Instance.backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // ����� ������� ��������
        if (Player.Instance.backpack.RemoveItem(item))
        {
            Player.Instance.coins += item.Price;
            Debug.Log($"������� '{item.Name}' ������� �� {item.Price} �����. ����� ������: {Player.Instance.coins} �����.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"�� ������� ������� ������� '{item.Name}'.");
        }
    }
}