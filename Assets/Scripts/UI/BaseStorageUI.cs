using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseStorageUI : MonoBehaviour
{
    public ScrollController scrollController;    // ��������� ScrollController ��� ��������� ����������
    public GameObject itemPrefab;                // ������ UI-�������� ��� ��������
    public GameObject emptySlotPrefab;           // ������ UI-�������� ��� ���������� �����

    protected List<GameObject> itemUIElements = new List<GameObject>();

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

        List<Item> items = GetItems();  // �������� �� �������� �� ��������� ������ GetItems()
        int totalUsedCapacity = 0;

        foreach (var item in items)
        {
            // ��������� ����� UI-������� ��� ��������
            GameObject newItemUI = Instantiate(itemPrefab, scrollController.content);


            // ������������ ������ ��������
            var itemImage = newItemUI.transform.Find("ItemImage").GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = item.Sprite;  // ������������ ������ � ����� ��������
            }
            else
            {
                Debug.LogWarning("UI-������� �� ������ ��'��� ItemImage ��� ��������� Image.");
            }

            // ����������� ����� �������� �������� �� ������ ��������
            RectTransform itemRect = newItemUI.GetComponent<RectTransform>();
            float itemDisplayWidth = itemRect.sizeDelta.x * item.Size;
            itemRect.sizeDelta = new Vector2(itemDisplayWidth, itemRect.sizeDelta.y);

            // ����������� �� ������, ��� ����������� � ��������� ����
            Button actionButton = newItemUI.GetComponentInChildren<Button>();
            actionButton.onClick.AddListener(() => OnItemAction(item));

            itemUIElements.Add(newItemUI);  // ������ ������� �� ������
            totalUsedCapacity += item.Size;
        }

        // ������ ������ �����, ���� � ��������� ������ � �������
        int emptySlotsCount = Mathf.Max(0, GetBackpackSize() - totalUsedCapacity);
        for (int i = 0; i < emptySlotsCount; i++)
        {
            GameObject emptySlotUI = Instantiate(emptySlotPrefab, scrollController.content);
            itemUIElements.Add(emptySlotUI);  // ������ ������� ���� �� ������
        }

        // ���������� ��������� ������� ���� ��������� �������
        scrollController.Initialize();
    }

    // ����������� ����� ��� ��������� ��������
    protected abstract List<Item> GetItems();

    // ����������� ����� ��� ��������� ������ �������
    protected abstract int GetBackpackSize();

    // ����������� �����, ���� ������� �� ������
    protected abstract void OnItemAction(Item item);

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