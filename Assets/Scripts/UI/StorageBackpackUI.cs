using System.Collections.Generic;
using UnityEngine;

public class StorageBackpackUI : BaseStorageUI
{
    private void OnEnable()
    {
        // Підписуємося на подію оновлення складу
        Player.Instance.backpack.OnStorageUpdated += UpdateInventoryUI;
        UpdateInventoryUI(); // Оновлюємо інтерфейс при активації
    }

    private void OnDisable()
    {
        // Відписуємося від події при вимкненні UI
        Player.Instance.backpack.OnStorageUpdated -= UpdateInventoryUI;
    }
    protected override List<Item> GetItems()
    {
        // Повертає предмети з рюкзака гравця
        return Player.Instance.backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // Повертає загальну ємність рюкзака гравця
        return Player.Instance.backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // Перевіряємо, чи є достатньо місця на складі для предмета
        if (Player.Instance.storage.HasEnoughSpace(item.Size))
        {
            // Видаляємо предмет з рюкзака
            if (Player.Instance.backpack.RemoveItem(item))
            {
                // Додаємо предмет на склад
                Player.Instance.storage.AddItem(item);
                Debug.Log($"Предмет '{item.Name}' переміщено на склад.");

                UpdateInventoryUI();  // Оновлюємо інтерфейс після переміщення предмета
            }
            else
            {
                Debug.LogWarning($"Не вдалося перемістити предмет '{item.Name}' з рюкзака.");
            }
        }
        else
        {
            Debug.LogWarning("Недостатньо місця на складі для переміщення предмета.");
        }
    }
}