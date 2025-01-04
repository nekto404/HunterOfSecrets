using System.Collections.Generic;
using UnityEngine;

public class ShopBackpackUI : BaseStorageUI
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
        // Логіка продажу предмета
        if (Player.Instance.Backpack.RemoveItem(item))
        {
            Player.Instance.AddCoins(item.Price);
            Debug.Log($"Предмет '{item.Name}' продано за {item.Price} монет. Новий баланс: {Player.Instance.Coins} монет.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning($"Не вдалося продати предмет '{item.Name}'.");
        }
    }
}