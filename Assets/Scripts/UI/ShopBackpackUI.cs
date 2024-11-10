using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBackpackUI : MonoBehaviour
{
    public ScrollController scrollController;   // Компонент ScrollController для керування прокруткою
    public GameObject itemPrefab;               // Префаб UI-елемента для предмета
    public GameObject emptySlotPrefab;          // Префаб UI-елемента для порожнього слота

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
        ClearInventoryUI();  // Очищаємо старі елементи

        // Отримуємо всі предмети з рюкзака гравця
        List<Item> items = Player.Instance.backpack.GetItems();
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

            // Налаштовуємо кнопку "Продати"
            Button sellButton = newItemUI.GetComponentInChildren<Button>();
            sellButton.onClick.AddListener(() => SellItem(item));

            itemUIElements.Add(newItemUI);  // Додаємо елемент до списку
            totalUsedCapacity += item.Size; // Додаємо розмір предмета до загального використаного простору
        }

        // Перевіряємо наявність вільного місця та додаємо порожні слоти
        int emptySlotsCount = Mathf.Max(0, Player.Instance.backpack.Size - totalUsedCapacity);
        for (int i = 0; i < emptySlotsCount; i++)
        {
            GameObject emptySlotUI = Instantiate(emptySlotPrefab, scrollController.content);
            itemUIElements.Add(emptySlotUI);  // Додаємо порожній слот до списку
        }

        // Після оновлення рюкзака ініціалізуємо прокрутку канваса
        scrollController.Initialize();
    }

    private void SellItem(Item item)
    {
        // Видаляємо предмет з рюкзака
        if (Player.Instance.backpack.RemoveItem(item))
        {
            // Додаємо вартість предмета до балансу гравця
            Player.Instance.coins += item.Price;
            Debug.Log($"Предмет '{item.Name}' продано за {item.Price} монет. Новий баланс: {Player.Instance.coins} монет.");

            // Оновлюємо інтерфейс після продажу
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"Не вдалося продати предмет '{item.Name}'.");
        }
    }

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