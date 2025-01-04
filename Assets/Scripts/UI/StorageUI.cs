using System.Collections.Generic;
using UnityEngine;

public class StorageUI : BaseStorageUI
{
    private void OnEnable()
    {
        // ϳ��������� �� ���� ��������� ������
        Player.Instance.Storage.OnStorageUpdated += UpdateInventoryUI;
        UpdateInventoryUI(); // ��������� ��������� ��� ���������
    }

    private void OnDisable()
    {
        // ³��������� �� ��䳿 ��� �������� UI
        Player.Instance.Storage.OnStorageUpdated -= UpdateInventoryUI;
    }
    protected override int GetBackpackSize()
    {
        // ������ ����� ������, ���� ��������������� ��� �����������
        return Player.Instance.Storage.Size;
    }

    protected override List<Item> GetItems()
    {
        // �������� �� �������� � ������ ������
        return Player.Instance.Storage.GetItems();
    }

    protected override void OnItemAction(Item item)
    {
        // ����������, �� � ��������� ���� � ������� ��� ��������
        if (Player.Instance.Backpack.HasEnoughSpace(item.Size))
        {
            // ���� ���� ���������, ��������� ������� � ������ �� ������ ���� � ������
            if (Player.Instance.Storage.RemoveItem(item))
            {
                if (Player.Instance.Backpack.AddItem(item))
                {
                    Debug.Log($"������� '{item.Name}' ��������� � ������ � ������.");
                    UpdateInventoryUI();
                }
                else
                {
                    // ��������� ������� ����� �� �����, ���� ��������� �� ������� �� �������
                    Player.Instance.Storage.AddItem(item);
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