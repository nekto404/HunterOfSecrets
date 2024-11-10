using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseStorageUI : MonoBehaviour
{
    public ScrollController scrollController;    // Компонент ScrollController для керування прокруткою
    public GameObject itemPrefab;                // Префаб UI-елемента для предмета
    public GameObject emptySlotPrefab;           // Префаб UI-елемента для порожнього слота

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
        ClearInventoryUI();  // Очищаємо старі елементи

        List<Item> items = GetItems();  // Отримуємо всі предмети за допомогою методу GetItems()
        int totalUsedCapacity = 0;

        foreach (var item in items)
        {
            // Створюємо новий UI-елемент для предмета
            GameObject newItemUI = Instantiate(itemPrefab, scrollController.content);
            newItemUI.GetComponentInChildren<Text>().text = item.Name;  // Встановлюємо ім'я предмета

            // Налаштовуємо розмір елемента відповідно до розміру предмета
            RectTransform itemRect = newItemUI.GetComponent<RectTransform>();
            float itemDisplayWidth = itemRect.sizeDelta.x * item.Size;
            itemRect.sizeDelta = new Vector2(itemDisplayWidth, itemRect.sizeDelta.y);

            // Налаштовуємо дію кнопки, яка визначається в похідному класі
            Button actionButton = newItemUI.GetComponentInChildren<Button>();
            actionButton.onClick.AddListener(() => OnItemAction(item));

            itemUIElements.Add(newItemUI);  // Додаємо елемент до списку
            totalUsedCapacity += item.Size;
        }

        // Додаємо порожні слоти, якщо є залишкова ємність у рюкзаку
        int emptySlotsCount = Mathf.Max(0, GetBackpackSize() - totalUsedCapacity);
        for (int i = 0; i < emptySlotsCount; i++)
        {
            GameObject emptySlotUI = Instantiate(emptySlotPrefab, scrollController.content);
            itemUIElements.Add(emptySlotUI);  // Додаємо порожній слот до списку
        }

        // Ініціалізуємо прокрутку канваса після оновлення рюкзака
        scrollController.Initialize();
    }

    // Абстрактний метод для отримання предметів
    protected abstract List<Item> GetItems();

    // Абстрактний метод для отримання розміру рюкзака
    protected abstract int GetBackpackSize();

    // Абстрактний метод, який визначає дію кнопки
    protected abstract void OnItemAction(Item item);

    private void ClearInventoryUI()
    {
        // Видаляємо всі старі елементи з контенту
        foreach (var itemUI in itemUIElements)
        {
            Destroy(itemUI);
        }
        itemUIElements.Clear();  // Очищаємо список
    }
}