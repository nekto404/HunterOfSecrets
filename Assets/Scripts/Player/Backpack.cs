using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    [SerializeField] private int capacity = 10; 
    private List<Item> items = new List<Item>(); 

    public bool AddItem(Item item)
    {
        if (item.Size < 1 || item.Size > 4)
        {
            Debug.LogError("Розмір предмета має бути від 1 до 4.");
            return false;
        }

        if (GetUsedCapacity() + item.Size <= capacity)
        {
            items.Add(item);
            return true;
        }
        else
        {
            Debug.Log("Недостатньо місця для цього предмета.");
            return false;
        }
    }

    public bool RemoveItem(Item item)
    {
        return items.Remove(item);
    }

    public bool CanFitItem(Item item)
    {
        return item.Size >= 1 && item.Size <= 4 && GetUsedCapacity() + item.Size <= capacity;
    }

    public List<Item> GetItems()
    {
        return new List<Item>(items);
    }

    private int GetUsedCapacity()
    {
        int usedCapacity = 0;
        foreach (var item in items)
        {
            usedCapacity += item.Size;
        }
        return usedCapacity;
    }

    public bool MoveItem(int currentIndex, int newIndex)
    {
        if (currentIndex < 0 || currentIndex >= items.Count || newIndex < 0 || newIndex >= items.Count)
        {
            Debug.LogError("Індекс виходить за межі рюкзака.");
            return false;
        }

        var item = items[currentIndex];
        items.RemoveAt(currentIndex);
        items.Insert(newIndex, item);
        return true;
    }

    public (Item left, Item right) GetNeighborItems(int index)
    {
        Item left = index > 0 ? items[index - 1] : null;
        Item right = index < items.Count - 1 ? items[index + 1] : null;
        return (left, right);
    }
}