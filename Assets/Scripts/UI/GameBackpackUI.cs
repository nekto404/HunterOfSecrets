using System.Collections.Generic;
using UnityEngine;

public class GameBackpackUI : BaseStorageUI
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
        // ����� ��� ��������� ��������
        if (Player.Instance.Backpack.RemoveItem(item))
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