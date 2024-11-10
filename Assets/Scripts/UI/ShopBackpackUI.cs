using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBackpackUI : MonoBehaviour
{
    public ScrollController scrollController;   // ��������� ScrollController ��� ��������� ����������
    public GameObject itemPrefab;               // ������ UI-�������� ��� ��������
    public GameObject emptySlotPrefab;          // ������ UI-�������� ��� ���������� �����

    private List<GameObject> itemUIElements = new List<GameObject>();

    private void Start()
    {
        UpdateInventoryUI();  
    }

    private void OnEnable()
    {
        UpdateInventoryUI();  
    }

    public void UpdateInventoryUI()
    {
        ClearInventoryUI();  // ������� ���� ��������

        // �������� �� �������� � ������� ������
        List<Item> items = Player.Instance.backpack.GetItems();
        int totalUsedCapacity = 0;

        foreach (var item in items)
        {
            // ��������� ����� UI-������� ��� ��������
            GameObject newItemUI = Instantiate(itemPrefab, scrollController.content);
            newItemUI.GetComponentInChildren<Text>().text = item.Name;  // ������������ ��'� ��������

            // ����������� ����� �������� �������� �� ������ ��������
            RectTransform itemRect = newItemUI.GetComponent<RectTransform>();
            float itemDisplayWidth = itemRect.sizeDelta.x * item.Size;
            itemRect.sizeDelta = new Vector2(itemDisplayWidth, itemRect.sizeDelta.y);

            // ����������� ������ "�������"
            Button sellButton = newItemUI.GetComponentInChildren<Button>();
            sellButton.onClick.AddListener(() => SellItem(item));

            itemUIElements.Add(newItemUI);  // ������ ������� �� ������
            totalUsedCapacity += item.Size; // ������ ����� �������� �� ���������� ������������� ��������
        }

        // ���������� �������� ������� ���� �� ������ ������ �����
        int emptySlotsCount = Mathf.Max(0, Player.Instance.backpack.Size - totalUsedCapacity);
        for (int i = 0; i < emptySlotsCount; i++)
        {
            GameObject emptySlotUI = Instantiate(emptySlotPrefab, scrollController.content);
            itemUIElements.Add(emptySlotUI);  // ������ ������� ���� �� ������
        }

        // ϳ��� ��������� ������� ���������� ��������� �������
        scrollController.Initialize();
    }

    private void SellItem(Item item)
    {
        // ��������� ������� � �������
        if (Player.Instance.backpack.RemoveItem(item))
        {
            // ������ ������� �������� �� ������� ������
            Player.Instance.coins += item.Price;
            Debug.Log($"������� '{item.Name}' ������� �� {item.Price} �����. ����� ������: {Player.Instance.coins} �����.");

            // ��������� ��������� ���� �������
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"�� ������� ������� ������� '{item.Name}'.");
        }
    }

    private void ClearInventoryUI()
    {
        // ��������� �� ���� �������� � ��������
        foreach (var itemUI in itemUIElements)
        {
            Destroy(itemUI);
        }
        itemUIElements.Clear();  // ������� ������
    }
}