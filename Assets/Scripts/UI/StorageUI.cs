using System.Collections.Generic;
using UnityEngine;

public class StorageUI : BaseStorageUI
{
    private void OnEnable()
    {
        // Підписуємося на подію оновлення складу
        Player.Instance.Storage.OnStorageUpdated += UpdateInventoryUI;
        UpdateInventoryUI(); // Оновлюємо інтерфейс при активації
    }

    private void OnDisable()
    {
        // Відписуємося від події при вимкненні UI
        Player.Instance.Storage.OnStorageUpdated -= UpdateInventoryUI;
    }
    protected override int GetBackpackSize()
    {
        // Отримує розмір складу, який використовується для відображення
        return Player.Instance.Storage.Size;
    }

    protected override List<Item> GetItems()
    {
        // Отримуємо всі предмети зі складу гравця
        return Player.Instance.Storage.GetItems();
    }

    protected override void OnItemAction(Item item)
    {
        // Перевіряємо, чи є достатньо місця у рюкзаку для предмета
        if (Player.Instance.Backpack.HasEnoughSpace(item.Size))
        {
            // Якщо місця достатньо, видаляємо предмет зі складу та додаємо його в рюкзак
            if (Player.Instance.Storage.RemoveItem(item))
            {
                if (Player.Instance.Backpack.AddItem(item))
                {
                    Debug.Log($"Предмет '{item.Name}' переміщено зі складу в рюкзак.");
                    UpdateInventoryUI();
                }
                else
                {
                    // Повертаємо предмет назад на склад, якщо додавання до рюкзака не вдалося
                    Player.Instance.Storage.AddItem(item);
                    Debug.LogWarning("Не вдалося перемістити предмет у рюкзак.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Недостатньо місця у рюкзаку для цього предмета.");
        }
    }
}