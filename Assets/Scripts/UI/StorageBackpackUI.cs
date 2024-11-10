using System.Collections.Generic;
using UnityEngine;

public class StorageBackpackUI : BaseStorageUI
{
    protected override List<Item> GetItems()
    {
        // ������� �������� � ������� ������
        return Player.Instance.backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // ������� �������� ������ ������� ������
        return Player.Instance.backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // ����������, �� � ��������� ���� �� ����� ��� ��������
        if (Player.Instance.storage.HasEnoughSpace(item.Size))
        {
            // ��������� ������� � �������
            if (Player.Instance.backpack.RemoveItem(item))
            {
                // ������ ������� �� �����
                Player.Instance.storage.AddItem(item);
                Debug.Log($"������� '{item.Name}' ��������� �� �����.");

                UpdateInventoryUI();  // ��������� ��������� ���� ���������� ��������
            }
            else
            {
                Debug.LogWarning($"�� ������� ���������� ������� '{item.Name}' � �������.");
            }
        }
        else
        {
            Debug.LogWarning("����������� ���� �� ����� ��� ���������� ��������.");
        }
    }
}