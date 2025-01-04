using System.Collections.Generic;
using UnityEngine;

public class StorageBackpackUI : BaseStorageUI
{
    private void OnEnable()
    {
        // ϳ��������� �� ���� ��������� ������
        Player.Instance.Backpack.OnStorageUpdated += UpdateInventoryUI;
        UpdateInventoryUI(); // ��������� ��������� ��� ���������
    }

    private void OnDisable()
    {
        // ³��������� �� ��䳿 ��� ��������� UI
        Player.Instance.Backpack.OnStorageUpdated -= UpdateInventoryUI;
    }
    protected override List<Item> GetItems()
    {
        // ������� �������� � ������� ������
        return Player.Instance.Backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // ������� �������� ������� ������� ������
        return Player.Instance.Backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // ����������, �� � ��������� ���� �� ����� ��� ��������
        if (Player.Instance.Storage.HasEnoughSpace(item.Size))
        {
            // ��������� ������� � �������
            if (Player.Instance.Backpack.RemoveItem(item))
            {
                // ������ ������� �� �����
                Player.Instance.Storage.AddItem(item);
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