using System.Collections.Generic;
using UnityEngine;

public class GameBackpackUI : BaseStorageUI
{
    private void OnEnable()
    {
        // Підписуємося на подію оновлення складу
        Player.Instance.Backpack.OnStorageUpdated += UpdateInventoryUI;
        UpdateInventoryUI(); // Оновлюємо інтерфейс при активації
    }

    private void OnDisable()
    {
        // Відписуємося від події при вимкненні UI
        Player.Instance.Backpack.OnStorageUpdated -= UpdateInventoryUI;
    }
    protected override List<Item> GetItems()
    {
        // Повертає предмети з рюкзака гравця
        return Player.Instance.Backpack.GetItems();
    }

    protected override int GetBackpackSize()
    {
        // Повертає загальну ємність рюкзака
        return Player.Instance.Backpack.Size;
    }

    protected override void OnItemAction(Item item)
    {
        // Логіка для викидання предмета
        if (Player.Instance.Backpack.RemoveItem(item))
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