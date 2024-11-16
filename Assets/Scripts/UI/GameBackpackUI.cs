using System.Collections.Generic;
using UnityEngine;

public class GameBackpackUI : BaseStorageUI
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
        // Повертає загальну ємність рюкзака
        return Player.Instance.backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // Логіка для викидання предмета
        if (Player.Instance.backpack.RemoveItem(item))
        {
            Debug.Log($"Предмет '{item.Name}' викинуто з рюкзака.");
            UpdateInventoryUI();  // Оновлюємо інтерфейс після викидання предмета
        }
        else
        {
            Debug.LogWarning($"Не вдалося викинути предмет '{item.Name}'.");
        }
    }
}