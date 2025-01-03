using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Player();
            }
            return _instance;
        }
    }

    public int coins = 0;
    public int level = 1;
    public Backpack backpack;
    public Backpack storage;
    public List<PlayerSkill> skills = new List<PlayerSkill>();
    public int[] currentStatuses = new int[10];
    public delegate void CoinsChanged(int newAmount);
    public event CoinsChanged OnCoinsChanged;
    private List<Skill> tileEnterSkills = new List<Skill>();

    private Player()
    {
        backpack = new Backpack();
        storage = new Backpack();
    }



    public void CalculateTileEnterSkills()
    {
        tileEnterSkills.Clear();
        foreach (var item in backpack.GetItems())
        {
            foreach (var skill in item.Skills)
            {
                if (skill.Trigger == Trigger.TileEnter)
                {
                    tileEnterSkills.Add(skill);
                }
            }
        }
        Debug.Log($"Загальна кількість навичок, що активуються при вході на тайл: {tileEnterSkills.Count}");
    }

    public List<Skill> GetTileEnterSkills()
    {
        return tileEnterSkills;
    }

    public void Initialize()
    {
        coins = 40;                        // Початкова кількість монет
        backpack = new Backpack();           // Створення нового рюкзака
        storage = new Backpack();             // Створення нового складу
        skills = new List<PlayerSkill>();    // Порожній список навичок
        currentStatuses = new int[10];       // Масив статусів
        level = 1;                           // Початковий рівень
        OnCoinsChanged?.Invoke(coins);
    }

    public void AddCoins(int amount)
    {
        OnCoinsChanged?.Invoke(coins);
        coins += amount;
        Debug.Log("Coins added: " + amount + ". Total coins: " + coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log("Coins spent: " + amount + ". Remaining coins: " + coins);
            OnCoinsChanged?.Invoke(coins);
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough coins to complete the transaction.");
            return false;
        }
    }


    public void LevelUp()
    {
        level++;
        Debug.Log("Player level increased! New level: " + level);
    }

    public void AddSkill(PlayerSkill skill)
    {
        if (!skills.Contains(skill))
        {
            skills.Add(skill);
            Debug.Log("Skill added: " + skill.Name);
        }
    }

    public void ApplyStatus(int statusIndex)
    {
        if (statusIndex >= 0 && statusIndex < currentStatuses.Length)
        {
            currentStatuses[statusIndex]++;
            Debug.Log("Status " + statusIndex + " applied. Current count: " + currentStatuses[statusIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid status index.");
        }
    }

    public float GetSpeedModifier()
    {
        float speedModifier = 1.0f;
        if (currentStatuses[1] > 0)
        {
            speedModifier -= 0.01f * currentStatuses[1]; // Зменшення швидкості на 1% за кожну одиницю ефекту 1
        }
        return Mathf.Max(speedModifier, 0.1f); // Мінімальна швидкість 10%
    }

    public bool ShouldStop()
    {
        if (currentStatuses[2] > 20)
        {
            currentStatuses[2] -= 20; // Зменшення значення ефекту 2 на 20
            return true;
        }
        return false;
    }

    public void RemoveStatus(int statusIndex)
    {
        if (statusIndex >= 0 && statusIndex < currentStatuses.Length && currentStatuses[statusIndex] > 0)
        {
            currentStatuses[statusIndex]--;
            Debug.Log("Status " + statusIndex + " removed. Current count: " + currentStatuses[statusIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid status index or no active statuses to remove.");
        }
    }

    public bool CanAfford(int amount)
    {
        return coins >= amount;
    }

    public void ClearInstance()
    {
        _instance = null;
    }

    public string GetActiveStatuses()
    {
        List<string> activeStatuses = new List<string>();
        for (int i = 0; i < currentStatuses.Length; i++)
        {
            if (currentStatuses[i] > 0)
            {
                activeStatuses.Add($"Effect {i}: {currentStatuses[i]}");
            }
        }
        return string.Join(", ", activeStatuses);
    }
}