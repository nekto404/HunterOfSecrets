using System.Collections.Generic;
using UnityEngine;

public class StorageUI : BaseStorageUI
{
    protected override int GetBackpackSize()
    {
        // ������ ����� ������, ���� ��������������� ��� �����������
        return Player.Instance.storage.Size;
    }

    protected override List<Item> GetItems()
    {
        // �������� �� �������� � ������ ������
        return Player.Instance.storage.GetItems();
    }

    protected override void OnItemAction(Item item)
    {
        // ����������, �� � ��������� ���� � ������� ��� ��������
        if (Player.Instance.backpack.HasEnoughSpace(item.Size))
        {
            // ���� ���� ���������, ��������� ������� � ������ �� ������ ���� � ������
            if (Player.Instance.storage.RemoveItem(item))
            {
                if (Player.Instance.backpack.AddItem(item))
                {
                    Debug.Log($"������� '{item.Name}' ��������� � ������ � ������.");
                    UpdateInventoryUI();
                }
                else
                {
                    // ��������� ������� ����� �� �����, ���� ��������� �� ������� �� �������
                    Player.Instance.storage.AddItem(item);
                    Debug.LogWarning("�� ������� ���������� ������� � ������.");
                }
            }
        }
        else
        {
            Debug.LogWarning("����������� ���� � ������� ��� ����� ��������.");
        }
    }
}