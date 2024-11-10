using System.Collections.Generic;
using UnityEngine;

public class GameBackpackUI : BaseStorageUI
{
    protected override List<Item> GetItems()
    {
        // ������� �������� � ������� ������
        return Player.Instance.backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // ������� �������� ������ �������
        return Player.Instance.backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // ����� ��� ��������� ��������
        if (Player.Instance.backpack.RemoveItem(item))
        {
            Debug.Log($"������� '{item.Name}' �������� � �������.");
            UpdateInventoryUI();  // ��������� ��������� ���� ��������� ��������
        }
        else
        {
            Debug.LogWarning($"�� ������� �������� ������� '{item.Name}'.");
        }
    }
}